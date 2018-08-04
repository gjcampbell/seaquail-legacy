using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Schema
{
    public enum IndexOrders { ASC, DESC }

    /// <summary>
    /// A column used in an SQIndex
    /// </summary>
    public class SQIndexItem
    {
        public IndexOrders Order { get; set; }
        public string ColumnName { get; set; }
    }

    /// <summary>
    /// A table index with an ordered list of columns
    /// </summary>
    public class SQIndex
    {
        #region Private Fields
        private List<SQIndexItem> _Items = new List<SQIndexItem>();
        #endregion

        public string TableName { get; set; }

        public List<SQIndexItem> Items
        {
            get { return _Items ?? (_Items = new List<SQIndexItem>()); }
            set { _Items = value; }
        }
    }
}
