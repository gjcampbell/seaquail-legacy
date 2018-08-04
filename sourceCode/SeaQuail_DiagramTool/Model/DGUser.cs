using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSBDataSlave.Schema;
using MSBDataSlave.Data;
using SeaQuail.Data;

namespace SeaQuail_DiagramTool.Model
{
    [SchemaHintTable(TableName = "DiagramUser")]
    public class DGUser : DGBase<DGUser>
    {
        #region Properties
        [SchemaHintColumn(Length = 250, Nullable = true)]
        public string Name
        {
            get { return GetField("Name").AsString; }
            set { SetField("Name", value); }
        }

        [SchemaHintColumn(Length = 250, Nullable = true)]
        public string ExternalID
        {
            get { return GetField("ExternalID").AsString; }
            set { SetField("ExternalID", value); }
        }
        #endregion

        public static DGUser ByExternalID(string id)
        {
            List<DGUser> user = Select(new ORMCond("ExternalID", SQRelationOperators.Equal, id));
            if (user.Count > 0)
            {
                return user[0];
            }

            return null;
        }

        public static DGUser ByName(string name)
        {
            List<DGUser> user = Select(new ORMCond("Name", SQRelationOperators.Equal, name));
            if (user.Count > 0)
            {
                return user[0];
            }

            return null;
        }
    }
}
