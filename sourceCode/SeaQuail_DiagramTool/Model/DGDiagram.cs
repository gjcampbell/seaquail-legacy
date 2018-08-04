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
    [SchemaHintTable(TableName = "Diagram")]
    public class DGDiagram : DGBase<DGDiagram>
    {
        #region Private Fields
        private DGSnapshot _PrimarySnapshot = null;
        private DGUser _User = null;
        private DGChildList<DGDiagram, DGSnapshot> _Snapshots = null;
        #endregion

        #region Properties
        [ScriptIgnore]
        [SchemaHintForeignKey(ForeignKey = typeof(DGUser), Storage = "UserID")]
        public DGUser User
        {
            get { return _User ?? (_User = DGUser.ByID(UserID)); }
            set { UserID = (User = value) == null ? default(Int64) : value.ID; }
        }

        [SchemaHintColumn(Nullable = true)]
        public Int64 UserID
        {
            get { return GetField("UserID").AsInt64; }
            set { SetFieldNullIfDefault<Int64>("UserID", value); }
        }

        [SchemaHintColumn(Nullable = true, Length = 250)]
        public string Name
        {
            get { return GetField("Name").AsString; }
            set { SetField("Name", value); }
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

        public bool AllowPublicAccess
        {
            get { return GetField("AllowPublicAccess").AsBoolean; }
            set { SetField("AllowPublicAccess", value); }
        }

        public DGChildList<DGDiagram, DGSnapshot> Snapshots
        {
            get
            {
                return _Snapshots ?? (_Snapshots = new DGChildList<DGDiagram,DGSnapshot>().AttachToParent(this, (s, d) => s.Diagram = d, ID > 0 ? DGSnapshot.ByDiagramID(ID) : null));
            }
        }
        #endregion

        protected override void BeforeSave(SQTransaction exec)
        {
            ModifyDate = DateTime.Now;
            if (!_Persisted)
            {
                CreateDate = ModifyDate;
            }

            base.BeforeSave(exec);
        }

        protected override void AfterSave(SQTransaction exec)
        {
            if (PrimarySnapshot.DiagramID != ID)
            {
                PrimarySnapshot.DiagramID = ID;
            }
            PrimarySnapshot.Save(exec);
            
            base.AfterSave(exec);
        }

        protected override void BeforeDelete(SQTransaction exec)
        {
            foreach (DGSnapshot snap in DGSnapshot.ByDiagramID(ID))
            {
                snap.Delete(exec);
            }
            
            base.BeforeDelete(exec);
        }

        public static List<DGDiagram> ByUserID(Int64 userID)
        {
            return Select(new ORMCond("UserID", 
                userID != 0 ? SQRelationOperators.Equal : SQRelationOperators.Is, 
                userID != 0 ? (object)userID : null));
        }

        public void Share(string email, DGSharePermisson permission)
        {
            DGShare share = DGShare.ByEmailAndDiagram(email, ID).Single();
            if (share == null)
            {
                share = new DGShare()
                {
                    DiagramID = ID,
                    Email = email
                };
            }

            share.Permission = permission;
            share.Save();
        }

        public void Unshare(string email)
        {
            DGShare.ByEmailAndDiagram(email, ID).ForEach(x => x.Delete());
        }

        public DGSnapshot PrimarySnapshot
        {
            get
            {
                return _PrimarySnapshot ?? (_PrimarySnapshot = DGSnapshot.ByDiagramID(ID).FirstOrDefault(x => x.IsDefault) ??
                    new DGSnapshot()
                    {
                        Name = "Primary Diagram",
                        DiagramID = ID,
                        IsDefault = true
                    });
            }
        }
    }
}
