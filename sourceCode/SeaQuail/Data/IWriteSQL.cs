using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public interface IWriteSQL
    {
        string Write(ISQLWriter wr);
    }
}
