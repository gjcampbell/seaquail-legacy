using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBDataSlave.Data
{
    public class ORMCondGroup : ORMCondBase
    {
        /// <summary>
        /// The inner condition chain initiator
        /// </summary>
        public ORMCondBase InnerCondition { get; set; }

        public ORMCondGroup() { }
        public ORMCondGroup(ORMCondBase innercond)
        {
            InnerCondition = innercond;
        }
    }
}
