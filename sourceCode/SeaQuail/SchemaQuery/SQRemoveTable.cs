using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Schema;

namespace SeaQuail.SchemaQuery
{
    public class SQRemoveTable : SQSchemaQueryBase
    {
        public SQTable Table { get; set; }
    }
}
