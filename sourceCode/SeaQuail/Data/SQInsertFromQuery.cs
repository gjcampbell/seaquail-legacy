using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    /// <summary>
    /// Insert with a select statement e.g. INSERT INTO T2 (c1, c2) SELECT c1, c2 FROM T2
    /// </summary>
    public class SQInsertFromQuery : SQSetQuery
    {
        /// <summary>
        /// Table into which inserting will be done
        /// </summary>
        public SQAliasableObject Table { get; set; }

        /// <summary>
        /// The from clause of the select statement
        /// </summary>
        public SQFromClause From { get; set; }

        /// <summary>
        /// The where clause of the select
        /// </summary>
        public SQConditionBase Condition { get; set; }

        public SQInsertFromQuery() { }
        public SQInsertFromQuery(SQAliasableObject intoTable)
            : this() 
        {
            Table = intoTable;
        }
        public SQInsertFromQuery(SQAliasableObject intoTable, params SQSetQueryPair[] pairs)
            : this(intoTable)
        {
            SetPairs = new List<SQSetQueryPair>(pairs);
        }
        public SQInsertFromQuery(SQAliasableObject intoTable, IList<string> intoColumns, IList<string> selectColumns)
            : this(intoTable)
        {
            for (int i = 0; i < intoColumns.Count; i++)
            {
                SetPairs.Add(new SQSetQueryPair(intoColumns[i], selectColumns[i]));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adp"></param>
        /// <returns></returns>
        public override string Write(ISQLWriter adp)
        {
            return adp.Write(this);
        }

        public void Execute(IDBExecutor dbe)
        {
            dbe.Insert(this);
        }
    }
}
