using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Schema;

namespace SeaQuail_SQLite.Schema
{
    [Obsolete("The base SQColumn now has a foreign key property. ")]
    public class SQColumn_SQLite : SQColumn
    {
        public SQColumn ForeignKey { get; set; }
    }
}
