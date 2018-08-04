using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SeaQuail.Data;
using MSBDataSlave.Schema;
using SeaQuail;
using System.Reflection;
using System.Collections;
using SeaQuail.Schema;

namespace MSBDataSlave.Data
{
    public class ORMSelect
    {
        private List<string> _Columns = null;
        private SchemaManager _Schema = null;

        public ORMSelect(SQAdapter adp) 
        {
            _Schema = SchemaManager.GetMgr(adp);
        }

        /// <summary>
        /// Property Chains as Columns to select, all columns of the focal type will be used if empty
        /// </summary>
        public List<string> Columns
        {
            get
            {
                return _Columns ?? (_Columns = new List<string>());
            }
        }

        /// <summary>
        /// The primary table of the select
        /// </summary>
        public Type FocalType { get; set; }

        /// <summary>
        /// The initiator of the conditions of this select statement
        /// </summary>
        public ORMCondBase Condition { get; set; }

        public List<ORMSortProperty> SortProperties { get; set; }

        /// <summary>
        /// For paging, enter the start row
        /// </summary>
        public int RecordStart { get; set; }

        /// <summary>
        /// For paging, enter the number of rows
        /// </summary>
        public int RecordCount { get; set; }

        public int Top { get; set; }

        public ORMSelectQueryBuilder GetQuery()
        {
            return new ORMSelectQueryBuilder(this, _Schema);
        }
    }

    public class ORMSelectQueryBuilder
    {
        private Dictionary<string, string> _JoinAliases = new Dictionary<string, string>();
        private SchemaManager _Schema = null;
        private SQSelectQuery _Query = null;

        public SQSelectQuery Query
        {
            get
            {
                return _Query;
            }
        }

        public ORMSelectQueryBuilder(ORMSelect select, SchemaManager schema)
        {
            _Schema = schema;

            SQSelectQuery q = new SQSelectQuery();
            _Query = q;
            SchemaTable t = _Schema.EnsureSchema(select.FocalType);
            q.From = new SQFromClause();
            q.From.Tables.Add(new SQFromTable(t.Table.Name, select.FocalType.Name));
            if (select.Columns.Count == 0)
            {
                foreach (SQColumn col in t.Table.Columns)
                {
                    q.Columns.Add(new SQAliasableObject(select.FocalType.Name + "." + col.Name));
                }
            }
            else
            {
                foreach (string col in select.Columns)
                {
                    EnsureChainJoins(col, select);
                    q.Columns.Add(new SQAliasableObject(GetPropertyChainColumn(col, select)));
                }
            }

            q.Condition = GetCondition(select.Condition, select);
            
            q.Top = select.Top;
            if (select.SortProperties != null)
            {
                foreach (ORMSortProperty sp in select.SortProperties)
                {
                    EnsureChainJoins(sp.Property, select);
                    q.SortColumns.Add(new SQSortColumn()
                    {
                        Column = GetPropertyChainColumn(sp.Property, select),
                        Direction = !sp.Ascending.HasValue ?
                            SortOrder.Unspecified : sp.Ascending.Value ?
                            SortOrder.Ascending : SortOrder.Descending
                    });
                }
            }

            q.RecordCount = select.RecordCount;
            q.RecordStart = select.RecordStart;
        }

        private SQConditionBase GetCondition(ORMCondBase cond, ORMSelect select)
        {
            if (cond != null)
            {
                SQConditionBase res = null;
                if (cond is ORMCond)
                {
                    ORMCond cnd = (ORMCond)cond;
                    string operandA = GetPropertyChainColumn(cnd.PropertyChain, select);
                    string operandB = null;
                    if (cnd.Value is ORMPropertyChain)
                    {
                        operandB = GetPropertyChainColumn(((ORMPropertyChain)cnd.Value).Chain, select);
                    }
                    else if (cnd.Value == ORMCond.NullOperand)
                    {
                        operandB = "NULL";
                    }
                    else
                    {
                        operandB = GetNextParam();
                        Query.Parameters.Add(new SQParameter((string)operandB, cnd.Value));
                    }
                    res = new SQCondition(operandA, cnd.Operator, operandB, cnd.InvertMeaning);                    
                }
                else if (cond is ORMCondSQL)
                {
                    ORMCondSQL cnd = (ORMCondSQL)cond;
                    res = cnd.Cond;

                    foreach (SQParameter prm in cnd.Parameters)
                    {
                        Query.Parameters.Add(prm);
                    }
                }
                else if (cond is ORMCondGroup)
                {
                    ORMCondGroup grp = (ORMCondGroup)cond;
                    res = new SQConditionGroup(grp.InvertMeaning, GetCondition(grp.InnerCondition, select));
                }

                if (cond.NextCondition != null)
                {
                    if (cond.Connective == SQLogicOperators.AND)
                    {
                        res.And(GetCondition(cond.NextCondition, select));
                    }
                    else
                    {
                        res.Or(GetCondition(cond.NextCondition, select));
                    }
                }

                return res;
            }

            return null;
        }

