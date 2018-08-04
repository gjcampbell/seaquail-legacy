using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    /// <summary>
    /// Types of joins, not supporting full outer because it is
    /// not supported by MySQL
    /// </summary>
    public enum SQJoinTypes { Left, Right, Inner }

    // TODO:
    // -- Support join hints a la SQL Server's HASH, MERGE? 
    /// <summary>
    /// Represents a join statement and its conditions in an SQL query
    /// </summary>
    public class SQJoin : SQFromItem
    {
        /// <summary>
        /// The join predicate, or the conditions of the join
        /// </summary>
        public SQConditionBase Predicate { get; set; }

        /// <summary>
        /// Type of join to execute, default is Left
        /// </summary>
        public SQJoinTypes JoinType { get; set; }

        public SQJoin() { }
        public SQJoin(string tableName) : base(tableName) { }
        public SQJoin(string tableName, string alias) : base(tableName, alias) { }
    }
}
