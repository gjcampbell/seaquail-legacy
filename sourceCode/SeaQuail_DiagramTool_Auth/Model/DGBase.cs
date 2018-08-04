using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSBDataSlave.Schema;
using SeaQuail.Data;
using MSBDataSlave.Data;
using SeaQuail;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Concurrent;

namespace SeaQuail_DiagramTool.Model
{
    public interface IDGBase
    {
        Int64 ID { get; }
    }

    public abstract class DGBase : IDGBase
    {
        public event SQTransactionHandler Saving;
        public event SQTransactionHandler Saved;
        public event SQTransactionHandler Deleting;
        public event SQTransactionHandler Deleted;

        public delegate void SQTransactionHandler(SQTransaction t);

        protected DGFieldMap _Map = new DGFieldMap();
        protected bool _Persisted = false;

        public delegate SQAdapter ProvideAdapter();

        public static ProvideAdapter AdapterProvider;

        protected static DGCtx GetCtx()
        {
            return new DGCtx();
        }

        protected static SQAdapter GetAdp()
        {
            return AdapterProvider();
        }

        [SchemaHintColumn(IsPrimary = true, IsIdentity = true)]
        public Int64 ID { get; protected set; }

        public void Delete()
        {
            SQTransaction trans = null;
            try
            {
                if (_Persisted)
                {
                    trans = GetAdp().OpenTransaction();
                    Delete(trans);
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.RollBack();
                }
                throw ex;
            }
        }
        public void Delete(SQTransaction trans)
        {
            if (_Persisted)
            {
                CallDeleting(trans);
                GetCtx().Delete(this, trans);
                CallDeleted(trans);
            }
        }

        public void Save()
        {
            SQTransaction trans = null;
            try
            {
                trans = GetAdp().OpenTransaction();
                Save(trans);
                trans.Commit();
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.RollBack();
                }
                throw ex;
            }
        }
        public void Save(SQTransaction trans)
        {
            CallSaving(trans);
            if (!_Persisted)
            {
                ID = GetCtx().Insert(this, trans);
            }
            else
            {
                GetCtx().Update(this, trans);
            }
            CallSaved(trans);
        }

        private void CallSaving(SQTransaction exec) 
        { 
            BeforeSave(exec);
            if (Saving != null)
            {
                Saving(exec);
            }
        }
        private void CallSaved(SQTransaction exec) 
        { 
            AfterSave(exec);
            if (Saved != null)
            {
                Saved(exec);
            }
        }
        private void CallDeleting(SQTransaction exec) 
        { 
            BeforeDelete(exec);
            if (Deleting != null)
            {
                Deleting(exec);
            }
        }
        private void CallDeleted(SQTransaction exec) 
        { 
            AfterDelete(exec);
            if (Deleted != null)
            {
                Deleted(exec);
            }
        }
        protected virtual void BeforeSave(SQTransaction exec) {}
        protected virtual void AfterSave(SQTransaction exec) {}
        protected virtual void BeforeDelete(SQTransaction exec) {}
        protected virtual void AfterDelete(SQTransaction exec) {}
                        
        public object this[string propertyName]
        {
            get { return GetField(propertyName).Value; }
            set { SetField(propertyName, value); }
        }

        protected void SetFieldNullIfDefault<T>(string name, T value) where T : IComparable
        {
            SetField(name, default(T).Equals(value) ? null : (object)value);
        }
        protected void SetField(string name, object value)
        {
            _Map[name].Value = value;
        }

        protected DGField GetField(string name)
        {
            return _Map[name];
        }

        #region IPopulateProperties Members

        public virtual void PopulateProperty(string name, object value)
        {
            switch (name)
            {
                case "ID":
                    ID = (Int64)value;
                    _Persisted = true;
                    break;
                default:
                    SetField(name, value);
                    break;
            }
        }

        #endregion

        #region IPopulateParameters Members

        public virtual object GetParamaterValue(string name)
        {
            switch (name)
            {
                case "ID": return ID;
                default: return _Map[name].Value; //return GetType().GetProperty(name).GetValue(this, null);
            }
        }

