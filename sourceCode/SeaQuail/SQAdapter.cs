using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using SeaQuail.Data;
using System.Data;
using SeaQuail.Schema;
using SeaQuail.SchemaQuery;

namespace SeaQuail
{
    /// <summary>
    /// Inherit to implement SQL writing for an RDBMS. 
    /// Provides some overloads and some methods for writing 
    /// SQL for which the syntax is universal or close to it.
    /// </summary>
    public abstract class SQAdapter : IDBExecutor, ISQLWriter
    {
        public static SQAdapter Instance { get; private set; }
        public static SQAdapter SecondaryInstance { get; private set; }

        public static void SetAdapter(SQAdapter adp)
        {
            Instance = adp;
        }

        public static void SetSecondaryAdapter(SQAdapter adp)
        {
            SecondaryInstance = adp;
        }

        public string ConnectionString { get; set; }

        private DbTransaction _Transaction = null;

        protected internal DbTransaction Transaction
        {
            get
            {
                return _Transaction;
            }
            set
            {
                _Transaction = value;
            }
        }

        protected virtual SQTransaction OpenTransaction(SQAdapter adp, DbTransaction trans)
        {
            return new SQTransaction(adp, trans);
        }
        public virtual SQTransaction OpenTransaction()
        {
            SQAdapter adp = (SQAdapter)Activator.CreateInstance(GetType());
            adp._Connection = CreateConnection();
            adp._Connection.Open();
            adp.Transaction = adp._Connection.BeginTransaction();
            SQTransaction trans = new SQTransaction(adp, adp.Transaction);
            trans.OnClose += new SQTransaction.CloseHandler(adp.Transaction_Close);
            return trans;
        }

        protected virtual void Transaction_Close()
        {
            if (_Connection != null && _Connection.State == ConnectionState.Open)
            {
                _Connection.Close();
            }
        }

        protected DbConnection _Connection = null;

        protected T GetConnection<T>() where T : DbConnection
        {
            return (T)GetConnection();
        }
        protected DbConnection GetConnection()
        {
            if (_Connection != null)
            {
                return _Connection;
            }
            return CreateConnection();
        }

        protected abstract DbConnection CreateConnection();

        #region Data
        public virtual void ExecuteQuery(string query)
        {
            ExecuteQuery(query, new List<SQParameter>());
        }
        public abstract void ExecuteQuery(string query, List<SQParameter> parameters);

