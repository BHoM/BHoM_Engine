using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Types
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<Type, List<object>> GroupByType(this IEnumerable<object> objects, bool goDeep = false)
        {
            if (!goDeep)
                return objects.GroupBy(x => x.GetType()).ToDictionary(x => x.Key, x => x.ToList());
            else
            {
                return objects.SelectMany(x => x.GetPropertyObjects(true)).Concat(objects).Distinct()
                                .GroupBy(x => x.GetType()).ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        /***************************************************/

        public static List<object> GetPropertyObjects(this object obj, bool goDeep = false)
        {
            List<object> properties = new List<object>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite || prop.GetMethod.GetParameters().Count() > 0) continue;  
                var value = prop.GetValue(obj, null);
                if (value != null && !(value is ValueType))
                {
                    properties.Add(value);
                    if (goDeep)
                        properties.AddRange(value.GetPropertyObjects(true));
                }
            }
            return properties;
        }

        /***************************************************/

        public static Dictionary<Type, List<object>> GetPropertyObjects(this IEnumerable<object> objects, Type type)
        {
            Dictionary<Type, List<object>> propByType = new Dictionary<Type, List<object>>();
            foreach(var prop in type.GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite || prop.PropertyType.IsValueType || prop.GetMethod.GetParameters().Count() > 0) continue;
                List<object> properties = new List<object>();
                foreach (object obj in objects)
                {
                    var value = prop.GetValue(obj, null);
                    if (value != null)
                        properties.Add(value);
                }
                if (properties.Count > 0)
                    propByType.Add(prop.PropertyType, properties);
            }
            return propByType;
        }

    }
}
