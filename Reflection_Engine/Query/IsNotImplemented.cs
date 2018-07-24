using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsNotImplemented(this MethodBase method)
        {
            NotImplemented attribute = method.GetCustomAttribute<NotImplemented>();
            return (attribute != null);
        }


        /***************************************************/

        public static bool IsNotImplemented(this Type type) 
        {
            NotImplemented attribute = type.GetCustomAttribute<NotImplemented>();
            return (attribute != null);
        }


        /***************************************************/



    }
}
