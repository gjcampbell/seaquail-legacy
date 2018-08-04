using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace SeaQuail.Data
{
    public class SQInsertQuery : SQSetQuery
    {
        public bool ReturnID { get; set; }

        public override string Write(ISQLWriter adp)
        {
            return adp.Write(this);
        }

        /// <summary>
        /// Table into which inserting will be done
        /// </summary>
        public SQAliasableObject Table { get; set; }

        /// <summary>
        /// Execute the insert
        /// </summary>
        /// <param name="adp"></param>
        public void Execute(IDBExecutor adp)
        {
            adp.Insert(this).Close();
        }

        /// <summary>
        /// Execute the insert and return the inserted identity value
        /// </summary>
        /// <typeparam name="T">Type of identity column</typeparam>
        /// <param name="adp"></param>
        /// <returns></returns>
        public T ExecuteReturnID<T>(IDBExecutor adp)
        {
            object value = ExecuteReturnID(adp);
            if (value != null)
            {
                if (value is T)
                {
                    return (T)value;
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default(T);
        }
        /// <summary>
        /// Execute the insert and return the inserted identity value
        /// </summary>
        /// <param name="adp"></param>
        /// <returns></returns>
        public object ExecuteReturnID(IDBExecutor adp)
        {
            bool prevValue = ReturnID;
            ReturnID = true;

            SQSelectResult res = null;
            try
            {
                res = adp.Insert(this);

                if (res.Reader.Read())
                {
                    return res.Reader.GetValue(0);
                }
            }
            finally
            {
                ReturnID = prevValue;
                if (res != null)
                {
                    res.Close();
                }
            }

            return null;
        }
    }
}

