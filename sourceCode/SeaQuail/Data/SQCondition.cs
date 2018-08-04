using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public class SQCondition : SQConditionBase
    {
        /// <summary>
        /// The left operand
        /// </summary>
        public IWriteSQL OperandA { get; set; }
        /// <summary>
        /// The operator with which to compare the operands
        /// </summary>
        public SQRelationOperators Operator { get; set; }
        /// <summary>
        /// The right operand
        /// </summary>
        public IWriteSQL OperandB { get; set; }

        public SQCondition() { }
        /// <summary>
        /// Enter operands as strings to be written as they are
        /// </summary>
        public SQCondition(string operandA, SQRelationOperators op, string operandB)
            : this(operandA, op, operandB, false) { }
        /// <summary>
        /// Enter operands as strings to be written as they are
        /// </summary>
        public SQCondition(string operandA, SQRelationOperators op, string operandB, bool invertLogic)
            : this(new SQCustomSQL(operandA), op, new SQCustomSQL(operandB), invertLogic) { }
        /// <summary>
        /// Both operands are converted to unnamed parameters
        /// </summary>
        public SQCondition(object operandA, SQRelationOperators op, object operandB)
            : this(operandA, op, operandB, false) { }
        /// <summary>
        /// Both operands are converted to unnamed parameters
        /// </summary>
        public SQCondition(object operandA, SQRelationOperators op, object operandB, bool invertLogic)
            : this(new SQParameter("", operandA), op, new SQParameter("", operandB), invertLogic) { }
        /// <summary>
        /// The first operand is written as is, the second is converted to a parameter
        /// </summary>
        public SQCondition(string operandA, SQRelationOperators op, object operandB)
            : this(new SQCustomSQL(operandA), op, new SQParameter("", operandB), false) { }
        public SQCondition(IWriteSQL operandA, SQRelationOperators op, IWriteSQL operandB)
            : this(operandA, op, operandB, false) { }
        public SQCondition(IWriteSQL operandA, SQRelationOperators op, IWriteSQL operandB, bool invertLogic)
        {
            OperandA = operandA;
            Operator = op;
            OperandB = operandB;
            InvertMeaning = invertLogic;
        }
    }
}
