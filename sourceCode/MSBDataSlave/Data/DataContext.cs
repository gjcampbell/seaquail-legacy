using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSBDataSlave.Schema;
using SeaQuail;
using SeaQuail.Data;
using System.Data.Common;
using System.Reflection;
using System.IO;
using SeaQuail.Schema;

namespace MSBDataSlave.Data
{
    /// <summary>
    /// TODO: extend this and make an ORMDataContext which might enforce things like as an ID and keep a cache of all objects opened
    /// </summary>
    public class DataContext
    {
        public SchemaManager Schema { get; private set; }
        public SQAdapter Adp { get; private set; }

        public DataContext(SQAdapter adp)
        {
            Adp = adp;
            Schema = SchemaManager.GetMgr(adp);
        }

        public virtual void Update(object o, ORMCondBase condition)
        {
            //if (o != null)
            //{
            //    SchemaTable t = Schema.EnsureSchema(o.GetType());
            //    if (t != null)
            //    {
            //        UpdateQuery q = new UpdateQuery()
            //        {
            //            PrimaryTable = new AliasableObject(t.Table.Name)
            //        };
            //        PopulateSetQuery(q, o, t);
            //        q.Condition = condition;
            //        Adp.Update(q);
            //    }
            //    else
            //    {
            //        throw new InvalidTypeException(o.GetType());
            //    }
            //}
        }

        public virtual void Insert(object o)
        {
            if (o != null)
            {
                SchemaTable t = Schema.EnsureSchema(o.GetType());
                if (t != null)
                {
                    SQInsertQuery q = new SQInsertQuery() 
                    { 
                        Table = new SQAliasableObject(t.Table.Name),
                        ReturnID = t.GetPrimaryKeyColumn() != null && t.GetPrimaryKeyColumn().Column.IsIdentity
                    };
                    PopulateSetQuery(q, o, t);
                    SQSelectResult sr = Adp.Insert(q);
                    try
                    {
                        if (q.ReturnID)
                        {
                            if (sr.Reader.Read())
                            {
                                SchemaColumn pkCol = t.GetPrimaryKeyColumn();
                                object id = sr.Reader.GetValue(0);
                                if (id.GetType() != pkCol.Property.PropertyType)
                                {
                                    switch (pkCol.Column.DataType)
                                    {
                                        case SQDataTypes.Int16:
                                            id = Convert.ToInt16(id);
                                            break;
                                        case SQDataTypes.Int32:
                                            id = Convert.ToInt32(id);
                                            break;
                                        case SQDataTypes.Int64:
                                            id = Convert.ToInt64(id);
                                            break;
                                        case SQDataTypes.Decimal:
                                            id = Convert.ToDecimal(id);
                                            break;
                                    }
                                }

                                if (o is IPopulateProperties)
                                {
                                    ((IPopulateProperties)o).PopulateProperty(pkCol.Property.Name, id);
                                }
                                else
                                {
                                    pkCol.Property.SetValue(o, id, null);
                                }
                            }
                        }
                    }
                    finally
                    {
                        sr.Close();
                    }
                }
                else
                {
                    throw new InvalidTypeException(o.GetType());
                }
            }
        }

