using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail;
using SeaQuail.Schema;
using SeaQuail.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using SeaQuail_SQLite.Schema;
using SeaQuail.SchemaQuery;

namespace SeaQuail_SQLite
{
    public class SQLiteAdapter : SQAdapter
    {
        protected override DbConnection CreateConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        public override void ExecuteQuery(string query, List<SQParameter> parameters)
        {
            SQLiteConnection con = GetConnection<SQLiteConnection>();
            bool alreadyOpen = con.State == ConnectionState.Open;
            if (!alreadyOpen)
            {
                con.Open();
            }
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(query, con, (SQLiteTransaction)Transaction);

                foreach (SQParameter p in parameters)
                {
                    cmd.Parameters.AddWithValue(p.Name, p.Value);
                }
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (!alreadyOpen)
                {
                    con.Close();
                }
            }
        }

        public override SQSelectResult Select(string query, List<SQParameter> parameters)
        {
            SQLiteConnection con = GetConnection<SQLiteConnection>();
            bool alreadyOpen = con.State == ConnectionState.Open;
            if (!alreadyOpen)
            {
                con.Open();
            }

            SQLiteCommand cmd = new SQLiteCommand(query, con, (SQLiteTransaction)Transaction);

            foreach (SQParameter p in parameters)
            {
                cmd.Parameters.AddWithValue(p.Name, p.Value);
            }

            return new SQSelectResult(alreadyOpen ? null : con, cmd.ExecuteReader());
        }

        public override string Write(SQInsertQuery q)
        {
            string res = base.Write(q);

            if (q.ReturnID)
            {
                res += ";SELECT last_insert_rowid() ID;";
            }

            return res;
        }

        public override string Write(SQSelectQuery q)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT");
            
            if (q.Distinct)
            {
                sb.AppendLine("\tDISTINCT");
            }

            for (int i = 0; i < q.Columns.Count; i++)
            {
                SQAliasableObject col = q.Columns[i];
                sb.AppendLine((i == 0 ? "\t" : "\t,") + col.Actual.Write(this) + (string.IsNullOrEmpty(col.Alias) ? string.Empty : " '" + col.Alias + "'"));
            }

            sb.AppendLine("FROM " + Write(q.From));

            if (q.Condition != null)
            {
                sb.AppendLine(string.Format("WHERE {0}", Write(q.Condition)));
            }

            if (q.SortColumns.Count > 0)
            {
                List<string> cols = new List<string>();
                foreach (SQSortColumn col in q.SortColumns)
                {
                    cols.Add(col.Column + (col.Direction == SortOrder.Ascending ? " ASC"
                        : col.Direction == SortOrder.Descending ? " DESC" : ""));
                }
                sb.AppendLine("ORDER BY " + string.Join(", ", cols.ToArray()));
            }

            if (q.Top > 0)
            {
                sb.AppendLine("LIMIT 0, " + q.Top);
            }
            else if (q.RecordCount > 0)
            {
                sb.AppendLine(string.Format("LIMIT {0}, {1}", q.RecordStart, q.RecordCount));
            }

            // TODO: total rows

