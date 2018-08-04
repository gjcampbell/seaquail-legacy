using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail.Data;
using SeaQuail.SchemaQuery;
using SeaQuail.Schema;

namespace SeaQuail
{
    public interface ISQLWriter
    {
        string Write(SQParameter paramter);
        string Write(SQCustomSQL sql);
        string Write(SQInsertQuery insert);
        string Write(SQInsertFromQuery insertFrom);
        string Write(SQSelectQuery select);
        string Write(SQUpdateQuery update);
        string Write(SQDeleteQuery delete);
        string Write(SQConditionBase condition);
        string Write(SQFromClause from);
        string Write(SQFromTable fromTable);
        string Write(SQJoin join);
        string Write(IWriteSQL writes);
        string Write(SQAddColumn addColumn);
        string Write(SQAddForeignKey addKey);
        string Write(SQAddIndex addIndex);
        string Write(SQCreateTable createTable);
        string Write(SQInsertColumn insertColumn);
        string Write(SQRemoveColumn removeColumn);
        string Write(SQRemoveForeignKey removeKey);
        string Write(SQRemoveIndex removeIndex);
        string Write(SQRemoveTable removeTable);
        string Write(SQRenameColumn renameColumn);
        string Write(SQRenameTable renameTable);
    }
}
