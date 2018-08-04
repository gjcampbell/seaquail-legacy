using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail;
using System.Reflection;
using SeaQuail.Schema;

namespace MSBDataSlave.Schema
{
    public class SchemaTable
    {
        public List<SchemaHintTable> TableHints { get; private set; }
        public List<SchemaHintForeignKey> FKeyHints { get; private set; }
        public List<SchemaHintColumn> ColumnHints { get; private set; }
        public List<SchemaColumn> Columns { get; private set; }
        public Type ObjectType { get; private set; }
        public SQTable Table { get; private set; }

        public bool LockSchema { get; private set; }
        public bool NoInheritedProperties { get; private set; }

        public SchemaTable(Type t)
        {
            Columns = new List<SchemaColumn>();
            TableHints = new List<SchemaHintTable>();
            FKeyHints = new List<SchemaHintForeignKey>();
            ColumnHints = new List<SchemaHintColumn>();

            ObjectType = t;
            
            // gather all hints first from properties, then from those
            // declared on the class, TODO: last gather hints from a
            // delegate call allowing the hints on classes onto which
            // attributes cannot be added
            #region Gather Hints
            foreach (PropertyInfo pi in t.GetProperties())
            {
                foreach (Attribute att in Attribute.GetCustomAttributes(pi))
                {
                    if (att is SchemaHintColumn)
                    {
                        ((SchemaHintColumn)att).PropertyName = pi.Name;
                        ColumnHints.Add((SchemaHintColumn)att);
                    }
                    else if (att is SchemaHintForeignKey)
                    {
                        SchemaHintForeignKey fk = (SchemaHintForeignKey)att;
                        if (string.IsNullOrEmpty(fk.PropertyName))
                        {
                            fk.PropertyName = pi.Name;
                        }
                        FKeyHints.Add((SchemaHintForeignKey)att);
                    }
                }
            }

            foreach (Attribute att in Attribute.GetCustomAttributes(t))
            {
                if (att is SchemaHintTable)
                {
                    TableHints.Add((SchemaHintTable)att);
                }
                else if (att is SchemaHintColumn)
                {
                    ColumnHints.Add((SchemaHintColumn)att);
                }
                else if (att is SchemaHintForeignKey)
                {
                    FKeyHints.Add((SchemaHintForeignKey)att);
                }
            }
            #endregion
            
            // construct table using the type and the table hints
            Table = new SQTable() { Name = t.Name };
            foreach (SchemaHintTable hint in TableHints)
            {
                if (!string.IsNullOrEmpty(hint.TableName))
                {
                    Table.Name = hint.TableName;
                }
                if (hint.LockSchema.HasValue)
                {
                    LockSchema = hint.LockSchema.Value;
                }
                if (hint.NoInheritedProperties.HasValue)
                {
                    NoInheritedProperties = hint.NoInheritedProperties.Value;
                }
            }

            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (NoInheritedProperties && pi.DeclaringType != t)
                {
                    continue;
                }

                // collect all column hints specific to this property to be
                // passed in the schemahintcol constructor
                List<SchemaHintColumn> colHints = new List<SchemaHintColumn>();
                foreach (SchemaHintColumn colHint in ColumnHints)
                {
                    if (colHint.PropertyName == pi.Name)
                    {
                        colHints.Add(colHint);
                    }
                }

                // create the schemacolumn and if a db column could be created
                // for it, use add it to the table.
                SchemaColumn col = new SchemaColumn(this, colHints, pi);
                if (col.Column != null)
                {
                    Columns.Add(col);
                    Table.Columns.Add(col.Column);
                }
            }
        }

        public SchemaColumn GetColumnByName(string name)
        {
            foreach (SchemaColumn col in Columns)
            {
                if (col.Property.Name == name)
                {
                    return col;
                }
            }

            return null;
        }

        public SchemaColumn GetPrimaryKeyColumn()
        {
            foreach (SchemaColumn col in Columns)
            {
                if (col.Column.IsPrimary)
                {
                    return col;
                }
            }

            return null;
        }

        public SchemaHintForeignKey GetFKeyByName(string name)
        {
            foreach (SchemaHintForeignKey fk in FKeyHints)
            {
                if (fk.PropertyName == name)
                {
                    return fk;
                }
            }

            return null;
        }
    }
}
