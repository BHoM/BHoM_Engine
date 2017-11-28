using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
