using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Schema
{
    /// <summary>
    /// Represents a database table, currently this consists only
    /// of a list of SQColumns and a name. 
    /// TODO:
    /// -- Extended properties?
    /// -- Multiple primary keys
    /// -- Methods of manipulating the table such as add/remove 
    ///    columns and rename, deleting the table. 
    /// </summary>
    public class SQTable
    {
        private SQColumnList _Columns = null;

        public string Name { get; set; }
        public SQColumnList Columns 
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new SQColumnList(this);
                }
                return _Columns;
            }
            set
            {
                _Columns = value;
                if (_Columns.Table != this)
                {
                    _Columns.Table = this;
                }
            }
        }

        /// <summary>
        /// Set columns' table to this one. This is a shitty thing. Columns ought to have their table property set when they're added to a column list.
        /// </summary>
        public void EnsureColumns()
        {
            foreach (SQColumn col in Columns)
            {
                col.Table = this;
            }
        }

        /// <summary>
        /// Get the primary key column. Return null if there is none.
        /// </summary>
        /// <returns></returns>
        public SQColumn GetPrimaryKey()
        {
            foreach (SQColumn col in Columns)
            {
                if (col.IsPrimary)
                {
                    return col;
                }
            }

            return null;
        }

        /// <summary>
        /// Find a column with the passed name, and return it. Return null if not found.
        /// </summary>
        /// <param name="name">Name of the column to look for, not case sensitive</param>
        /// <returns></returns>
        public SQColumn GetColumnByName(string name)
        {
            foreach (SQColumn col in Columns)
            {
                if (col.Name.ToLower() == name.ToLower())
                {
                    return col;
                }
            }

            return null;
        }
    }
}
