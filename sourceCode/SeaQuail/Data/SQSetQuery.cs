using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public abstract class SQSetQuery : SQQueryBase
    {
        private List<SQSetQueryPair> _SetPairs = null;

        public List<SQSetQueryPair> SetPairs
        {
            get
            {
                if (_SetPairs == null)
                {
                    _SetPairs = new List<SQSetQueryPair>();
                }
                return _SetPairs;
            }
            set
            {
                _SetPairs = value;
            }
        }
    }

    public class SQSetQueryPair
    {
        /// <summary>
        /// Column to be set
        /// </summary>
        public string Left { get; set; }
        /// <summary>
        /// Value to set
        /// </summary>
        public string Right { get; set; }

        public SQSetQueryPair() { }
        public SQSetQueryPair(string left, string right)
            : this()
        {
            Left = left;
            Right = right;
        }
    }
}
