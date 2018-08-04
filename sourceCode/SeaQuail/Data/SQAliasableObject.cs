using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public class SQAliasableObject
    {
        /// <summary>
        /// The name of an actual SQL object of any aliasable type
        /// such as a table or column
        /// </summary>
        public IWriteSQL Actual { get; set; }
        /// <summary>
        /// The alias to be used for an SQL object
        /// </summary>
        public string Alias { get; set; }

        public SQAliasableObject() { }
        public SQAliasableObject(string actualObject)
            : this()
        {
            Actual = new SQCustomSQL(actualObject);
        }
        public SQAliasableObject(string actualObject, string alias)
            : this(actualObject)
        {
            Alias = alias;
        }
        public SQAliasableObject(IWriteSQL actualObject)
            : this()
        {
            Actual = actualObject;
        }
        public SQAliasableObject(IWriteSQL actualObject, string alias)
            : this(actualObject)
        {
            Alias = alias;
        }
    }
}
