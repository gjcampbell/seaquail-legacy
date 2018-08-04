using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace MSBDataSlave.Data
{
    public class ORMPropertyChain
    {
        public string Chain { get; set; }

        public ORMPropertyChain() { }
        public ORMPropertyChain(string chain)
        {
            Chain = chain;
        }

        public List<PropertyInfo> GetChain(Type t)
        {
            List<PropertyInfo> res = new List<PropertyInfo>();

            Type parentType = t;
            foreach (string link in Chain.Split('.'))
            {
                PropertyInfo pi = null;
                if ((pi = parentType.GetProperty(link)) != null)
                {
                    res.Add(pi);
                    parentType = GetParent(pi);
                }
                else
                {
                    throw new Exception("Invalid property in chain '" + Chain + "' " + parentType  + " has no property '" + link + "'");
                }
            }

            return res;
        }

        /// <summary>
        /// The ORM type object from property info, the property type for parent relationships, a generic list's object for child relationships
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static Type GetParent(PropertyInfo pi)
        {
            if (typeof(IEnumerable).IsAssignableFrom(pi.PropertyType))
            {
                Type baseType = pi.PropertyType;
                ORMChildJoinHint hint = pi.GetJoinHint();
                if (hint != null && hint.ChildType != null)
                {
                    return hint.ChildType;
                }
                else
                {
                    do
                    {
                        Type[] types = baseType.GetGenericArguments();
                        if (types.Length > 0)
                        {
                            return types[0];
                        }
                    }
                    while ((baseType = baseType.BaseType) != null);
                }
            }
            else
            {
                return pi.PropertyType;
            }
            return null;
        }
    }
}
