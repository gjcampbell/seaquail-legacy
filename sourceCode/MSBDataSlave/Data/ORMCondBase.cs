using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Data;

namespace MSBDataSlave.Data
{
    public class ORMCondBase
    {
        /// <summary>
        /// The And or Or logic operator
        /// </summary>
        public SQLogicOperators Connective { get; private set; }
        /// <summary>
        /// The subsequent condition
        /// </summary>
        public ORMCondBase NextCondition { get; private set; }
        /// <summary>
        /// True becomes false, false becomes true.
        /// </summary>
        public bool InvertMeaning { get; set; }

        /// <summary>
        /// Append a condition with a connective "or" to this condition chain and return this.
        /// </summary>
        /// <param name="nextCondition"></param>
        /// <returns></returns>
        public ORMCondBase Or(ORMCondBase nextCondition)
        {
            AppendCondition(nextCondition, SQLogicOperators.OR);
            return this;
        }
        public ORMCondBase Or(string operandA, SQRelationOperators op, string operandB)
        {
            return Or(operandA, op, operandB, false);
        }
        public ORMCondBase Or(string operandA, SQRelationOperators op, string operandB, bool invert)
        {
            return Or(new ORMCond(operandA, op, operandB, invert));
        }

        /// <summary>
        /// Append a condition with a connective "and" to this condition chain and return this.
        /// </summary>
        /// <param name="nextCondition"></param>
        /// <returns></returns>        
        public ORMCondBase And(ORMCondBase nextCondition)
        {
            AppendCondition(nextCondition, SQLogicOperators.AND);
            return this;
        }
        public ORMCondBase And(string operandA, SQRelationOperators op, object operandB)
        {
            return And(operandA, op, operandB, false);
        }
        public ORMCondBase And(string operandA, SQRelationOperators op, object operandB, bool invert)
        {
            return And(new ORMCond(operandA, op, operandB, invert));
        }

        private void AppendCondition(ORMCondBase condition, SQLogicOperators connective)
        {
            if (NextCondition == null)
            {
                NextCondition = condition;
                Connective = connective;
            }
            else
            {
                NextCondition.AppendCondition(condition, connective);
            }
        }
    }
}
