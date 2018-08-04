using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSBDataSlave.Schema;
using System.Web.Script.Serialization;
using SeaQuail.Schema;
using MSBDataSlave.Data;

namespace SeaQuail_DiagramTool.Model
{
    public enum DGSharePermisson { View = 0, Edit = 1 }

    [SchemaHintTable(TableName = "Share")]
    public class DGShare : DGBase<DGShare>
    {
        #region Private Fields
        private DGDiagram _Diagram = null;
        #endregion

        [SchemaHintColumn(Length = 150, Nullable = false)]
        public string Email
        {
            get { return GetField("Email").AsString; }
            set { SetField("Email", value); }
        }

        [ScriptIgnore]
        [SchemaHintForeignKey(ForeignKey = typeof(DGDiagram), Storage = "DiagramID")]
        public DGDiagram Diagram
        {
            get { return _Diagram ?? (_Diagram = DGDiagram.ByID(DiagramID)); }
            set { DiagramID = (_Diagram = value) == null ? default(Int64) : value.ID; }
        }

        public Int64 DiagramID
        {
            get { return GetField("DiagramID").AsInt64; }
            set { SetField("DiagramID", value); }
        }

        [SchemaHintColumn(DataType = SQDataTypes.Int32)]
        public DGSharePermisson Permission
        {
            get { return (DGSharePermisson)GetField("Permission").AsInt32; }
            set { SetField("Permission", (int)value); }
        }

        public static List<DGShare> ByEmailAndDiagram(string email, Int64 diagramID)
        {
            return Select(new ORMCond("Email", SeaQuail.Data.SQRelationOperators.Equal, email)
                .And("DiagramID", SeaQuail.Data.SQRelationOperators.Equal, diagramID));
        }

        public static List<DGShare> ByDiagram(Int64 diagramID)
        {
            return Select(new ORMCond("DiagramID", SeaQuail.Data.SQRelationOperators.Equal, diagramID));
        }
    }
}
