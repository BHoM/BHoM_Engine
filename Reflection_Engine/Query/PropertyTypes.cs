using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Type> PropertyTypes(this object obj, bool goDeep = false)
        {
            return obj.GetType().PropertyTypes(goDeep);
        }

        /***************************************************/

        public static List<Type> PropertyTypes(this Type type, bool goDeep = false)
        {
            HashSet<Type> properties = new HashSet<Type>();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite || prop.GetMethod.GetParameters().Count() > 0) continue;
                properties.Add(prop.PropertyType);
                if (goDeep)
                {
                    foreach (Type t in prop.PropertyType.PropertyObjects(true))
                        properties.Add(t);
                }
            }
            return properties.ToList();
        }

        /***************************************************/
    }
}
