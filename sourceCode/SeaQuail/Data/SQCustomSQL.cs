using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public class SQCustomSQL : IWriteSQL
    {
        public string SQL { get; set; }

        public SQCustomSQL() { }
        public SQCustomSQL(string sql)
        {
            SQL = sql;
        }

        #region IWriteSQL Members

        public string Write(ISQLWriter wr)
        {
            return SQL;
        }

        #endregion
    }
}
