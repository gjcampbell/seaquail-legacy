using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail;
using System.ComponentModel;
using SeaQuail.Schema;

namespace MSBDataSlave.Schema
{
    public enum SerializationTypes { None, SerializeJSON, SerializeXML, SerializeBinary }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SchemaHintColumn : Attribute
    {
        public string ColumnName { get; set; }
        public string PropertyName { get; set; }
        internal Int64? _Length { get; set; }
        internal int? _Scale { get; set; }
        internal int? _Precision { get; set; }
        internal bool? _IsPrimary { get; set; }
        internal bool? _Nullable { get; set; }
        internal bool? _IsIdentity { get; set; }
        internal bool? _Ignore { get; set; }
        internal SQDataTypes? _DataType { get; set; }
        internal SerializationTypes? _Serialization { get; set; }

        public Int64 Length { get { return _Length.GetValueOrDefault(0); } set{_Length = value;} }
        public int Scale { get { return _Scale.GetValueOrDefault(0); } set { _Scale = value; } }
        public int Precision { get { return _Precision.GetValueOrDefault(0); } set { _Precision = value; } }
        public bool IsPrimary { get { return _IsPrimary.GetValueOrDefault(false); } set { _IsPrimary = value; } }
        public bool Nullable { get { return _Nullable.GetValueOrDefault(DataType == SQDataTypes.String); } set { _Nullable = value; } }
        public bool IsIdentity { get { return _IsIdentity.GetValueOrDefault(false); } set { _IsIdentity = value; } }
        public bool Ignore { get { return _Ignore.GetValueOrDefault(false); } set { _Ignore = value; } }
        public SQDataTypes DataType { get { return _DataType.GetValueOrDefault(SQDataTypes.String); } set { _DataType = value; } }
        public SerializationTypes Serialization { get { return _Serialization.GetValueOrDefault(SerializationTypes.None); } set { _Serialization = value; } }
    }
}
