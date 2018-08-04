using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public class SQParameter : IWriteSQL
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public SQParameter() { }
        public SQParameter(string name, object value)
            : this()
        {
            Name = name;
            Value = value;
        }

        #region IWriteSQL Members

        public string Write(ISQLWriter wr)
        {
            return wr.Write(this);
        }

        #endregion
    }
}
