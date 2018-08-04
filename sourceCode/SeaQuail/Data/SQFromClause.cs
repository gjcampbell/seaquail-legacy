using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public class SQFromClause
    {
        private SQFromTableList _Tables = null;

        /// <summary>
        /// Root level tables in the from clause
        /// </summary>
        public SQFromTableList Tables 
        {
            get
            {
                return _Tables ?? (_Tables = new SQFromTableList());
            }
            set
            {
                _Tables = value;
            }
        }

        /// <summary>
        /// Look through the items in the from clause and return the one with an alias matching the one passed
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public SQFromItem GetByAlias(string alias)
        {
            SQFromItem res = null;
            foreach (SQFromTable table in Tables)
            {
                if ((res = table.GetByAlias(alias)) != null)
                {
                    break;
                }
            }

            return res;
        }

        public SQFromClause() { }
        public SQFromClause(params string[] tableNames)
        {
            foreach (string tableName in tableNames)
            {
                Tables.Add(new SQFromTable(tableName));
            }
        }
        public SQFromClause(params SQAliasableObject[] tables)
        {
            foreach (SQAliasableObject table in tables)
            {
                Tables.Add(new SQFromTable(table));
            }
        }
        public SQFromClause(params SQFromTable[] tables) 
        {
            Tables.AddRange(tables);
        }
    }
}
