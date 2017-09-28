using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Type> GetPropertyTypes(this object obj, bool goDeep = false)
        {
            return obj.GetType().GetPropertyTypes(goDeep);
        }

        /***************************************************/

        public static List<Type> GetPropertyTypes(this Type type, bool goDeep = false)
        {
            HashSet<Type> properties = new HashSet<Type>();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite || prop.GetMethod.GetParameters().Count() > 0) continue;
                properties.Add(prop.PropertyType);
                if (goDeep)
                {
                    foreach (Type t in prop.PropertyType.GetPropertyObjects(true))
                        properties.Add(t);
                }
            }
            return properties.ToList();
        }
    }
}