        private string GetPropertyChainColumn(string chain, ORMSelect q)
        {
            EnsureChainJoins(chain, q);

            List<PropertyInfo> pis = new ORMPropertyChain(chain).GetChain(q.FocalType);

            PropertyInfo endProp = pis[pis.Count - 1];
            SchemaTable t = _Schema.EnsureSchema(endProp.ReflectedType);
            string colName = t.GetColumnByName(endProp.Name).Column.Name;
            return GetJoinAlias(chain, q) + "." + colName;
        }

        private void EnsureChainJoins(string propertyChain, ORMSelect q)
        {
            EnsureChainJoins(new ORMPropertyChain(propertyChain).GetChain(q.FocalType), q);
        }
        private void EnsureChainJoins(List<PropertyInfo> pis, ORMSelect q)
        {
            SQFromTable t = (SQFromTable)Query.From.GetByAlias(q.FocalType.Name);

            if (pis.Count > 1)
            {
                string previousAlias = q.FocalType.Name;
                SchemaTable previousTable = _Schema.GetSchemaForType(q.FocalType);
                string currChain = pis[0].Name;
                for (int i = 1; i < pis.Count; i++)
                {
                    PropertyInfo prevPi = pis[i - 1];
                    PropertyInfo pi = pis[i];
                    // TODO: use foreign key hints somehow
                    currChain += "." + pi.Name;
                    string joinAlias = GetJoinAlias(currChain, q);
                    SchemaTable nextTable = _Schema.GetSchemaForType(pi.ReflectedType);
                    if (Query.From.GetByAlias(joinAlias) == null)
                    {
                        SQJoin newJoin = new SQJoin()
                        {
                            Table = new SQAliasableObject(nextTable.Table.Name, joinAlias),
                            JoinType = SQJoinTypes.Left
                        };
                        
                        t.Append(newJoin);

                        if (typeof(IEnumerable).IsAssignableFrom(prevPi.PropertyType) && pi.ReflectedType != prevPi.PropertyType)
                        {
                            // join to a child
                            ORMChildJoinHint hint = prevPi.GetJoinHint();
                            if (hint != null)
                            {
                                newJoin.Predicate = new SQCondition(previousAlias + "." + previousTable.Table.GetPrimaryKey().Name, 
                                    SQRelationOperators.Equal, 
                                    joinAlias + "." + nextTable.GetColumnByName(nextTable.GetFKeyByName(hint.FKeyPropertyName).Storage).Column.Name);
                            }
                        }
                        else
                        {
                            // join to a parent
                            newJoin.Predicate = new SQCondition(previousAlias + "." + previousTable.GetColumnByName(previousTable.GetFKeyByName(prevPi.Name).Storage).Column.Name, SQRelationOperators.Equal, joinAlias + "." + nextTable.Table.GetPrimaryKey().Name);
                        }
                    }

                    previousAlias = joinAlias;
                    previousTable = nextTable;
                }
            }
        }

        private string GetJoinAlias(string propertyChain, ORMSelect q)
        {
            string res = q.FocalType.Name;
            List<string> parts = new List<string>(propertyChain.Split('.'));
            parts.RemoveAt(parts.Count - 1);
            string joinName = string.Join(".", parts.ToArray());
            if (!_JoinAliases.ContainsKey(joinName))
            {
                _JoinAliases.Add(joinName, res + (parts.Count > 0 ? "_" + string.Join("_", parts.ToArray()) : ""));
            }

            return _JoinAliases[joinName];
        }

        private string GetNextParam()
        {
            return _Schema.Adp.CreateVariable(string.Format("Var{0}", Query.Parameters.Count));
        }
    }

    public class ORMSortProperty
    {
        /// <summary>
        /// The name of a property or property chain on which to sort
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// Set true for ascending, false for descending, null for unspecified
        /// </summary>
        public bool? Ascending { get; set; }

        public ORMSortProperty() { }
        public ORMSortProperty(string property)
        {
            Property = property;
        }
        public ORMSortProperty(string property, bool ascending)
            :this(property)
        {
            Ascending = ascending;
        }
    }
}
