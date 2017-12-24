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

        public static List<Type> GetBHoMEnumList()
        {
            return GetBHoMTypeList().Where(x => x.IsEnum).ToList();
        }

    }
}
