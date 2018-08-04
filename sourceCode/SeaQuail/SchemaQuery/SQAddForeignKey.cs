using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Schema;

namespace SeaQuail.SchemaQuery
{
    public class SQAddForeignKey : SQSchemaQueryBase
    {
        public SQTable FromTable { get; set; }
        public string FromColumnName { get; set; }
        public SQTable ToTable { get; set; }
        public string ToColumnName { get; set; }
        public string ForeignKeyName { get; set; }
    }
}
