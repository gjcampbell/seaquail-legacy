using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Data;
using SeaQuail.Schema;

namespace SeaQuail
{
    public interface IDBExecutor
    {
        SQSelectResult Select(SQSelectQuery query);
        SQSelectResult Insert(SQInsertQuery query);
        void Insert(SQInsertFromQuery query);
        void Update(SQUpdateQuery query);
        void Delete(SQDeleteQuery query);

        SQTable GetTable(string name);
        void CreateTable(SQTable table);
        void RemoveTable(string name);

        void AddColumn(SQColumn col);
        void RemoveColumn(SQColumn col);
        void RenameColumn(SQColumn col, string oldName);

        void AddForeignKey(SQColumn from, SQColumn to);
        SQColumn GetForeignKeyColumn(SQColumn local);
    }
}
