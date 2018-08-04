using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Schema;

namespace SeaQuail.SchemaQuery
{
    public class SQRemoveColumn : SQSchemaQueryBase
    {
        /// <summary>
        /// Table from which the column will be removed
        /// </summary>
        public SQTable Table { get; set; }
        /// <summary>
        /// Name of column to remove
        /// </summary>
        public string ColumnName { get; set; }
    }
}
