using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public class SQFromTable : SQFromItem
    {
        public SQFromTable() { }
        public SQFromTable(string tableName) : base(tableName, null) { }
        public SQFromTable(string tableName, string alias) : base(tableName, alias) { }
        public SQFromTable(SQAliasableObject table) : base(table) { }
    }
}
