using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBDataSlave.Data
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(Type t)
            : base (string.Format("A schema could not be generated for type {0}. Try adding schema hints. ", t.AssemblyQualifiedName))
        {
        }
    }
}
