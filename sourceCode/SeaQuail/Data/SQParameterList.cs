using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public class SQParameterList : ExtendableList<SQParameter>
    {
        public SQParameter this[string name]
        {
            get
            {
                return GetParameterByName(name);
            }
            set
            {
                SQParameter p = GetParameterByName(name);
                if (p == null)
                {
                    Add(value);
                }
                else
                {
                    this[IndexOf(p)] = value;
                }
            }
        }

        public SQParameter GetParameterByName(string name)
        {
            foreach (SQParameter param in this)
            {
                if (param.Name == name)
                {
                    return param;
                }
            }

            return null;
        }
    }
}
