using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail;
using System.Data.Common;
using Npgsql;
using SeaQuail.Schema;
using SeaQuail.Data;
using System.Data.SqlClient;
using System.Data;

namespace SeaQuail_PostgreSQL
{
    public class PostgreSQLAdapter : SQAdapter
    {
        #region Adapter Implementation
        protected override DbConnection CreateConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        #region Data
        public override void ExecuteQuery(string query, List<SQParameter> parameters)
        {
            NpgsqlConnection con = GetConnection<NpgsqlConnection>();
            bool alreadyOpen = (con.State == ConnectionState.Open);
            if (!alreadyOpen)
            {
                con.Open();
            }

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(query, con, (NpgsqlTransaction)Transaction);
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
            NpgsqlConnection con = GetConnection<NpgsqlConnection>();
            bool alreadyOpen = (con.State == ConnectionState.Open);
            if (!alreadyOpen)
            {
                con.Open();
            }

            NpgsqlCommand cmd = new NpgsqlCommand(query, con, (NpgsqlTransaction)Transaction);
            foreach (SQParameter p in parameters)
            {
                cmd.Parameters.AddWithValue(p.Name, p.Value);
            }
            NpgsqlDataReader rdr = cmd.ExecuteReader();

            return new SQSelectResult(alreadyOpen ? null : con, rdr);
        }

        public override Int64 GetTotalRecordCount(SQSelectResult result)
        {
            if (result.Reader.NextResult())
            {
                if (result.Reader.Read())
                {
                    return result.Reader.GetInt64(0);
                }
            }
            throw new Exception("Could not get record count.");
        }

        public override string CreateVariable(string name)
        {
            return "@" + name;
        }

        public override string Write(SQSelectQuery q)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT");
            for (int i = 0; i < q.Columns.Count; i++)
            {
                SQAliasableObject col = q.Columns[i];
                sb.AppendLine((i == 0 ? "\t" : "\t,") + Write(col.Actual) + (string.IsNullOrEmpty(col.Alias) ? string.Empty : " '" + col.Alias + "'"));
            }

            // branch off from original string builder to create
            // a get row count statement
            StringBuilder sb2 = new StringBuilder();
            sb2.AppendLine("FROM " + Write(q.From));

            if (q.Condition != null)
            {
                sb2.AppendLine(string.Format("WHERE {0}", Write(q.Condition)));
            }

            if (q.SortColumns.Count > 0)
            {
                List<string> cols = new List<string>();
                foreach (SQSortColumn col in q.SortColumns)
                {
                    cols.Add(col.Column + (col.Direction == SortOrder.Ascending ? " ASC"
                        : col.Direction == SortOrder.Descending ? " DESC" : ""));
                }
                sb2.AppendLine("ORDER BY " + string.Join(", ", cols.ToArray()));
            }

            sb.AppendLine(sb2.ToString());
            if (q.Top > 0)
            {
                sb.AppendLine("LIMIT " + q.Top);
            }
            else if (q.RecordCount > 0)
            {
                sb.AppendLine(string.Format("LIMIT {0} OFFSET {1}", q.RecordCount, q.RecordStart));
            }

            if (q.IncludeTotalRows)
            {
                sb.AppendLine(";SELECT COUNT(*)\r\n" + sb2.ToString());
            }

            return sb.ToString();
        }

        public override string Write(SQInsertQuery q)
        {
            StringBuilder sb = new StringBuilder(base.Write(q));

            if (q.ReturnID)
            {
                sb.AppendLine(";SELECT LAST_INSERT_ID()");
            }

            return sb.ToString();
        }
        
        public override string GetSafeObjectName(string name)
        {
            return name.Contains(" ") ? "\"" + name + "\"" : name;
        }
        #endregion

        #region Schema
        public override SQTable GetTable(string name)
        {
            NpgsqlConnection sqlcon = new NpgsqlConnection(ConnectionString);
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
                    res.Columns.Add(new SQColumn()
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
                //foreach (DataRow row in fks.Rows)
                //{
                //    ((SQColumn)res.GetColumnByName(row["FKEY_FROM_COLUMN"].ToString())).ForeignKey = new SQColumn()
                //    {
                //        Name = row["FKEY_TO_COLUMN"].ToString(),
                //        Table = new SQTable() { Name = row["FKEY_TO_TABLE"].ToString() }
                //    };
                //}
            }

            return res;
        }

        public override string WriteCreateTable(SQTable table)
        {
            // create the table
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE " + GetSafeObjectName(table.Name));
            sb.AppendLine("(");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                SQColumn col = table.Columns[i];
                sb.AppendLine((i > 0 ? "\t," : "\t") + GetCreateColumnText(col));
            }

            foreach (SQColumn col in table.Columns)
            {
                if (col.IsPrimary)
                {
                    sb.AppendLine(string.Format("\t,CONSTRAINT {0} PRIMARY KEY ({1})", GetSafeObjectName("PK_" + table.Name + "_" + col.Name), GetSafeObjectName(col.Name)));
                }
            }

