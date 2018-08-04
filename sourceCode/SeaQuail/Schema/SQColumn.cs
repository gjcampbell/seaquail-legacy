using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Schema
{
    public enum SQDataTypes { Boolean, Int16, Int32, Int64, Decimal, DateTime, String, Bytes, Float }

    /// <summary>
    /// Represents a database table column, instantiate, and add it to a
    /// SQTable when creating a table, pass an instance with the Table 
    /// property set when adding or remove a column or foreign key.
    /// TODO: 
    /// -- Settings for identity, start number and increment amount
    /// </summary>
    public class SQColumn
    {
        /// <summary>
        /// Name of column
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Table to which this column belongs. 
        /// </summary>
        public SQTable Table { get; set; }
        public SQDataTypes DataType { get; set; }
        /// <summary>
        /// Length of the data, applicable in most RDBMS for string and 
        /// byte types. Use length of zero for length that is RDBMS
        /// default
        /// </summary>
        public Int64 Length { get; set; }
        public int Scale { get; set; }
        public int Precision { get; set; }
        /// <summary>
        /// True if this is a primary key column
        /// </summary>
        public bool IsPrimary { get; set; }
        /// <summary>
        /// True if the column is nullable
        /// </summary>
        public bool Nullable { get; set; }
        /// <summary>
        /// True if the column is an identity column, or has auto 
        /// increment settings
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// Default value of the column
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// Foreign key to another column
        /// </summary>
        public SQForeignKey ForeignKey { get; set; }
    }
}
