using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    /// <summary>
    /// Standard SQL DELETE query
    /// </summary>
    public class SQDeleteQuery : SQQueryBase
    {
        private List<SQAliasableObject> _Columns = new List<SQAliasableObject>();

        /// <summary>
        /// Table joined for use in searching for records to delete
        /// </summary>
        public SQJoin Join { get; set; }

        /// <summary>
        /// The table on which the deletion should be executed
        /// </summary>
        public SQAliasableObject DeleteTable { get; set; }

        /// <summary>
        /// The initiator of the conditions of this statement
        /// </summary>
        public SQConditionBase Condition { get; set; }

        public override string Write(ISQLWriter wr)
        {
            return wr.Write(this);
        }

        public void Execute(IDBExecutor adp)
        {
            adp.Delete(this);
        }
    }
}
