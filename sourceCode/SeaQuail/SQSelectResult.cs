using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace SeaQuail
{
    /// <summary>
    /// This contains the open database connection and data reader returned by a select
    /// </summary>
    public class SQSelectResult : IDisposable
    {
        public DbConnection Connection { get; private set; }
        public DbDataReader Reader { get; private set; }
        internal Int64 TotalRecordCount { get; set; }
        public SQSelectResult(DbConnection con, DbDataReader rdr)
        {
            Connection = con;
            Reader = rdr;
        }

        public DataTable GetDataTable()
        {
            DataTable res = new DataTable();
            res.Load(Reader);
            Close();
            return res;
        }

        public void Close()
        {
            Reader.Close();
            if (Connection != null)
            {
                Connection.Close();
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            Close();
        }
        #endregion
    }
}
