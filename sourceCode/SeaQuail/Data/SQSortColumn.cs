using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SeaQuail.Data
{
    public class SQSortColumn
    {
        public string Column { get; set; }
        public SortOrder Direction { get; set; }
    }
}