            return sb.ToString();
        }

        public override string GetSafeObjectName(string name)
        {
            return "[" + name + "]";
        }

        public override string CreateVariable(string name)
        {
            return "@" + name;
        }

        public override long GetTotalRecordCount(SQSelectResult result)
        {
            throw new NotImplementedException();
        }

        public override SQTable GetTable(string name)
        {
            SQLiteConnection sqlcon = new SQLiteConnection(ConnectionString);
            sqlcon.Open();
            DataTable dt = sqlcon.GetSchema("Columns", new string[] { null, null, name, null });
            DataTable fks = sqlcon.GetSchema("ForeignKeys", new string[] { null, null, name, null });
            sqlcon.Close();

            SQTable res = null;
            if (dt.Rows.Count > 0)
            {
                res = new SQTable();
                res.Name = name;
                foreach (DataRow row in dt.Rows)
                {
                    SQDataTypes dataType = GetTypeFromName(row["DATA_TYPE"].ToString());
                    int precision = row["NUMERIC_PRECISION"] == DBNull.Value ? 0 : Convert.ToInt32(row["NUMERIC_PRECISION"]);
                    res.Columns.Add(new SQColumn_SQLite()
                    {
                        Name = row["COLUMN_NAME"].ToString(),
                        DataType = dataType != SQDataTypes.Int32 ? dataType : precision == 19 ? SQDataTypes.Int64 : SQDataTypes.Int32,
                        IsIdentity = row["AUTOINCREMENT"].Equals(true),
                        IsPrimary = row["PRIMARY_KEY"].Equals(true),
                        DefaultValue = row["COLUMN_DEFAULT"],
                        Length = Convert.ToInt64(row["CHARACTER_MAXIMUM_LENGTH"]),
                        Nullable = row["IS_NULLABLE"].Equals(true),
                        Precision = precision,
                        Scale = row["NUMERIC_SCALE"] == DBNull.Value ? 0 : Convert.ToInt32(row["NUMERIC_SCALE"])
                    });
                }
                foreach (DataRow row in fks.Rows)
                {
                    ((SQColumn_SQLite)res.GetColumnByName(row["FKEY_FROM_COLUMN"].ToString())).ForeignKey = new SQColumn() 
                    {
                        Name = row["FKEY_TO_COLUMN"].ToString(),
                        Table = new SQTable() { Name = row["FKEY_TO_TABLE"].ToString() } 
                    };
                }
            }

            return res;
        }

        public override string WriteCreateTable(SQTable table)
        {
            // create the table
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + table.Name);
            sb.AppendLine("(");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                SQColumn col = table.Columns[i];
                sb.AppendLine((i > 0 ? "\t," : "\t") + GetCreateColumnText(col));
            }
            foreach (SQColumn col in table.Columns)
            {
                if (col is SQColumn_SQLite)
                {
                    SQColumn_SQLite l = (SQColumn_SQLite)col;
                    if (l.ForeignKey != null)
                    {
                        sb.AppendLine(string.Format(
                            "\t,CONSTRAINT [FK_{0}_{1}_{2}_{3}] FOREIGN KEY ([{1}]) REFERENCES [{2}] ([{3}])", 
                            l.Table.Name, l.Name, l.ForeignKey.Table.Name, l.ForeignKey.Name));
                    }
                }
            }
            sb.AppendLine(")");

            return sb.ToString();
        }

        public override string WriteRemoveTable(string name)
        {
            return "DROP TABLE " + GetSafeObjectName(name) + ";";
        }

        public override string WriteAddColumn(SQColumn col)
        {
            throw new NotImplementedException();
        }

        public override string WriteAddForeignKey(SQColumn from, SQColumn to)
        {
            // add foreign key to from property
            SQTable table = GetTable(from.Table.Name);
            ((SQColumn_SQLite)table.GetColumnByName(from.Name)).ForeignKey = to;

            // get list of columns
            List<string> columns = new List<string>();
            foreach (SQColumn col in table.Columns)
            {
                columns.Add(col.Name);
            }

            return WriteUpdateTable(table, columns, columns);
        }

        private string WriteUpdateTable(SQTable table, List<string> newColumns, List<string> oldColumns)
        {
            StringBuilder sb = new StringBuilder();

            // rename the existing table
            string tableRename = table.Name + "_" + Guid.NewGuid().ToString().Replace("-", "");
            sb.AppendLine(string.Format("ALTER TABLE [{0}] RENAME TO [{1}];\r\n\r\n", table.Name, tableRename));

            // create the new table
            sb.AppendLine(WriteCreateTable(table) + ";\r\n\r\n");

            // copy data from the old table
            SQInsertFromQuery copyQuery = new SQInsertFromQuery(new SQAliasableObject(table.Name), newColumns, oldColumns)
            {
                From = new SQFromClause(tableRename)
            };
            sb.AppendLine(Write(copyQuery) + ";\r\n\r\n");

            // remove the old table
            sb.AppendLine(WriteRemoveTable(tableRename));

            return sb.ToString();
        }

        public override void AddForeignKey(SQColumn from, SQColumn to)
        {
            ExecuteUpdateTable(WriteAddForeignKey(from, to));
        }

        public override void RemoveColumn(SQColumn col)
        {
            ExecuteUpdateTable(WriteRemoveColumn(col));
        }

        public override void RenameColumn(SQColumn col, string oldName)
        {
            ExecuteUpdateTable(WriteRenameColumn(col, oldName));
        }

        private void ExecuteUpdateTable(string query)
        {
            SQLiteConnection con = GetConnection<SQLiteConnection>();
            bool alreadyOpen = con.State == ConnectionState.Open;
            if (!alreadyOpen)
            {
                con.Open();
            }

            string[] stmts = query.Split(new string[] { ";\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string stmt in stmts)
            {
                SQLiteCommand cmd = new SQLiteCommand(stmt, con);
                cmd.ExecuteNonQuery();
            }

            if (!alreadyOpen)
            {
                con.Close();
            }
        }

        public override SQColumn GetForeignKeyColumn(SQColumn local)
        {
            throw new NotImplementedException();
        }

        public override string WriteRemoveColumn(SQColumn column)
        {
            // add foreign key to from property
            SQTable table = GetTable(column.Table.Name);
            table.Columns.Remove(table.GetColumnByName(column.Name));

            // get list of columns
            List<string> columns = new List<string>();
            foreach (SQColumn col in table.Columns)
            {
                columns.Add(col.Name);
            }

            return WriteUpdateTable(table, columns, columns);
        }

        public override string WriteRenameColumn(SQColumn column, string oldName)
        {
            SQTable table = GetTable(column.Table.Name);

            // get list of columns
            List<string> newColumns = new List<string>();
            List<string> oldColumns = new List<string>();
            foreach (SQColumn col in table.Columns)
            {
                if (oldName.ToUpper() == col.Name.ToUpper())
                {
                    newColumns.Add(column.Name);
                    oldColumns.Add(col.Name);
                }
                else
                {
                    newColumns.Add(col.Name);
                    oldColumns.Add(col.Name);
                }
            }

            table.GetColumnByName(oldName).Name = column.Name;

            return WriteUpdateTable(table, newColumns, oldColumns);
        }
        
        public SQLiteAdapter() : base() { }
        public SQLiteAdapter(string connectionString) : base(connectionString) { }

        private string GetCreateColumnText(SQColumn col)
        {
            StringBuilder sb = new StringBuilder(GetSafeObjectName(col.Name));
            sb.Append(" " + GetSQLNameForType(col));
            if (col.IsPrimary)
            {
                sb.Append(" PRIMARY KEY");
            }
            if (col.IsIdentity)
            {
                sb.Append(" AUTOINCREMENT");
            }
            if (!col.Nullable)
            {
                sb.Append(" NOT NULL");
            }

            return sb.ToString();
        }

        public string GetSQLNameForType(SQColumn col)
        {
            switch (col.DataType)
            {
                case SQDataTypes.Boolean:
                    return "BIT";
                case SQDataTypes.Bytes:
                    return "BLOB";
                case SQDataTypes.DateTime:
                    return "DATETIME";
                case SQDataTypes.Decimal:
                    return string.Format("NUMERIC({0}, {1})", col.Precision, col.Scale);
                case SQDataTypes.Int16:
                    return "SMALLINT";
                case SQDataTypes.Int32:
                    return "INT";
                case SQDataTypes.Int64:
                    return "INTEGER";
                case SQDataTypes.Float:
                    return "FLOAT";
                case SQDataTypes.String:
                    return col.Length <= 0 || col.Length > 65535 ? "TEXT" : string.Format("VARCHAR({0})", col.Length);
            }
            return null;
        }

        public SQDataTypes GetTypeFromName(string typeName)
        {
            switch (typeName.ToUpper())
            {
                case "SMALLINT":
                    return SQDataTypes.Int16;
                case "INT":
                    return SQDataTypes.Int32;
                case "INTEGER":
                case "BIGINT":
                    return SQDataTypes.Int64;
                case "DECIMAL":
                case "NUMERIC":
                case "MONEY":
                    return SQDataTypes.Decimal;
                case "FLOAT":
                    return SQDataTypes.Float;
                case "VARCHAR":
                case "CHAR":
                case "NVARCHAR":
                case "NCHAR":
                case "TEXT":
                case "NTEXT":
                case "CLOB":
                    return SQDataTypes.String;
                case "DATETIME":
                    return SQDataTypes.DateTime;
                case "BIT":
                    return SQDataTypes.Boolean;
                case "BLOB":
                    return SQDataTypes.Bytes;

            }
            throw new Exception("Unmappped SQL Type: " + typeName);
        }

        public override string Write(SQAddColumn addColumn)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQAddForeignKey addKey)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQAddIndex addIndex)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQCreateTable createTable)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQInsertColumn insertColumn)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQRemoveColumn removeColumn)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQRemoveForeignKey removeKey)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQRemoveIndex removeIndex)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQRemoveTable removeTable)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQRenameColumn renameColumn)
        {
            throw new NotImplementedException();
        }

        public override string Write(SQRenameTable renameTable)
        {
            throw new NotImplementedException();
        }
    }
}
