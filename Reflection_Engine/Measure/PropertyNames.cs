using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<string> GetPropertyNames(this object obj)
        {
            return obj.GetType().GetPropertyNames();
        }

        /***************************************************/

        public static List<string> GetPropertyNames(this Type type)
        {
            List<string> names = new List<string>();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                names.Add(prop.Name);
            }
            return names;
        }
    }
}
