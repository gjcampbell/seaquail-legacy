using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SeaQuail.Data
{
    public class SQSelectQuery : SQQueryBase
    {
        private bool _IncludeTotalRows = false;
        private List<SQAliasableObject> _Columns = null;
        private List<SQSortColumn> _SortColumns = null;
        private List<string> _GroupByColumns = null;

        /// <summary>
        /// The columns selected in the query
        /// </summary>
        public List<SQAliasableObject> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new List<SQAliasableObject>();
                }
                return _Columns;
            }
            set
            {
                _Columns = value;
            }
        }

        public bool Distinct { get; set; }

        /// <summary>
        /// The from clause of the query
        /// </summary>
        public SQFromClause From { get; set; }

        /// <summary>
        /// The initiator of the conditions of this select statement
        /// </summary>
        public SQConditionBase Condition { get; set; }

        /// <summary>
        /// For paging, enter the start row
        /// </summary>
        public int RecordStart { get; set; }

        /// <summary>
        /// For paging, enter the number of rows
        /// </summary>
        public int RecordCount { get; set; }

        public int Top { get; set; }

        /// <summary>
        /// Set to true to get a count of the total rows
        /// </summary>
        public bool IncludeTotalRows
        {
            get
            {
                return _IncludeTotalRows || RecordCount > 0 || RecordStart > 0;
            }
            set
            {
                _IncludeTotalRows = value;
            }
        }

        public List<SQSortColumn> SortColumns
        {
            get
            {
                if (_SortColumns == null)
                {
                    _SortColumns = new List<SQSortColumn>();
                }
                return _SortColumns;
            }
            set
            {
                _SortColumns = value;
            }
        }

        public List<string> GroupByColumns
        {
            get
            {
                if (_GroupByColumns == null)
                {
                    _GroupByColumns = new List<string>();
                }
                return _GroupByColumns;
            }
            set
            {
                _GroupByColumns = value;
            }
        }

        public override string Write(ISQLWriter wr)
        {
            return wr.Write(this);
        }

        public SQSelectResult Execute(IDBExecutor adp)
        {
            return adp.Select(this);
        }
    }
}
