using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Data;
using MSBDataSlave.Schema;
using SeaQuail;

namespace MSBDataSlave.Data
{
    public class ORMCond : ORMCondBase
    {
        public class ORMNullOperand
        {
            internal ORMNullOperand() { }
        }

        public static readonly ORMNullOperand NullOperand = new ORMNullOperand();

        public string PropertyChain { get; set; }
        public SQRelationOperators Operator { get; set; }
        public object Value { get; set; }

        public ORMCond(string propertyChain, SQRelationOperators op, object value)
            : this(propertyChain, op, value, false) { }
        public ORMCond(string propertyChain, SQRelationOperators op, object value, bool invertLogic)
        {
            PropertyChain = propertyChain;
            Operator = op;
            Value = value;
            InvertMeaning = invertLogic;
        }
    }
}
