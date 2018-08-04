using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail;
using System.Reflection;
using System.IO;
using SeaQuail.Schema;
using System.Collections.Concurrent;

namespace MSBDataSlave.Schema
{
    public class SchemaManager
    {
        private static ConcurrentDictionary<SQAdapter, SchemaManager> _Mgrs = new ConcurrentDictionary<SQAdapter, SchemaManager>();

        public static bool DisableSchemaBuild { get; set; }

        public static SchemaManager GetMgr(SQAdapter adp)
        {
            if (!_Mgrs.ContainsKey(adp))
            {
                _Mgrs.TryAdd(adp, new SchemaManager(adp));
            }

            return _Mgrs[adp];
        }

        private Dictionary<Type, SchemaTable> _SchemaTables { get; set; }

        public SQAdapter Adp { get; private set; }

        private SchemaManager(SQAdapter apd)
        {
            Adp = apd;
            _SchemaTables = new Dictionary<Type, SchemaTable>();
        }

        public SchemaTable EnsureSchema(Type type)
        {
            if (!_SchemaTables.ContainsKey(type))
            {
                SchemaTable res = new SchemaTable(type);
                if (res.Columns.Count < 1)
                {
                    _SchemaTables.Add(type, null);
                }
                else
                {
                    _SchemaTables.Add(type, res);
                    SQTable curr = Adp.GetTable(res.Table.Name);
                    if (curr != null)
                    {
                        if (!res.LockSchema)
                        {
                        }
                    }
                    else if (!DisableSchemaBuild)
                    {
                        Adp.CreateTable(res.Table);
                        foreach (SchemaHintForeignKey fk in res.FKeyHints)
                        {
                            SchemaTable fkt = EnsureSchema(fk.ForeignKey);
                            Adp.AddForeignKey(res.GetColumnByName(fk.Storage).Column, fkt.Table.GetPrimaryKey());
                        }
                    }                    
                }
            }

            return _SchemaTables[type];
        }

        public SchemaTable GetSchemaForType(Type t)
        {
            // TODO: resolve inheritence here
            if (!_SchemaTables.ContainsKey(t))
            {
                SchemaTable table = new SchemaTable(t);
                if (table.Columns.Count < 1)
                {
                    _SchemaTables.Add(t, null);
                }
                else
                {
                    _SchemaTables.Add(t, table);
                }
            }

            return _SchemaTables[t];
        }
    }
}
