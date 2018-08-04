using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Schema;
using System.Data.Common;
using System.Data;

namespace SeaQuail.Data
{
    public class SQTransaction : IDBExecutor, IDisposable
    {
        private SQAdapter _Adapter;
        private DbTransaction _Transaction;

        public delegate void CloseHandler();

        public event CloseHandler OnClose;

        protected internal SQTransaction(SQAdapter adp, DbTransaction trans)
        {
            _Adapter = adp;
            _Transaction = trans;
        }

        public void Commit()
        {
            DbConnection conn = _Transaction.Connection;
            _Transaction.Commit();
            Close();
        }

        public void RollBack()
        {
            DbConnection conn = _Transaction.Connection;
            _Transaction.Rollback();
            Close();
        }

        #region IDBExecutor Members

        public SQSelectResult Select(SQSelectQuery query)
        {
            return _Adapter.Select(query);
        }

        public SQSelectResult Insert(SQInsertQuery query)
        {
            return _Adapter.Insert(query);
        }

        public void Insert(SQInsertFromQuery query)
        {
            _Adapter.Insert(query);
        }

        public void Update(SQUpdateQuery query)
        {
            _Adapter.Update(query);
        }

        public void Delete(SQDeleteQuery query)
        {
            _Adapter.Delete(query);
        }

        public SQTable GetTable(string name)
        {
            return _Adapter.GetTable(name);
        }

        public void CreateTable(SQTable table)
        {
            _Adapter.CreateTable(table);
        }

        public void RemoveTable(string name)
        {
            _Adapter.RemoveTable(name);
        }

        public void AddColumn(SQColumn col)
        {
            _Adapter.AddColumn(col);
        }

        public void RemoveColumn(SQColumn col)
        {
            _Adapter.RemoveColumn(col);
        }

        public void RenameColumn(SQColumn col, string oldName)
        {
            _Adapter.RenameColumn(col, oldName);
        }

        public void AddForeignKey(SQColumn from, SQColumn to)
        {
            _Adapter.AddForeignKey(from, to);
        }

        public SQColumn GetForeignKeyColumn(SQColumn local)
        {
            return _Adapter.GetForeignKeyColumn(local);
        }

        #endregion

        private void Close()
        {
            if (_Transaction.Connection != null && _Transaction.Connection.State == ConnectionState.Open)
            {
                _Transaction.Connection.Close();
            }
            if (OnClose != null)
            {
                OnClose();
            }
        }

        #region IDisposable Members
        
        public void Dispose()
        {
            if (_Transaction.Connection.State != ConnectionState.Closed)
            {
                _Transaction.Connection.Close();
            }
        }

        #endregion
    }
}
