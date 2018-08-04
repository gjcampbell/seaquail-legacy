using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail;
using System.Reflection;
using System.IO;
using SeaQuail.Schema;

namespace MSBDataSlave.Schema
{
    public class SchemaColumn
    {
        public SchemaTable Table { get; private set; }
        public List<SchemaHintColumn> Hints { get; private set; }
        public PropertyInfo Property { get; private set; }
        public SQColumn Column { get; private set; }
        public SerializationTypes Serialization { get; private set; }

        public SchemaColumn(SchemaTable table, List<SchemaHintColumn> hints, PropertyInfo pi) 
        {
            Table = table;
            Hints = hints;
            Property = pi;

            SQDataTypes? datatype = GetDataTypeForType(pi.PropertyType);

            SQColumn res = new SQColumn() { Name = pi.Name };
            res.Table = table.Table;
            if (datatype.HasValue)
            {
                res.DataType = datatype.Value;
            }

            bool ignore = false;
            foreach (SchemaHintColumn colHint in hints)
            {
                if (colHint._Ignore.HasValue)
                {
                    ignore = colHint._Ignore.Value;
                }
                if (colHint.ColumnName != null)
                {
                    res.Name = colHint.ColumnName;
                }
                if (colHint._IsIdentity.HasValue)
                {
                    res.IsIdentity = colHint._IsIdentity.Value;
                }
                if (colHint._IsPrimary.HasValue)
                {
                    res.IsPrimary = colHint._IsPrimary.Value;
                }
                if (colHint._Length.HasValue)
                {
                    res.Length = colHint._Length.Value;
                }
                if (colHint._Precision.HasValue)
                {
                    res.Precision = colHint._Precision.Value;
                }
                if (colHint._Scale.HasValue)
                {
                    res.Scale = colHint._Scale.Value;
                }
                if (colHint._DataType.HasValue)
                {
                    res.DataType = colHint._DataType.Value;
                }
                if (colHint._Serialization.HasValue)
                {
                    Serialization = colHint._Serialization.Value;
                    if (Serialization == SerializationTypes.SerializeBinary)
                    {
                        res.DataType = SQDataTypes.Bytes;                        
                    }
                    else if (Serialization == SerializationTypes.SerializeJSON
                        || Serialization == SerializationTypes.SerializeXML)
                    {
                        res.DataType = SQDataTypes.String;
                    }
                }
                if (colHint._Nullable.HasValue)
                {
                    res.Nullable = colHint._Nullable.Value;
                }
            }

            if (!datatype.HasValue || ignore)
            {
                return;
            }

            Column = res;
        }

        public SQDataTypes? GetDataTypeForType(Type type)
        {
            if (type == typeof(string))
            {
                return SQDataTypes.String;
            }
            else if (type == typeof(Int16))
            {
                return SQDataTypes.Int16;
            }
            else if (type == typeof(int) || type == typeof(Byte))
            {
                return SQDataTypes.Int32;
            }
            else if (type == typeof(Int64))
            {
                return SQDataTypes.Int64;
            }
            else if (type == typeof(bool))
            {
                return SQDataTypes.Boolean;
            }
            else if (type == typeof(decimal))
            {
                return SQDataTypes.Decimal;
            }
            else if (type == typeof(float))
            {
                return SQDataTypes.Float;
            }
            else if (type == typeof(DateTime))
            {
                return SQDataTypes.DateTime;
            }
            else if (type == typeof(byte[]) || typeof(Stream).IsAssignableFrom(type))
            {
                return SQDataTypes.Bytes;
            }
            else if (type.IsEnum)
            {
                return SQDataTypes.Int64;
            }

            return null;
        }
    }
}
