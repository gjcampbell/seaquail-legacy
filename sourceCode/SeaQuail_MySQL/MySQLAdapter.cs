using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail;
using SeaQuail.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using SeaQuail.Schema;
using System.Data.Common;
using System.Data;
using SeaQuail.SchemaQuery;

namespace SeaQuail_MySQL
{
    /// <summary>
    /// TODO: fix total row count
    /// </summary>
    public class MySQLAdapter : SQAdapter
    {
        #region Adapter Implementation
        protected override DbConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        #region Data
        public override void ExecuteQuery(string query, List<SQParameter> parameters)
        {
            MySqlConnection con = GetConnection<MySqlConnection>();
            bool alreadyOpen = (con.State == ConnectionState.Open);
            if (!alreadyOpen)
            {
                con.Open();
            }

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, con, (MySqlTransaction)Transaction);
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
            MySqlConnection con = GetConnection<MySqlConnection>();
            bool alreadyOpen = (con.State == ConnectionState.Open);
            if (!alreadyOpen)
            {
                con.Open();
            }

            MySqlCommand cmd = new MySqlCommand(query, con, (MySqlTransaction)Transaction);
            foreach (SQParameter p in parameters)
            {
                cmd.Parameters.AddWithValue(p.Name, p.Value);
            }
            MySqlDataReader rdr = cmd.ExecuteReader();

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
            sb.AppendLine("SELECT" + (q.IncludeTotalRows ? " SQL_CALC_FOUND_ROWS" : ""));
            for (int i = 0; i < q.Columns.Count; i++)
            {
                SQAliasableObject col = q.Columns[i];
                sb.AppendLine((i == 0 ? "\t" : "\t,") + Write(col.Actual) + (string.IsNullOrEmpty(col.Alias) ? string.Empty : " '" + col.Alias + "'"));
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

            if (q.IncludeTotalRows)
            {
                sb.AppendLine(";SELECT FOUND_ROWS()");
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
            return "`" + name + "`";
        }
        #endregion

        #region Schema
        public override SQTable GetTable(string name)
        {
            string varTable = CreateVariable("Table");
            string varPK = CreateVariable("PK");
            SQSelectQuery q = new SQSelectQuery();
            q.Columns.AddRange(new List<SQAliasableObject>
            {
                new SQAliasableObject("cols.COLUMN_NAME"),
                new SQAliasableObject("IS_NULLABLE"),
                new SQAliasableObject("DATA_TYPE"),
                new SQAliasableObject("CHARACTER_MAXIMUM_LENGTH"),
                new SQAliasableObject("NUMERIC_PRECISION"),
                new SQAliasableObject("NUMERIC_SCALE"),
                new SQAliasableObject("EXTRA", "IS_IDENTITY"),
                new SQAliasableObject("CONSTRAINT_TYPE")
            });
            q.From = new SQFromClause(new SQFromTable("INFORMATION_SCHEMA.COLUMNS", "cols")
            {
                Join = new SQJoin("INFORMATION_SCHEMA.KEY_COLUMN_USAGE", "tuse")
                {
                    JoinType = SQJoinTypes.Left,
                    Predicate = new SQCondition("tuse.COLUMN_NAME", SQRelationOperators.Equal, "cols.COLUMN_NAME")
                        .And("tuse.TABLE_NAME", SQRelationOperators.Equal, "cols.TABLE_NAME"),
                    Join = new SQJoin("INFORMATION_SCHEMA.TABLE_CONSTRAINTS", "tcons")
                    {
                        JoinType = SQJoinTypes.Left,
                        Predicate = new SQCondition("tcons.CONSTRAINT_NAME", SQRelationOperators.Equal, "tuse.CONSTRAINT_NAME")
                            .And("tcons.TABLE_NAME", SQRelationOperators.Equal, "cols.TABLE_NAME")
                            .And("tcons.CONSTRAINT_TYPE", SQRelationOperators.Equal, varPK)
                    }
                }
            });
            q.Condition = new SQCondition("cols.TABLE_NAME", SQRelationOperators.Equal, varTable);
            q.Parameters.Add(new SQParameter(varTable, name));
            q.Parameters.Add(new SQParameter(varPK, "PRIMARY KEY"));

            SQSelectResult selres = Select(q);
            MySqlDataReader rdr = (MySqlDataReader)selres.Reader;
            try
            {
                if (rdr.HasRows)
                {
                    SQTable res = new SQTable()
                    {
                        Name = name
                    };
                    while (rdr.Read())
                    {
                        res.Columns.Add(new SQColumn()
                        {
                            Table = res,
                            Name = rdr.GetString(0),
                            DataType = GetTypeFromName(rdr.GetString(2)),
                            Length = rdr.IsDBNull(3) ? 0 : rdr.GetInt64(3),
                            Nullable = rdr.GetString(1) == "YES",
                            Precision = rdr.IsDBNull(4) ? 0 : Convert.ToInt32(rdr.GetInt64(4)),
                            Scale = rdr.IsDBNull(5) ? 0 : rdr.GetInt32(5),
                            IsIdentity = rdr.GetString(6).ToLower().Contains("auto_increment"),
                            IsPrimary = rdr.IsDBNull(7) ? false : rdr.GetString(7) == "PRIMARY KEY"
                        });
                    }

                    return res;
                }
            }
            finally
            {
                selres.Close();
            }

            return null;
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
                if (col.IsPrimary)
                {
                    sb.AppendLine("\t,PRIMARY KEY (" + col.Name + ")");
                }
            }
            sb.AppendLine(")");
            sb.AppendLine("ENGINE=INNODB");

            return sb.ToString();
        }

