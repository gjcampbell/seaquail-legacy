using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Schema;

namespace SeaQuail.SchemaQuery
{
    public class SQInsertColumn : SQSchemaQueryBase
    {
        /// <summary>
        /// Column to be added to the table
        /// </summary>
        public SQColumn Column { get; set; }
        /// <summary>
        /// Table to which the column will be added
        /// </summary>
        public SQTable Table { get; set; }
        /// <summary>
        /// Zero-based position at which the column should be inserted
        /// </summary>
        public int Position { get; set; }
    }
}
