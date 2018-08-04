using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Schema;

namespace SeaQuail
{
    public class ExtendableList<T> : IList<T>
    {
        protected List<T> _List = new List<T>();

        public virtual void AddRange(IEnumerable<T> items)
        {
            _List.AddRange(items);
        }

        #region IList<T> Members
        public virtual int IndexOf(T item)
        {
            return _List.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            _List.Insert(index, item);
        }

        public virtual void RemoveAt(int index)
        {
            _List.RemoveAt(index);
        }

        public virtual T this[int index]
        {
            get
            {
                return _List[index];
            }
            set
            {
                _List[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public virtual void Add(T item)
        {
            _List.Add(item);
        }

        public virtual void Clear()
        {
            _List.Clear();
        }

        public virtual bool Contains(T item)
        {
            return _List.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _List.CopyTo(array, arrayIndex);
        }

        public virtual int Count
        {
            get { return _List.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual bool Remove(T item)
        {
            return _List.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        #endregion
    }
}
