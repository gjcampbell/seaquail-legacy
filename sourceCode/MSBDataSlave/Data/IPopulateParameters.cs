using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBDataSlave.Data
{
    public interface IPopulateParameters
    {
        object GetParamaterValue(string name);
    }
}
