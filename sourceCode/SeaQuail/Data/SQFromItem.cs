using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public abstract class SQFromItem
    {
        /// <summary>
        /// Table to add to a from clause
        /// </summary>
        public SQAliasableObject Table { get; set; }

        /// <summary>
        /// Join to execute on table
        /// </summary>
        public SQJoin Join { get; set; }

        /// <summary>
        /// Add a from clause item to the end of the join chain
        /// </summary>
        /// <param name="nextJoin"></param>
        /// <returns></returns>
        public SQFromItem Append(SQJoin nextJoin)
        {
            if (Join == null)
            {
                Join = nextJoin;
            }
            else
            {
                Join.Append(nextJoin);
            }

            return this;
        }

        /// <summary>
        /// Look through the items in the from clause and return the one with an alias matching the one passed
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public SQFromItem GetByAlias(string alias)
        {
            SQFromItem j = this;
            do
            {
                if (j.Table.Alias == alias)
                {
                    return j;
                }
            }
            while ((j = j.Join) != null);

            return null;
        }

        /// <summary>
        /// Insert a from clause item after this one and before the next
        /// </summary>
        /// <param name="nextJoin"></param>
        /// <returns></returns>
        public SQFromItem Insert(SQJoin join)
        {
            if (Join != null)
            {
                join.Join = Join;
            }
            Join = join;

            return this;
        }

        public SQFromItem() { }
        public SQFromItem(string tableName) : this(tableName, null) { }
        public SQFromItem(string tableName, string alias)
        {
            Table = new SQAliasableObject(tableName, alias);
        }
        public SQFromItem(SQAliasableObject table)
        {
            Table = table;
        }
    }
}