        #endregion
    }

    public class DGBase<T> : DGBase, IPopulateProperties, IPopulateParameters
        where T : DGBase<T>, new()
    {
        #region Convenient Select Methods
        public static List<T> Select()
        {
            return GetCtx().Select<T>();
        }
        public static List<T> Select(IDBExecutor exec)
        {
            return GetCtx().Select<T>(exec);
        }
        public static List<T> Select(ORMCondBase condition)
        {
            return GetCtx().Select<T>(condition);
        }
        public static List<T> Select(ORMCondBase condition, IDBExecutor exec)
        {
            return GetCtx().Select<T>(condition, exec);
        }
        public static List<T> Select(ORMSelect select)
        {
            return GetCtx().Select<T>(select);
        }
        public static List<T> Select(ORMSelect select, IDBExecutor exec)
        {
            return GetCtx().Select<T>(select, exec);
        }
        #endregion

        public static T ByID(Int64 id)
        {
            List<T> res = Select(new ORMCond("ID", SQRelationOperators.Equal, id.ToString()));
            if (res.Count > 0)
            {
                return res[0];
            }

            return null;
        }
    }

    public abstract class DGView<T> : IPopulateProperties
        where T : DGView<T>, new()
    {
        private static ConcurrentDictionary<Type, SQSelectQuery> _Views = new ConcurrentDictionary<Type, SQSelectQuery>();

        protected static void RegisterView(SQSelectQuery sq)
        {
            _Views.TryAdd(typeof(T), sq);
        }

        public static List<T> Select(SQConditionBase cond, params SQParameter[] parameters)
        {
            List<T> res = new List<T>();

            SQSelectQuery q = _Views[typeof(T)];
            q.Condition = cond;
            q.Parameters = new List<SQParameter>(parameters);
            SQAdapter adp = DGBase.AdapterProvider();
            SQSelectResult rdr = q.Execute(adp);
            
            if (rdr.Reader.Read())
            {
                List<string> fields = new List<string>();
                for (int i = 0; i < rdr.Reader.VisibleFieldCount; i++)
                {
                    fields.Add(rdr.Reader.GetName(i));
                }

                do
                {
                    T item = new T();
                    for (int i = 0; i < rdr.Reader.VisibleFieldCount; i++)
                    {
                        item.PopulateProperty(fields[i], rdr.Reader.GetValue(i));
                    }
                    res.Add(item);
                }
                while (rdr.Reader.Read());
            }

            return res;
        }

        private DGFieldMap _Map = new DGFieldMap();
        
        protected DGField GetField(string name)
        {
            return _Map[name];
        }

        #region IPopulateProperties Members
        public void PopulateProperty(string name, object value)
        {
            _Map[name].Value = value;
        }
        #endregion
    }

    public class DGDiagramView : DGView<DGDiagramView>
    {
        static DGDiagramView()
        {
            SQSelectQuery sq = new SQSelectQuery()
            {
                From = new SQFromClause(new SQFromTable("Diagram", "DG")
                {
                    Join = new SQJoin("Snapshot", "Snap")
                    {
                        Predicate = new SQCondition("Snap.DiagramID", SQRelationOperators.Equal, "DG.ID")
                            .And("Snap.IsDefault", SQRelationOperators.Equal, true)
                    }
                }),
                Columns = new List<SQAliasableObject>
                {
                    new SQAliasableObject("DG.Name","ID"),
                    new SQAliasableObject("DG.Name","Name"),
                    new SQAliasableObject("Snap.DiagramData","Def"),
                }
            };
            RegisterView(sq);
        }

        public Int64 ID
        {
            get { return GetField("ID").AsInt64; }
        }

        public string Name
        {
            get { return GetField("Name").AsString; }
        }

        public string Def
        {
            get { return GetField("Def").AsString; }
        }
    }

    public class DGCtx : DataContext
    {
        public DGCtx()
            : base(DGBase.AdapterProvider())
        {
        }

        public Int64 Insert(IDGBase vl)
        {
            return Insert(vl, Adp);
        }
        public Int64 Insert(IDGBase vl, IDBExecutor exec)
        {
            SchemaTable t = Schema.EnsureSchema(vl.GetType());
            if (t != null)
            {
                SQInsertQuery ins = new SQInsertQuery()
                {
                    Table = new SQAliasableObject(t.Table.Name),
                    ReturnID = true
                };
                PopulateSetQuery(ins, vl, t);
                return ins.ExecuteReturnID<Int64>(exec);
            }

            return -1;
        }

        public void Update(IDGBase vl)
        {
            Update(vl, Adp);
        }
        public void Update(IDGBase vl, IDBExecutor exec)
        {
            SchemaTable t = Schema.EnsureSchema(vl.GetType());
            if (t != null)
            {
                SQUpdateQuery upd = new SQUpdateQuery()
                {
                    UpdateTable = new SQAliasableObject(t.Table.Name),
                    Condition = new SQCondition(t.Table.GetPrimaryKey().Name, SQRelationOperators.Equal, vl.ID.ToString())
                };
                PopulateSetQuery(upd, vl, t);
                upd.Execute(exec);
            }
        }

        public void Delete(IDGBase vl)
        {
            Delete(vl, Adp);
        }
        public void Delete(IDGBase vl, IDBExecutor exec)
        {
            SchemaTable t = Schema.EnsureSchema(vl.GetType());
            if (t != null)
            {
                SQDeleteQuery dlt = new SQDeleteQuery()
                {
                    DeleteTable = new SQAliasableObject(t.Table.Name),
                    Condition = new SQCondition(t.Table.GetPrimaryKey().Name, SQRelationOperators.Equal, vl.ID.ToString())
                };
                dlt.Execute(exec);
            }
        }
    }

    public class DGFieldMap : IDictionary<string, DGField>
    {
        private Dictionary<string, DGField> _Dict = new Dictionary<string, DGField>();

        #region IDictionary<string,DGField> Members

        public void Add(string key, DGField value)
        {
            _Dict.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _Dict.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _Dict.Keys; }
        }

        public bool Remove(string key)
        {
            return _Dict.Remove(key);
        }

        public bool TryGetValue(string key, out DGField value)
        {
            return _Dict.TryGetValue(key, out value);
        }

        public ICollection<DGField> Values
        {
            get { return _Dict.Values; }
        }

        public DGField this[string key]
        {
            get
            {
                return _Dict.ContainsKey(key) ? _Dict[key] : _Dict[key] = new DGField(null);
            }
            set
            {
                if (!_Dict.ContainsKey(key))
                {
                    _Dict.Add(key, value);
                }
                else
                {
                    _Dict[key] = value;
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,DGField>> Members

        public void Add(KeyValuePair<string, DGField> item)
        {
            _Dict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _Dict.Clear();
        }

        public bool Contains(KeyValuePair<string, DGField> item)
        {
            return _Dict.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, DGField>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _Dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, DGField> item)
        {
            return _Dict.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,DGField>> Members

        public IEnumerator<KeyValuePair<string, DGField>> GetEnumerator()
        {
            return _Dict.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Dict.GetEnumerator();
        }

        #endregion
    }

    public class DGField
    {
        private object _Value = null;

        public DGField(object value)
        {
            _Value = value;
        }
        public DGField() { }

        #region Conversions
        public static implicit operator DGField(Int64 val) { return new DGField(val); }
        public static implicit operator DGField(Int32 val) { return new DGField(val); }
        public static implicit operator DGField(String val) { return new DGField(val); }
        public static implicit operator DGField(DateTime val) { return new DGField(val); }
        public static implicit operator DGField(Decimal val) { return new DGField(val); }
        public static implicit operator DGField(float val) { return new DGField(val); }
        public static implicit operator DGField(Boolean val) { return new DGField(val); }
        #endregion

        public Int64 AsInt64 { get { return (Int64)(_Value ?? default(Int64)); } }
        public Int32 AsInt32 { get { return (Int32)(_Value ?? default(Int64)); } }
        public String AsString { get { return (String)_Value; } }
        public DateTime AsDateTime { get { return (DateTime)(_Value ?? default(DateTime)); } }
        public Decimal AsDecimal { get { return (Decimal)(_Value ?? default(Decimal)); } }
        public float AsFloat { get { return (float)(_Value ?? default(float)); } }
        public Boolean AsBoolean { get { return (Boolean)(_Value ?? default(Boolean)); } }
        public object Value { get { return _Value; } set { _Value = value; } }
    }

    public class DGChildList<ParentType, ChildType> : ExtendableList<ChildType> 
        where ParentType : DGBase 
        where ChildType : DGBase
    {
        private Action<ChildType, ParentType> _ParentSetter;

        public ParentType Parent { get; private set; }

        public DGChildList<ParentType, ChildType> AttachToParent(
            ParentType parent, 
            Action<ChildType, ParentType> parentSetter,
            IEnumerable<ChildType> children)
        {
            if (Parent != null)
            {
                throw new Exception("You may never reattach, dude");
            }
            Parent = parent;
            Parent.Saved += new DGBase.SQTransactionHandler(Parent_Saved);
            Parent.Deleting += new DGBase.SQTransactionHandler(Parent_Deleting);
            _ParentSetter = parentSetter;

            AddRange(children);

            return this;
        }

        void Parent_Deleting(SQTransaction t)
        {
 	        foreach(ChildType item in this)
            {
                item.Delete(t);
            }
        }

        void Parent_Saved(SQTransaction t)
        {
            foreach (ChildType item in this)
            {
                item.Save(t);
            }
        }

        public override void Insert(int index, ChildType item)
        {
            _ParentSetter(item, Parent);

            base.Insert(index, item);
        }
        public override void Add(ChildType item)
        {
            _ParentSetter(item, Parent);

            base.Add(item);
        }

        public override bool Remove(ChildType item)
        {
            return base.Remove(item);
        }
        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
        }
        public override void Clear()
        {
            base.Clear();
        }
    }


    public static class IEnumerableDGBaseExtensions
    {
        public static T ByID<T>(this IEnumerable<T> list, Int64 id)
            where T : DGBase
        {
            return list.FirstOrDefault(x => x.ID == id);
        }

        public static T GetOne<T>(this IEnumerable<T> list, Func<T, bool> condition)
            where T : DGBase
        {
            return list.FirstOrDefault(condition);
        }

        /// <summary>
        /// Return first item in list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Single<T>(this IEnumerable<T> list)
            where T : DGBase
        {
            return list.FirstOrDefault(x => true);
        }

        public static List<T> GetMany<T>(this IEnumerable<T> list, Func<T, bool> condition)
            where T : DGBase
        {
            List<T> res = new List<T>();

            foreach (T item in list)
            {
                if (condition(item))
                {
                    res.Add(item);
                }
            }

            return res;
        }
    }

    public static class IEnumerableDGDiagramExtensions
    {
        public static DGDiagram ByName(this IEnumerable<DGDiagram> list, string name)
        {
            return list.GetOne(x => x.Name == name);
        }
    }
}