        public override string WriteRemoveTable(string name)
        {
            return "DROP TABLE `" + name + "`";
        }

        public override string WriteAddColumn(SQColumn col)
        {
            throw new NotImplementedException();
        }

        public override string WriteAddForeignKey(SQColumn from, SQColumn to)
        {
            return string.Format("ALTER TABLE {0} ADD FOREIGN KEY ({1}) REFERENCES {2}({3});", from.Table.Name, from.Name, to.Table.Name, to.Name);
        }

        public override SQColumn GetForeignKeyColumn(SQColumn local)
        {
            throw new NotImplementedException();
        }

        public override string WriteRemoveColumn(SQColumn col)
        {
            return "ALTER TABLE `" + col.Table.Name + "` DROP `" + col.Name + "`";
        }

        public override string WriteRenameColumn(SQColumn col, string oldName)
        {
            return "ALTER TABLE `" + col.Table.Name + "` CHANGE COLUMN `" + oldName + "` " + GetCreateColumnText(col);
        }
        #endregion
        #endregion

        public MySQLAdapter() : base() { }
        public MySQLAdapter(string connectionString) : base(connectionString) { }

        private string GetCreateColumnText(SQColumn col)
        {
            return string.Format("{0} {1} {2} NULL {3}",
                GetSafeObjectName(col.Name), GetSQLNameForType(col), col.Nullable ? "" : "NOT", col.IsIdentity ? "AUTO_INCREMENT" : "");
        }

        public string GetSQLNameForType(SQColumn col)
        {
            switch (col.DataType)
            {
                case SQDataTypes.Boolean:
                    return "BIT";
                case SQDataTypes.Bytes:
                    return col.Length > 65535 ? "BLOB" : string.Format("VARBINARY({0})", col.Length < 1 ? "65535" : col.Length.ToString());
                case SQDataTypes.DateTime:
                    return "DATETIME";
                case SQDataTypes.Decimal:
                    return string.Format("DECIMAL({0}, {1})", col.Precision, col.Scale);
                case SQDataTypes.Int16:
                    return "SMALLINT";
                case SQDataTypes.Int32:
                    return "INT";
                case SQDataTypes.Int64:
                    return "BIGINT";
                case SQDataTypes.Float:
                    return "FLOAT";
                case SQDataTypes.String:
                    return col.Length > 65535 ? "LONGTEXT" : string.Format("VARCHAR({0})", col.Length < 1 ? "65535" : col.Length.ToString());
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
                case "BIGINT":
                    return SQDataTypes.Int64;
                case "DECIMAL":
                case "MONEY":
                    return SQDataTypes.Decimal;
                case "FLOAT":
                    return SQDataTypes.Float;
                case "VARCHAR":
                case "CHAR":
                case "TEXT":
                case "MEDIUMTEXT":
                case "LONGTEXT":
                    return SQDataTypes.String;
                case "VARBINARY":
                    return SQDataTypes.Bytes;
                case "DATE":
                case "DATETIME":
                    return SQDataTypes.DateTime;
                case "BIT":
                    return SQDataTypes.Boolean;

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
