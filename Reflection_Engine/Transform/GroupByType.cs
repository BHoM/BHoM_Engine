using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Transform
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
