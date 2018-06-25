using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsInterfaceMethod(this MethodBase method)
        {
            if (method is ConstructorInfo)
                return false;

            string name = method.Name;
            if (name.Length <= 2 || name[0] != 'I' || char.IsLower(name[1]))
                return false;

            return true;
        }


        /***************************************************/



    }
}