        public T GetByID<T>(object id) where T : new()
        {
            SchemaTable t = Schema.EnsureSchema(typeof(T));
            if (t != null)
            {
                ORMSelect s = new ORMSelect(Adp);
                s.FocalType = typeof(T);
                s.Condition = new ORMCond(t.GetPrimaryKeyColumn().Property.Name, SQRelationOperators.Equal, id);
                s.Top = 1;
                List<T> result = Select<T>(s);
                if (result.Count > 0)
                {
                    return result[0];
                }
            }

            return default(T);
        }
        public List<T> Select<T>() where T : new()
        {
            return Select<T>(new ORMSelect(Adp) { FocalType = typeof(T) });
        }
        public List<T> Select<T>(IDBExecutor exec) where T : new()
        {
            return Select<T>(new ORMSelect(Adp) { FocalType = typeof(T) }, exec);
        }
        public List<T> Select<T>(ORMCondBase condition) where T : new()
        {
            return Select<T>(condition, Adp);
        }
        public List<T> Select<T>(ORMCondBase condition, IDBExecutor exec) where T : new()
        {
            ORMSelect s = new ORMSelect(Adp);
            s.FocalType = typeof(T);
            s.Condition = condition;
            return Select<T>(s, exec);
        }
        public List<T> Select<T>(ORMSelect select) where T : new()
        {
            return Select<T>(select, Adp);
        }
        public virtual List<T> Select<T>(ORMSelect select, IDBExecutor exec) where T : new()
        {
            List<T> res = new List<T>();
            
            SchemaTable t = Schema.EnsureSchema(typeof(T));
            if (t != null)
            {
                SQSelectResult sr = exec.Select(select.GetQuery().Query);
                try
                {
                    while (sr.Reader.Read())
                    {
                        T item = new T();
                        BeforePopulateFromDB(item);
                        for (int i = 0; i < t.Columns.Count; i++)
                        {
                            if (!sr.Reader.IsDBNull(i))
                            {
                                if (item is IPopulateProperties)
                                {
                                    ((IPopulateProperties)item).PopulateProperty(t.Columns[i].Property.Name, GetFieldValue(sr.Reader, i, t.Columns[i]));
                                }
                                else
                                {
                                    t.Columns[i].Property.SetValue(item, GetFieldValue(sr.Reader, i, t.Columns[i]), null);
                                }
                            }
                        }
                        res.Add(item);
                    }
                }
                finally
                {
                    sr.Connection.Close();
                }
            }
            else
            {
                throw new InvalidTypeException(typeof(T));
            }

            return res;
        }

        protected virtual void BeforePopulateFromDB(object o)
        {
        }

        protected virtual void PopulateSetQuery(SQSetQuery q, object o, SchemaTable t)
        {
            foreach (SchemaColumn col in t.Columns)
            {
                if (!col.Column.IsIdentity)
                {
                    object val = o is IPopulateParameters ? ((IPopulateParameters)o).GetParamaterValue(col.Property.Name) : col.Property.GetValue(o, null);
                    if (val is IWriteSQL)
                    {
                        q.SetPairs.Add(new SQSetQueryPair(col.Column.Name, ((IWriteSQL)val).Write(Adp)));
                    }
                    else
                    {
                        string var = Adp.CreateVariable(col.Property.Name);
                        q.SetPairs.Add(new SQSetQueryPair(col.Column.Name, var));
                        q.Parameters.Add(new SQParameter(var, val == null ? DBNull.Value : val));
                    }
                }
            }
        }

        protected virtual object GetFieldValue(DbDataReader rdr, int ordinal, SchemaColumn col)
        {
            switch (col.Column.DataType)
            {
                case SQDataTypes.Boolean:
                    return rdr.GetBoolean(ordinal);
                case SQDataTypes.Bytes:
                    MemoryStream ms = new MemoryStream();
                    byte[] bytes = new byte[8000];
                    long read = 0;
                    while ((read = rdr.GetBytes(ordinal, 0, bytes, 0, bytes.Length)) > 0)
                    {
                        ms.Write(bytes, 0, Convert.ToInt32(read));
                    }
                    if (col.Property.PropertyType == typeof(byte[]))
                    {
                        return ms.ToArray();
                    }
                    return ms.ToArray();
                    break;
                case SQDataTypes.DateTime:
                    return rdr.GetDateTime(ordinal);
                case SQDataTypes.Decimal:
                    return rdr.GetDecimal(ordinal);
                case SQDataTypes.Float:
                    return rdr.GetFloat(ordinal);
                case SQDataTypes.Int16:
                    Int16 small = rdr.GetInt16(ordinal);
                    if (col.Property.PropertyType == typeof(Byte))
                    {
                        return Convert.ToByte(small);
                    }
                    return small;
                case SQDataTypes.Int32:
                    return rdr.GetInt32(ordinal);
                case SQDataTypes.Int64:
                    Int64 value = rdr.GetInt64(ordinal);
                    if (col.Property.PropertyType.IsEnum)
                    {
                        return Enum.ToObject(col.Property.PropertyType, value);
                    }
                    return value;
                case SQDataTypes.String:
                    return rdr.GetString(ordinal);
            }

            return null;
        }
    }
}