        /// <summary>
        /// Executy the passed SQL query.
        /// </summary>
        /// <param name="query"></param>
        public virtual SQSelectResult Select(string query)
        {
            return Select(query, new List<SQParameter>());
        }
        /// <summary>
        /// Executy the passed SQL query with parameters.
        /// </summary>
        /// <param name="query"></param>
        public abstract SQSelectResult Select(string query, List<SQParameter> parameters);
        /// <summary>
        /// Run the select query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual SQSelectResult Select(SQSelectQuery query)
        {
            return Select(Write(query), query.Parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual SQSelectResult Insert(SQInsertQuery query)
        {
            return Select(Write(query), query.Parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual void Insert(SQInsertFromQuery query)
        {
            ExecuteQuery(Write(query), query.Parameters);
        }

        public virtual void Update(SQUpdateQuery query)
        {
            ExecuteQuery(Write(query), query.Parameters);
        }

        public virtual void Delete(SQDeleteQuery query)
        {
            ExecuteQuery(Write(query), query.Parameters);
        }

        public abstract string Write(SQSelectQuery query);

        public virtual string Write(SQInsertQuery q)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO " + GetSafeObjectName(q.Table.Actual.Write(this)));
            sb.AppendLine("(");
            for (int i = 0; i < q.SetPairs.Count; i++)
            {
                SQSetQueryPair pair = q.SetPairs[i];
                sb.AppendLine((i == 0 ? "\t" : "\t,") + GetSafeObjectName(pair.Left));
            }
            sb.AppendLine(")");
            sb.AppendLine("VALUES");
            sb.AppendLine("(");
            for (int i = 0; i < q.SetPairs.Count; i++)
            {
                SQSetQueryPair pair = q.SetPairs[i];
                sb.AppendLine((i == 0 ? "\t" : "\t,") + pair.Right);
            }
            sb.AppendLine(")");

            return sb.ToString();
        }

        public virtual string Write(SQInsertFromQuery q)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO " + GetSafeObjectName(q.Table.Actual.Write(this)));
            sb.AppendLine("(");
            for (int i = 0; i < q.SetPairs.Count; i++)
            {
                SQSetQueryPair pair = q.SetPairs[i];
                sb.AppendLine((i == 0 ? "\t" : "\t,") + GetSafeObjectName(pair.Left));
            }
            sb.AppendLine(")");
            sb.AppendLine("SELECT");
            for (int i = 0; i < q.SetPairs.Count; i++)
            {
                SQSetQueryPair pair = q.SetPairs[i];
                sb.AppendLine((i == 0 ? "\t" : "\t,") + pair.Right);
            }

            sb.AppendLine("FROM " + Write(q.From));
            if (q.Condition != null)
            {
                sb.AppendLine("WHERE " + Write(q.Condition));
            }

            return sb.ToString();
        }

        public virtual string Write(SQUpdateQuery q)
        {
            StringBuilder sb = new StringBuilder();

            string fromClause = "";
            if (q.Join != null)
            {
                fromClause = string.Format("\r\nFROM {0} {1}",
                    GetSafeObjectName(q.UpdateTable.Actual.Write(this)) + (string.IsNullOrEmpty(q.UpdateTable.Alias) ? "" : " AS " + q.UpdateTable.Alias),
                    Write(q.Join));
            }

            // use the alias if there should be a from clause and the 
            // table's alias is set
            string updateObject = !string.IsNullOrEmpty(fromClause) && !string.IsNullOrEmpty(q.UpdateTable.Alias) ? q.UpdateTable.Alias : GetSafeObjectName(q.UpdateTable.Actual.Write(this));
            sb.AppendLine(string.Format("UPDATE {0} SET", updateObject));
            for (int i = 0; i < q.SetPairs.Count; i++)
            {
                SQSetQueryPair pair = q.SetPairs[i];
                sb.AppendLine((i == 0 ? "\t" : "\t,") + string.Format("{0} = {1}", GetSafeObjectName(pair.Left), pair.Right));
            }

            if (!string.IsNullOrEmpty(fromClause))
            {
                sb.AppendLine(fromClause);
            }

            if (q.Condition != null)
            {
                sb.AppendLine("WHERE " + Write(q.Condition));
            }

            return sb.ToString();
        }

        public virtual string Write(SQDeleteQuery q)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DELETE ");

            if (q.Join == null)
            {
                sb.Append(" FROM " + GetSafeObjectName(q.DeleteTable.Actual.Write(this)));
            }
            else
            {
                sb.AppendLine(!string.IsNullOrEmpty(q.DeleteTable.Alias) ? q.DeleteTable.Alias : GetSafeObjectName(q.DeleteTable.Actual.Write(this)));
                sb.AppendLine(string.Format("FROM {0} {1}",
                    GetSafeObjectName(q.DeleteTable.Actual.Write(this)) + (string.IsNullOrEmpty(q.DeleteTable.Alias) ? "" : " AS " + q.DeleteTable.Alias),
                    Write(q.Join)));
            }

            if (q.Condition != null)
            {
                sb.AppendLine("WHERE " + Write(q.Condition));
            }

            return sb.ToString();
        }

        public abstract string GetSafeObjectName(string name);

        public abstract string CreateVariable(string name);

        public virtual string CreateAggregate(SQAggregates agg, params string[] parameters)
        {
            return GetFunctionText(agg.ToString(), parameters);
        }

        public virtual string CreateFunction(SQFunctions func, params string[] parameters)
        {
            return GetFunctionText(func.ToString(), parameters);
        }

        /// <summary>
        /// When paging settings are set on a select query, pass the result to get the total record count. Finish using the reader before passing it here, because the reader may be useless afterward.
        /// </summary>
        /// <param name="result">the SelectResult returned by a Select(SelectQuery) call</param>
        /// <returns></returns>
        public abstract Int64 GetTotalRecordCount(SQSelectResult result);
        #endregion

        #region Schema
        public abstract SQTable GetTable(string name);

        public virtual void CreateTable(SQTable table)
        {
            ExecuteQuery(WriteCreateTable(table));
        }

        public abstract string WriteCreateTable(SQTable table);

        public virtual void RemoveTable(string name)
        {
            ExecuteQuery(WriteRemoveTable(name));
        }

        public abstract string WriteRemoveTable(string name);

        public virtual void AddColumn(SQColumn col)
        {
            ExecuteQuery(WriteAddColumn(col));
        }

        public abstract string WriteAddColumn(SQColumn col);

        // TODO: GetIndex, AddIndex

        public virtual void AddForeignKey(SQColumn from, SQColumn to)
        {
            ExecuteQuery(WriteAddForeignKey(from, to));
        }

        public abstract string WriteAddForeignKey(SQColumn from, SQColumn to);

        public abstract SQColumn GetForeignKeyColumn(SQColumn local);

        public virtual void RemoveColumn(SQColumn col)
        {
            ExecuteQuery(WriteRemoveColumn(col));
        }

        public abstract string WriteRemoveColumn(SQColumn col);

        public virtual void RenameColumn(SQColumn col, string oldName)
        {
            ExecuteQuery(WriteRenameColumn(col, oldName));
        }

        public abstract string WriteRenameColumn(SQColumn col, string oldName);
        #endregion

        public SQAdapter() { }
        public SQAdapter(string connectionString) 
            : this()
        {
            ConnectionString = connectionString;
        }

        protected virtual string GetOperatorText(SQRelationOperators op)
        {
            switch (op)
            {
                case SQRelationOperators.Equal:
                    return "=";
                case SQRelationOperators.After:
                case SQRelationOperators.GreaterThan:
                    return ">";
                case SQRelationOperators.GreaterThanOrEqual:
                    return ">=";
                case SQRelationOperators.Before:
                case SQRelationOperators.LessThan:
                    return "<";
                case SQRelationOperators.LessThanOrEqual:
                    return "<=";
                case SQRelationOperators.Contains:
                case SQRelationOperators.Like:
                case SQRelationOperators.EndsWith:
                case SQRelationOperators.StartsWith:
                    return "LIKE";
                case SQRelationOperators.In:
                    return "IN";
                case SQRelationOperators.Is:
                    return "IS";
                default:
                    return "=";
            }
        }

        public virtual string Write(IWriteSQL sqObject)
        {
            return sqObject.Write(this);
        }

        public virtual string Write(SQConditionBase condition)
        {
            string res = string.Empty;

            if (condition is SQCondition)
            {
                SQCondition con = (SQCondition)condition;
                res = string.Format("{0} {1} {2}", Write(con.OperandA), GetOperatorText(con.Operator), Write(con.OperandB));
                res = string.Format("{0}({1})", condition.InvertMeaning ? "NOT" : "", res);
            }
            else if (condition is SQConditionGroup)
            {
                SQConditionGroup con = (SQConditionGroup)condition;
                res = (condition.InvertMeaning ? "NOT" : "") + "(" + Write(con.InnerCondition) + ")";
            }

            if (condition.NextCondition != null)
            {
                res += string.Format(" {0} {1}", condition.Connective, Write(condition.NextCondition));
            }
            return res;
        }

        public virtual string Write(SQCustomSQL sql)
        {
            return sql.SQL;
        }

        public virtual string Write(SQParameter param)
        {
            return "@" + param.Name;
        }

        public virtual string Write(SQFromTable fromTable)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("\r\n\t{0}{1}",
                fromTable.Table.Actual.Write(this),
                string.IsNullOrEmpty(fromTable.Table.Alias) ? "" : " AS " + fromTable.Table.Alias));

