using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBDataSlave.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ORMChildJoinHint : Attribute
    {
        public Type ChildType
        {
            get;
            set;
        }

        public string FKeyPropertyName
        {
            get;
            set;
        }

        public ORMChildJoinHint()
        {
        }
    }
}
