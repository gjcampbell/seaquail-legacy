using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public abstract class SQQueryBase : IWriteSQL
    {
        private List<SQParameter> _Parameters = null;

        public List<SQParameter> Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new List<SQParameter>();
                }

                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }

        public abstract string Write(ISQLWriter adp);
    }
}
