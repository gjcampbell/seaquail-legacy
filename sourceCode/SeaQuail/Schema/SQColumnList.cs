using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Schema
{
    public class SQColumnList : ExtendableList<SQColumn>
    {
        #region Private Fields
        private SQTable _Table = null;
        #endregion

        #region Properties
        public SQTable Table
        {
            get
            {
                return _Table;
            }
            internal set
            {
                _Table = value;
                EnsureTable();
            }
        }
        #endregion

        #region Constructors
        public SQColumnList() { }
        public SQColumnList(SQTable table)
            : this()
        {
            Table = table;
        }
        #endregion

        public override void AddRange(IEnumerable<SQColumn> columns)
        {
            foreach (SQColumn col in columns)
            {
                col.Table = Table;
            }
            base.AddRange(columns);
        }

        private void EnsureTable()
        {
            foreach (SQColumn col in this)
            {
                col.Table = Table;
            }
        }

        public override void Insert(int index, SQColumn item)
        {
            item.Table = Table;
            base.Insert(index, item);
        }

        public override void RemoveAt(int index)
        {
            _List[index].Table = null;
            base.RemoveAt(index);
        }

        public override void Add(SQColumn item)
        {
            item.Table = Table;
            base.Add(item);
        }

        public override void Clear()
        {
            foreach (SQColumn col in this)
            {
                col.Table = null;
            }
            base.Clear();
        }

        public override bool Remove(SQColumn item)
        {
            item.Table = null;
            return base.Remove(item);
        }
    }
}
