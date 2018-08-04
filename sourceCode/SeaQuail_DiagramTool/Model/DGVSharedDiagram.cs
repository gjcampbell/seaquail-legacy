using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SeaQuail.Data;

namespace SeaQuail_DiagramTool.Model
{
    public class DGVSharedDiagram : DGView<DGVSharedDiagram>
    {
        static DGVSharedDiagram()
        {
            SQSelectQuery q = new SQSelectQuery()
            {
                From = new SQFromClause(new SQFromTable("Share")
                {
                    Join = new SQJoin("Diagram")
                    {
                        Predicate = new SQCondition("Diagram.ID", SQRelationOperators.Equal, "Share.DiagramID"),
                        Join = new SQJoin("DiagramUser")
                        {
                            Predicate = new SQCondition("DiagramUser.ID", SQRelationOperators.Equal, "Diagram.UserID")
                        }
                    }
                }),
                Columns = new List<SQAliasableObject>
                {
                    new SQAliasableObject("Share.Permission", "Permission"),
                    new SQAliasableObject("Share.Email", "Email"),
                    new SQAliasableObject("Diagram.ID", "DiagramID"),
                    new SQAliasableObject("Diagram.Name", "Diagram"),
                    new SQAliasableObject("DiagramUser.ID", "OwnerID"),
                    new SQAliasableObject("DiagramUser.Name", "Owner")
                }
            };
            RegisterView(q);
        }

        public DGSharePermisson Permission
        {
            get { return (DGSharePermisson)GetField("Permission").AsInt32; }
        }

        public string Email
        {
            get { return GetField("Email").AsString; }
        }

        public Int64 DiagramID
        {
            get { return GetField("DiagramID").AsInt64; }
        }

        public string Diagram
        {
            get { return GetField("Diagram").AsString; }
        }

        public Int64 OwnerID
        {
            get { return GetField("OwnerID").AsInt64; }
        }

        public string Owner
        {
            get { return GetField("Owner").AsString; }
        }

        public static List<DGVSharedDiagram> ByEmail(string email)
        {
            return Select(new SQCondition("Share.Email", SQRelationOperators.Like, "@Email"), new SQParameter("@Email", email));
        }        
    }
}
