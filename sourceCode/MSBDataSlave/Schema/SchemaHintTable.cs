using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBDataSlave.Schema
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SchemaHintTable : Attribute
    {
        public string TableName { get; set; }
        public bool? LockSchema { get; set; }
        public bool? NoInheritedProperties { get; set; }
    }
}
