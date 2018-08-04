using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBDataSlave.Schema
{
    public class SchemaHintForeignKey : Attribute
    {
        /// <summary>
        /// The type of the parent object
        /// </summary>
        public Type ForeignKey { get; set; }
        /// <summary>
        /// The name of the property in which the ID of the parent object is stored
        /// </summary>
        public string Storage { get; set; }
        /// <summary>
        /// When not created as a property attribute, you must specify the name of the property by which the parent object is accessed
        /// </summary>
        public string PropertyName { get; set; }
    }
}
