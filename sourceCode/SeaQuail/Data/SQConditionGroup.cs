using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    /// <summary>
    /// Condition written as a nested condition
    /// </summary>
    public class SQConditionGroup : SQConditionBase
    {
        /// <summary>
        /// Initiator of nested condition chain
        /// </summary>
        public SQConditionBase InnerCondition { get; set; }

        public SQConditionGroup(SQConditionBase innerCondition)
            : this(false, innerCondition) { }
        public SQConditionGroup(bool invertMeaning, SQConditionBase innerCondition)
        {
            InnerCondition = innerCondition;
            InvertMeaning = invertMeaning;
        }
    }
}