            sb.AppendLine(");");

            return sb.ToString();
        }

        public override string WriteRemoveTable(string name)
        {
            return "DROP TABLE " + GetSafeObjectName(name);
        }

        public override string WriteAddColumn(SQColumn col)
        {
            throw new NotImplementedException();
        }

        public override string WriteAddForeignKey(SQColumn from, SQColumn to)
        {
            string fkname = string.Format("FK_{0}_{1}_{2}_{3}",
                from.Table.Name, from.Name, to.Table.Name, to.Name).Replace(" ", "");
            return string.Format("ALTER TABLE {0} ADD CONSTRAINT {4} FOREIGN KEY ({1}) REFERENCES {2}({3});",
                GetSafeObjectName(from.Table.Name), GetSafeObjectName(from.Name), GetSafeObjectName(to.Table.Name), GetSafeObjectName(to.Name), fkname);
        }

        public override SQColumn GetForeignKeyColumn(SQColumn local)
        {
            throw new NotImplementedException();
        }

        public override string WriteRemoveColumn(SQColumn col)
        {
            return "ALTER TABLE " + GetSafeObjectName(col.Table.Name) + " DROP `" + col.Name + "`";
        }

        public override string WriteRenameColumn(SQColumn col, string oldName)
        {
            return "ALTER TABLE " + GetSafeObjectName(col.Table.Name) + " CHANGE COLUMN `" + oldName + "` " + GetCreateColumnText(col);
        }
        #endregion
        #endregion

        public PostgreSQLAdapter() : base() { }
        public PostgreSQLAdapter(string connectionString) : base(connectionString) { }

        private string GetCreateColumnText(SQColumn col)
        {
            if (col.IsIdentity && (col.DataType == SQDataTypes.Int64 || col.DataType == SQDataTypes.Int32))
                return string.Format("{0} {1}", GetSafeObjectName(col.Name), col.DataType == SQDataTypes.Int64 ? "BIGSERIAL" : "SERIAL");
            else
                return string.Format("{0} {1} {2} NULL {3}",
                    GetSafeObjectName(col.Name), GetSQLNameForType(col), col.Nullable ? "" : "NOT", col.IsIdentity ? "SERIAL" : "");
        }

        public string GetSQLNameForType(SQColumn col)
        {
            switch (col.DataType)
            {
                case SQDataTypes.Boolean:
                    return "BOOLEAN";
                case SQDataTypes.Bytes:
                    return "BYTEA";
                    //return col.Length > 65535 ? "BYTEA" : string.Format("BYTEA{0}", col.Length < 1 ? "" : "(" + col.Length.ToString() + ")");
                case SQDataTypes.DateTime:
                    return "TIMESTAMP";
                case SQDataTypes.Decimal:
                    return string.Format("DECIMAL({0}, {1})", col.Precision, col.Scale);
                case SQDataTypes.Int16:
                    return "SMALLINT";
                case SQDataTypes.Int32:
                    return "INTEGER";
                case SQDataTypes.Int64:
                    return "BIGINT";
                case SQDataTypes.Float:
                    return "REAL";
                case SQDataTypes.String:
                    return col.Length > 65535 ? "TEXT" : string.Format("VARCHAR{0}", col.Length < 1 ? "" : "(" + col.Length.ToString() + ")");
            }
            return null;
        }

        public SQDataTypes GetTypeFromName(string typeName)
        {
            switch (typeName.ToUpper())
            {
                case "SMALLINT":
                    return SQDataTypes.Int16;
                case "INTEGER":
                case "SERIAL":
                    return SQDataTypes.Int32;
                case "BIGINT":
                case "BIGSERIAL":
                    return SQDataTypes.Int64;
                case "DECIMAL":
                case "NUMERIC":
                case "MONEY":
                case "DOUBLE PRECISION":
                    return SQDataTypes.Decimal;
                case "REAL":
                    return SQDataTypes.Float;
                case "CHARACTER VARYING":
                case "VARCHAR":
                case "CHARACTER":
                case "CHAR":
                case "TEXT":
                    return SQDataTypes.String;
                case "BYTEA":
                    return SQDataTypes.Bytes;
                case "TIMESTAMP":
                case "DATE":
                    return SQDataTypes.DateTime;
                case "BOOLEAN":
                    return SQDataTypes.Boolean;

            }
            throw new Exception("Unmappped SQL Type: " + typeName);
        }

        public override string Write(SeaQuail.SchemaQuery.SQAddColumn addColumn)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQAddForeignKey addKey)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQAddIndex addIndex)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQCreateTable createTable)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQInsertColumn insertColumn)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQRemoveColumn removeColumn)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQRemoveForeignKey removeKey)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQRemoveIndex removeIndex)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQRemoveTable removeTable)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQRenameColumn renameColumn)
        {
            throw new NotImplementedException();
        }

        public override string Write(SeaQuail.SchemaQuery.SQRenameTable renameTable)
        {
            throw new NotImplementedException();
        }
    }
}
