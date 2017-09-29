using BH.oM.Base;
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

        public static Dictionary<string,Type> GetObjectTypes(Assembly assembly, string nameSpace)
        {
            return _GetObjectTypes(assembly, nameSpace);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public static Dictionary<string, Type> _GetObjectTypes(Assembly assembly, string nameSpace)
        {   
            Type[] typeList =  assembly.GetTypes().Where(x => String.Equals(x.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();

            return typeList.ToDictionary(x => x.ToString(), x => x);
        }       
    }
}
