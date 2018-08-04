using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail;
using System.Data.Common;
using SeaQuail.Data;
using System.Data.SqlClient;
using SeaQuail.Schema;
using System.Data;
using SeaQuail.SchemaQuery;

namespace SeaQuail_SQLServer
{
    public class SQLServerAdapter : SQAdapter
    {
        #region Adapter Implementation
        protected override DbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public override void ExecuteQuery(string query, List<SQParameter> parameters)
        {
            SqlConnection sqlcon = GetConnection<SqlConnection>();
            bool alreadyOpen = sqlcon.State == ConnectionState.Open;
            if (!alreadyOpen)
            {
                sqlcon.Open();
            }

            try
            {
                SqlCommand sqlcmd = new SqlCommand(query, sqlcon, (SqlTransaction)Transaction);

                foreach (SQParameter p in parameters)
                {
                    sqlcmd.Parameters.AddWithValue(p.Name, p.Value);
                }
                sqlcmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to execute query: " + query, ex);
            }
            finally
            {
                if (!alreadyOpen)
                {
                    sqlcon.Close();
                }
            }
        }

        public override SQSelectResult Select(string query, List<SQParameter> parameters)
        {
            SqlConnection con = GetConnection<SqlConnection>();
            bool alreadyOpen = con.State == ConnectionState.Open;
            if (!alreadyOpen)
            {
                con.Open();
            }

            SqlCommand cmd = new SqlCommand(query, con, (SqlTransaction)Transaction);

            foreach (SQParameter p in parameters)
            {
                cmd.Parameters.AddWithValue(p.Name, p.Value);
            }

            return new SQSelectResult(alreadyOpen ? null : con, cmd.ExecuteReader());
        }

        public override SQSelectResult Select(SQSelectQuery query)
        {
            string queryText = Write(query);

            SqlConnection con = GetConnection<SqlConnection>();
            bool alreadyOpen = con.State == ConnectionState.Open;
            if (!alreadyOpen)
            {
                con.Open();
            }
            SqlCommand sqlcmd = new SqlCommand(queryText, con, (SqlTransaction)Transaction);

            foreach (SQParameter p in query.Parameters)
            {
                sqlcmd.Parameters.AddWithValue(p.Name, p.Value);
            }
            SqlDataReader rdr = sqlcmd.ExecuteReader();

            SQSelectResult res = new SQSelectResult(alreadyOpen ? null : con, rdr);
            if (query.IncludeTotalRows)
            {
                if (res.Reader.Read())
                {
                    SetStoredTotalRecordCount(res, res.Reader.IsDBNull(res.Reader.FieldCount - 1) ? 0 : res.Reader.GetInt64(res.Reader.FieldCount - 1));
                }
            }

            return res;
        }

        public override Int64 GetTotalRecordCount(SQSelectResult result)
        {
            return GetStoredTotalRecordCount(result);
        }
        
        public override string Write(SQInsertQuery q)
        {
            StringBuilder sb = new StringBuilder(base.Write(q));

            if (q.ReturnID)
            {
                sb.AppendLine("SELECT @@IDENTITY AS [ID]");
            }

            return sb.ToString();
        }

