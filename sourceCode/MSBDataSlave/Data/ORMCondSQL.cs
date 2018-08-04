using SeaQuail.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBDataSlave.Data
{
    public class ORMCondSQL : ORMCondBase
    {
        private SQParameterList _Paramaters = new SQParameterList();

        public SQCondition Cond
        {
            get;
            set;
        }

        public SQParameterList Parameters
        {
            get
            {
                SQParameterList sQParameterList = this._Paramaters;
                return sQParameterList;
            }
        }

        public ORMCondSQL(SQCondition cond, params SQParameter[] sqParams)
        {
            this.Cond = cond;
            this.Parameters.AddRange(sqParams);
        }
    }
}
