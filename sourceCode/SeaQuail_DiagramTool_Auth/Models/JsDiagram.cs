using SeaQuail.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeaQuail_DiagramTool.Models
{
    public class JSDiagram
    {
        public string Name { get; set; }
        public Int64 ID { get; set; }
        public List<JSTable> Tables { get; set; }
        public List<JSFKey> FKeys { get; set; }

        internal void Relate()
        {
            Tables.ForEach(x => x.Relate(this));
            FKeys.ForEach(x => x.Relate(this));
        }
    }

    public class JSTable
    {
        private SQTable _Table = null;

        public string Name { get; set; }
        public string GUID { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public List<JSColumn> Columns { get; set; }

        internal JSDiagram Diagram { get; set; }

        internal void Relate(JSDiagram dg)
        {
            Diagram = dg;
            Columns.ForEach(x => x.Table = this);
        }

        internal SQTable GetTable()
        {
            if (_Table == null)
            {
                _Table = GetTable(true);
            }

            return _Table;
        }
        internal SQTable GetTable(bool refresh)
        {
            if (!refresh)
            {
                GetTable();
            }

            _Table = new SQTable();
            _Table.Name = Name;
            foreach (JSColumn col in Columns)
            {
                _Table.Columns.Add(col.GetColumn());
            }

            return _Table;
        }
    }

    public class JSColumn
    {
        private SQColumn _Column = null;

        public string Name { get; set; }
        public JSDataType DataType { get; set; }
        public string GUID { get; set; }
        public string Length { get; set; }
        public bool Nullable { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsIdentity { get; set; }
        public string Precision { get; set; }
        public string Scale { get; set; }
        public string DefaultValue { get; set; }

        internal JSTable Table { get; set; }

        internal SQColumn GetColumn()
        {
            if (_Column == null)
            {
                _Column = GetColumn(true);
            }

            return _Column;
        }
        internal SQColumn GetColumn(bool refresh)
        {
            if (!refresh)
            {
                return GetColumn();
            }

            _Column = new SQColumn();
            _Column.Name = Name;
            _Column.DataType = (SQDataTypes)Enum.Parse(typeof(SQDataTypes),
                DataType.Name == "BigInt" ? SQDataTypes.Int64.ToString()
                : DataType.Name == "Int" ? SQDataTypes.Int32.ToString()
                : DataType.Name == "SmallInt" ? SQDataTypes.Int16.ToString()
                : DataType.Name);

            int len = 0;
            if (DataType.HasLength && Int32.TryParse(Length, out len))
            {
                _Column.Length = len;
            }

            int precision = 0;
            if (DataType.HasPrecision && Int32.TryParse(Precision, out precision))
            {
                _Column.Precision = precision;
            }

            int scale = 0;
            if (DataType.HasPrecision && Int32.TryParse(Scale, out scale))
            {
                _Column.Scale = scale;
            }

            if (DataType.PKOK)
            {
                _Column.IsPrimary = IsPrimary;
            }
            if (DataType.IDOK)
            {
                _Column.IsIdentity = IsIdentity;
            }

            _Column.Nullable = Nullable;

            _Column.DefaultValue = DefaultValue;

            return _Column;
        }
    }

    public class JSDataType
    {
        public string Name { get; set; }
        public bool HasLength { get; set; }
        public bool HasPrecision { get; set; }
        public bool IDOK { get; set; }
        public bool PKOK { get; set; }
        public bool FKOK { get; set; }
    }

    public class JSFKey
    {
        public JSFKeyPair From { get; set; }
        public JSFKeyPair To { get; set; }

        internal JSDiagram Diagram { get; set; }

        internal void Relate(JSDiagram dg)
        {
            Diagram = dg;
            From.Relate(this);
            To.Relate(this);
        }
    }

    public class JSFKeyPair
    {
        private JSTable _TheTable = null;
        private JSColumn _TheColumn = null;

        public string Table { get; set; }
        public string Column { get; set; }

        internal JSFKey _FKey { get; set; }

        public JSTable GetTable()
        {
            return _TheTable ?? (_TheTable = _FKey.Diagram.Tables.FirstOrDefault(x => x.GUID == Table));
        }

        public JSColumn GetColumn()
        {
            return _TheColumn ?? (_TheColumn = GetTable().Columns.FirstOrDefault(x => x.GUID == Column));
        }

        internal void Relate(JSFKey fk)
        {
            _FKey = fk;
        }
    }
}