        public override string Write(SQSelectQuery q)
        {
            string sortColumns = null;
            if (q.SortColumns.Count > 0)
            {
                List<string> cols = new List<string>();
                foreach (SQSortColumn col in q.SortColumns)
                {
                    cols.Add(col.Column + (col.Direction == SortOrder.Ascending ? " ASC"
                        : col.Direction == SortOrder.Descending ? " DESC" : ""));
                }
                sortColumns = "ORDER BY " + string.Join(", ", cols.ToArray());
            }

            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("SELECT");

            if (q.Distinct)
            {
                sb.AppendLine("\tDISTINCT");
            }

            if (q.Top > 0)
            {
                sb.AppendLine("\tTOP " + q.Top);
            }

            for (int i = 0; i < q.Columns.Count; i++)
            {
                SQAliasableObject col = q.Columns[i];
                sb.AppendLine((i == 0 ? "\t" : "\t,") + Write(col.Actual) + (string.IsNullOrEmpty(col.Alias) ? string.Empty : " [" + col.Alias + "]"));
            }
            if (q.IncludeTotalRows)
            {
                sb.AppendLine(string.Format("\t,ROW_NUMBER() OVER({0}) [{1}]", sortColumns, "_RowNum"));
            }

            sb.AppendLine("FROM " + Write(q.From));

            if (q.Condition != null)
            {
                sb.AppendLine(string.Format("WHERE {0}", Write(q.Condition)));
            }

            if (!q.IncludeTotalRows)
            {
                sb.AppendLine(sortColumns);
            }
            else
            {
                // set up paging as an outer query 
                sb.Insert(0, @";WITH Paged AS
(
");

                string pagingWhere = "";
                if (q.IncludeTotalRows)
                {
                    pagingWhere = string.Format("WHERE _RowNum BETWEEN {0} AND {1}", q.RecordStart, q.RecordStart + q.RecordCount - 1);
                }
                sb.AppendLine(@"
)
SELECT R.*, C._RowCount
FROM (SELECT * FROM Paged " + pagingWhere + @") AS R	
	FULL JOIN (SELECT MAX(_RowNum) _RowCount FROM Paged) AS C ON
		C._RowCount = -1
ORDER BY _RowCount DESC
");
            }

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

        public override SQTable GetTable(string name)
        {
            string varTable = CreateVariable("Table");
            SQSelectQuery q = new SQSelectQuery();
            q.Columns.AddRange(new List<SQAliasableObject>
            {
                new SQAliasableObject("cols.COLUMN_NAME"),
                new SQAliasableObject("IS_NULLABLE"),
                new SQAliasableObject("DATA_TYPE"),
                new SQAliasableObject("CHARACTER_MAXIMUM_LENGTH"),
                new SQAliasableObject("NUMERIC_PRECISION"),
                new SQAliasableObject("NUMERIC_SCALE"),
                new SQAliasableObject("COLUMNPROPERTY(OBJECT_ID(" + varTable + "), cols.COLUMN_NAME, 'IsIdentity')", "IS_IDENTITY"),
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
                    }
                }
            });
            q.Condition = new SQCondition("cols.TABLE_NAME", SQRelationOperators.Equal, varTable);
            q.Parameters.Add(new SQParameter(varTable, name));

            SQSelectResult selres = Select(q);
            SqlDataReader rdr = (SqlDataReader)selres.Reader;
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
                            Length = rdr.IsDBNull(3) ? 0 : rdr.GetInt32(3),
                            Nullable = rdr.GetString(1) == "YES",
                            Precision = rdr.IsDBNull(4) ? 0 : Convert.ToInt32(rdr.GetByte(4)),
                            Scale = rdr.IsDBNull(5) ? 0 : rdr.GetInt32(5),
                            IsIdentity = rdr.GetInt32(6) > 0,
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

        public override void CreateTable(SQTable table)
        {
            string[] sqls = WriteCreateTable(table).Split(new string[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            ExecuteQuery(sqls[0]);
            ExecuteQuery(sqls[1]);
        }

        public override string WriteCreateTable(SQTable table)
        {
            // create the table
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE dbo." + table.Name);
            sb.AppendLine("(");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                SQColumn col = table.Columns[i];
                sb.AppendLine((i > 0 ? "\t," : "\t") + GetCreateColumnText(col));
            }
            sb.AppendLine(")");

            SQColumn pk = table.GetPrimaryKey();

            if (pk != null)
            {
                sb.AppendLine("ON [PRIMARY]");
                sb.AppendLine("GO");
                sb.AppendLine("ALTER TABLE dbo." + table.Name + " ADD CONSTRAINT");
                sb.AppendLine("PK_" + table.Name + " PRIMARY KEY CLUSTERED (" + pk.Name + ")");
                sb.AppendLine("WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            }

            return sb.ToString();
        }

        public override string WriteRemoveTable(string name)
        {
            return "DROP TABLE dbo." + GetSafeObjectName(name);
        }

        public override string WriteAddColumn(SQColumn col)
        {
            return string.Format("ALTER TABLE dbo.{0} ADD {1}", col.Table.Name, GetCreateColumnText(col));
        }

        public override string WriteAddForeignKey(SQColumn from, SQColumn to)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ALTER TABLE " + GetSafeObjectName(from.Table.Name) + " ADD CONSTRAINT");
            sb.AppendLine(string.Format("[FK_{0}_{1}]", from.Table.Name + "_" + from.Name, to.Table.Name + "_" + to.Name));
            sb.AppendLine(string.Format("FOREIGN KEY ({0})", GetSafeObjectName(from.Name)));
            sb.AppendLine(string.Format("REFERENCES {0} ({1})", GetSafeObjectName(to.Table.Name), GetSafeObjectName(to.Name)));
            sb.AppendLine("ON UPDATE NO ACTION");
            sb.AppendLine("ON DELETE NO ACTION");
            return sb.ToString();
        }

        public override SQColumn GetForeignKeyColumn(SQColumn local)
        {
            throw new NotImplementedException();
        }

        public override string WriteRemoveColumn(SQColumn col)
        {
            return string.Format("ALTER TABLE dbo.{0} DROP COLUMN {1}", col.Table.Name, col.Name);
        }

        public override string WriteRenameColumn(SQColumn col, string oldName)
        {
            return string.Format("EXECUTE sp_rename N'dbo.{0}.{1}', N'{2}', 'COLUMN'", col.Table.Name, oldName, col.Name);
        }

        public override string CreateFunction(SQFunctions func, params string[] parameters)
        {
            switch (func)
            {
                case SQFunctions.LCASE:
                    return base.GetFunctionText("LOWER", parameters);
                case SQFunctions.UCASE:
                    return base.GetFunctionText("UPPER", parameters);
                case SQFunctions.MID:
                    return base.GetFunctionText("SUBSTRING", parameters);
                case SQFunctions.NOW:
                    return "GETDATE()";
                default:
                    return base.CreateFunction(func, parameters);
            }
        }
        #endregion

        public SQLServerAdapter() : base() { }
        public SQLServerAdapter(string connectionString) : base(connectionString) { }

        private string GetCreateColumnText(SQColumn col)
        {
            return string.Format("{0} {1} {2} NULL {3}",
                col.Name, GetSQLNameForType(col), col.Nullable ? "" : "NOT", col.IsIdentity ? "IDENTITY(1, 1)" : "");
        }

        public string GetSQLNameForType(SQColumn col)
        {
            switch (col.DataType)
            {
                case SQDataTypes.Boolean:
                    return "BIT";
                case SQDataTypes.Bytes:
                    return string.Format("VARBINARY({0})", col.Length < 1 || col.Length > 8000 ? "MAX" : col.Length.ToString());
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
                case SQDataTypes.String:
                    return string.Format("VARCHAR({0})", col.Length < 1 || col.Length > 8000 ? "MAX" : col.Length.ToString());
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
                case "VARCHAR":
                case "NVARCHAR":
                case "TEXT":
                case "NTEXT":
                    return SQDataTypes.String;
                case "VARBINARY":
                    return SQDataTypes.Bytes;
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
