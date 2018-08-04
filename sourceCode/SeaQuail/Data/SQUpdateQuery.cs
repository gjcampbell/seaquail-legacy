using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    /// <summary>
    /// SQL UPDATE query
    /// </summary>
    public class SQUpdateQuery : SQSetQuery
    {
        /// <summary>
        /// Table joined for use in searching for records to update or for the source of data being set
        /// </summary>
        public SQJoin Join { get; set; }

        /// <summary>
        /// The table on which the updates should be executed
        /// </summary>
        public SQAliasableObject UpdateTable { get; set; }

        /// <summary>
        /// Condition of update
        /// </summary>
        public SQConditionBase Condition { get; set; }

        public override string Write(ISQLWriter adp)
        {
            return adp.Write(this);
        }

        public void Execute(IDBExecutor adp)
        {
            adp.Update(this);
        }
    }
}
