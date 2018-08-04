using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Schema;

namespace SeaQuail.SchemaQuery
{
    public class SQRenameColumn : SQSchemaQueryBase
    {
        public SQTable Table { get; set; }
        public string CurrentColumnName { get; set; }
        public string NewColumnName { get; set; }
    }
}
