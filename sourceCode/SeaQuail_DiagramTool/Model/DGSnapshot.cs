using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSBDataSlave.Schema;
using MSBDataSlave.Data;
using SeaQuail.Data;
using System.Web.Script.Serialization;

namespace SeaQuail_DiagramTool.Model
{
    [SchemaHintTable(TableName = "Snapshot")]
    public class DGSnapshot : DGBase<DGSnapshot>
    {
        #region Private Fields
        private DGDiagram _Diagram = null;
        #endregion

        #region Properties
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
            set { SetFieldNullIfDefault<Int64>("DiagramID", value); }
        }

        [SchemaHintColumn(Nullable = true, Length = 250)]
        public string Name
        {
            get { return GetField("Name").AsString; }
            set { SetField("Name", value); }
        }

        [SchemaHintColumn(Nullable = true)]
        public string DiagramData
        {
            get { return GetField("DiagramData").AsString; }
            set { SetField("DiagramData", value); }
        }

        public bool IsDefault
        {
            get { return GetField("IsDefault").AsBoolean; }
            set { SetField("IsDefault", value); }
        }

        [SchemaHintColumn(Nullable = true)]
        public DateTime CreateDate
        {
            get { return GetField("CreateDate").AsDateTime; }
            set { SetField("CreateDate", value); }
        }

        [SchemaHintColumn(Nullable = true)]
        public DateTime ModifyDate
        {
            get { return GetField("ModifyDate").AsDateTime; }
            set { SetField("ModifyDate", value); }
        }
        #endregion

        protected override void BeforeSave(SeaQuail.Data.SQTransaction exec)
        {
            ModifyDate = DateTime.Now;
            if (!_Persisted)
            {
                CreateDate = ModifyDate;
            }

            base.BeforeSave(exec);
        }

        public static List<DGSnapshot> ByDiagramID(Int64 diagramID)
        {
            return Select(new ORMCond("DiagramID", SQRelationOperators.Equal, diagramID));
        }
    }
}