            sb.AppendLine(Write(fromTable.Join));

            return sb.ToString();
        }

        public virtual string Write(SQFromClause from)
        {
            List<string> fromTables = new List<string>();
            foreach (SQFromTable table in from.Tables)
            {
                fromTables.Add(Write(table));
            }

            return string.Join(",", fromTables.ToArray());
        }

        public virtual string Write(SQJoin join)
        {
            StringBuilder sb = new StringBuilder();

            while (join != null)
            {
                sb.AppendLine(string.Format("\r\n\t{0}JOIN {1}{2} ON\r\n",
                    join.JoinType != SQJoinTypes.Inner ? join.JoinType.ToString().ToUpper() + " " : "",
                    Write(join.Table.Actual),
                    string.IsNullOrEmpty(join.Table.Alias) ? "" : " AS " + join.Table.Alias));

                sb.AppendLine(Write(join.Predicate));

                join = join.Join;
            }

            return sb.ToString();
        }

        protected virtual string GetFunctionText(string func, params string[] parameters)
        {
            return string.Format("{0}({1})", func, string.Join(",", parameters));
        }

        protected void SetStoredTotalRecordCount(SQSelectResult res, Int64 count)
        {
            res.TotalRecordCount = count;
        }

        protected Int64 GetStoredTotalRecordCount(SQSelectResult res)
        {
            return res.TotalRecordCount;
        }

        public abstract string Write(SQAddColumn addColumn);

        public abstract string Write(SQAddForeignKey addKey);

        public abstract string Write(SQAddIndex addIndex);

        public abstract string Write(SQCreateTable createTable);

        public abstract string Write(SQInsertColumn insertColumn);

        public abstract string Write(SQRemoveColumn removeColumn);

        public abstract string Write(SQRemoveForeignKey removeKey);

        public abstract string Write(SQRemoveIndex removeIndex);

        public abstract string Write(SQRemoveTable removeTable);

        public abstract string Write(SQRenameColumn renameColumn);

        public abstract string Write(SQRenameTable renameTable);
    }
}
