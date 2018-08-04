using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MSBDataSlave.Data
{
    public static class PropertyInfoExtensions
    {
        private static ConcurrentDictionary<PropertyInfo, ORMChildJoinHint> _JoinHints;

        static PropertyInfoExtensions()
        {
            PropertyInfoExtensions._JoinHints = new ConcurrentDictionary<PropertyInfo, ORMChildJoinHint>();
        }

        public static ORMChildJoinHint GetJoinHint(this PropertyInfo pi)
        {
            if (!PropertyInfoExtensions._JoinHints.ContainsKey(pi))
            {
                PropertyInfoExtensions._JoinHints.TryAdd(pi, null);
                Attribute[] customAttributes = Attribute.GetCustomAttributes(pi);
                int num = 0;
                while (num < (int)customAttributes.Length)
                {
                    Attribute attr = customAttributes[num];
                    if (!(attr is ORMChildJoinHint))
                    {
                        num++;
                    }
                    else
                    {
                        PropertyInfoExtensions._JoinHints[pi] = (ORMChildJoinHint)attr;
                        break;
                    }
                }
            }
            ORMChildJoinHint item = PropertyInfoExtensions._JoinHints[pi];
            return item;
        }
    }
}
