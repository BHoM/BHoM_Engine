using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<Type, List<object>> GetGroupedByType(this IEnumerable<object> objects, bool goDeep = false)
        {
            if (!goDeep)
                return objects.GroupBy(x => x.GetType()).ToDictionary(x => x.Key, x => x.ToList());
            else
            {
                return objects.SelectMany(x => x.GetPropertyObjects(true)).Concat(objects).Distinct()
                                .GroupBy(x => x.GetType()).ToDictionary(x => x.Key, x => x.ToList());
            }
        }


    }
}
