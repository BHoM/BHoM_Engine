using BH.oM.Reflection;
using System;
using System.Collections;
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

        public static UnderlyingType UnderlyingType(this Type type)
        {
            int depth = 0;
            while (type.GetGenericArguments().Count() == 1 && typeof(IEnumerable).IsAssignableFrom(type))
            {
                depth++;
                type = type.GetGenericArguments().First();
            }

            return new UnderlyingType { Type = type, Depth = depth };
        }

        /***************************************************/
    }
}
