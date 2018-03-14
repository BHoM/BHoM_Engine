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

        public static Dictionary<Type, List<object>> ObjectsByType(this IEnumerable<object> objects, bool goDeep = false)
        {
            if (!goDeep)
                return objects.GroupBy(x => x.GetType()).ToDictionary(x => x.Key, x => x.ToList());
            else
            {
                return objects.SelectMany(x => x.PropertyObjects(true)).Concat(objects).Distinct()
                                .GroupBy(x => x.GetType()).ToDictionary(x => x.Key, x => x.ToList());
            }
        }


    }
}
