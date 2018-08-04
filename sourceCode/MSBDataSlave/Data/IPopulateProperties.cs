using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBDataSlave.Data
{
    public interface IPopulateProperties
    {
        void PopulateProperty(string name, object value);
    }
}
