using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public enum SQLogicOperators { AND, OR }
    public enum SQRelationOperators { Equal, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, StartsWith, Like, EndsWith, Contains, Before, After, In, Is }

    public abstract class SQConditionBase
    {
        /// <summary>
        /// The And or Or logic operator
        /// </summary>
        public SQLogicOperators Connective { get; private set; }
        /// <summary>
        /// The subsequent condition
        /// </summary>
        public SQConditionBase NextCondition { get; private set; }
        /// <summary>
        /// Set to true to invert the meaning of the condition. For 
        /// example, if the condition is a == b, then when set to 
        /// true the condition becomes a != b
        /// </summary>
        public bool InvertMeaning { get; set; }

        /// <summary>
        /// Append a condition with a connective "or" to this condition chain and return this.
        /// </summary>
        /// <param name="nextCondition"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase Or(SQConditionBase nextCondition)
        {
            AppendCondition(nextCondition, SQLogicOperators.OR);
            return this;
        }
        /// <summary>
        /// Append an "or" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase Or(string operandA, SQRelationOperators op, string operandB)
        {
            return Or(new SQCondition(operandA, op, operandB, false));
        }
        /// <summary>
        /// Append an "or" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase Or(string operandA, SQRelationOperators op, string operandB, bool invert)
        {
            return Or(new SQCondition(operandA, op, operandB, invert));
        }
        /// <summary>
        /// Append an "or" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase Or(string operandA, SQRelationOperators op, object operandB)
        {
            return Or(new SQCondition(operandA, op, operandB, false));
        }
        /// <summary>
        /// Append an "or" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase Or(string operandA, SQRelationOperators op, object operandB, bool invert)
        {
            return Or(new SQCondition(operandA, op, operandB, invert));
        }
        /// <summary>
        /// Append an "or" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase Or(object operandA, SQRelationOperators op, object operandB)
        {
            return Or(new SQCondition(operandA, op, operandB, false));
        }
        /// <summary>
        /// Append an "or" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase Or(object operandA, SQRelationOperators op, object operandB, bool invert)
        {
            return Or(new SQCondition(operandA, op, operandB, invert));
        }
        /// <summary>
        /// Append an "or" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase Or(IWriteSQL operandA, SQRelationOperators op, IWriteSQL operandB)
        {
            return Or(new SQCondition(operandA, op, operandB, false));
        }
        /// <summary>
        /// Append an "or" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase Or(IWriteSQL operandA, SQRelationOperators op, IWriteSQL operandB, bool invert)
        {
            return Or(new SQCondition(operandA, op, operandB, invert));
        }

        /// <summary>
        /// Append a condition with a connective "and" to this condition chain and return this.
        /// </summary>
        /// <param name="nextCondition"></param>
        /// <returns>The current condition</returns>        
        public SQConditionBase And(SQConditionBase nextCondition)
        {
            AppendCondition(nextCondition, SQLogicOperators.AND);
            return this;
        }
        /// <summary>
        /// Append an "and" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase And(string operandA, SQRelationOperators op, string operandB)
        {
            return And(operandA, op, operandB, false);
        }
        /// <summary>
        /// Append an "and" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase And(string operandA, SQRelationOperators op, string operandB, bool invert)
        {
            return And(new SQCondition(operandA, op, operandB, invert));
        }
        /// <summary>
        /// Append an "and" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase And(string operandA, SQRelationOperators op, object operandB)
        {
            return And(operandA, op, operandB, false);
        }
        /// <summary>
        /// Append an "and" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase And(string operandA, SQRelationOperators op, object operandB, bool invert)
        {
            return And(new SQCondition(operandA, op, operandB, invert));
        }
        /// <summary>
        /// Append an "and" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase And(object operandA, SQRelationOperators op, object operandB)
        {
            return And(operandA, op, operandB, false);
        }
        /// <summary>
        /// Append an "and" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase And(object operandA, SQRelationOperators op, object operandB, bool invert)
        {
            return And(new SQCondition(operandA, op, operandB, invert));
        }
        /// <summary>
        /// Append an "and" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase And(ISQLWriter operandA, SQRelationOperators op, ISQLWriter operandB)
        {
            return And(operandA, op, operandB, false);
        }
        /// <summary>
        /// Append an "and" condition. Pass the settings for a SQCondition
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="op"></param>
        /// <param name="operandB"></param>
        /// <returns>The current condition</returns>
        public SQConditionBase And(ISQLWriter operandA, SQRelationOperators op, ISQLWriter operandB, bool invert)
        {
            return And(new SQCondition(operandA, op, operandB, invert));
        }

        private void AppendCondition(SQConditionBase condition, SQLogicOperators connective)
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
