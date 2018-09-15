using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Mono.Cecil;
using Mono.Reflection;
using BH.Engine.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<string> UsedNamespaces(this MethodBase method, bool onlyBHoM = false, int maxDepth = 10)
        {
            return method.UsedTypes(onlyBHoM).Select(x => ClipNamespace(x.Namespace, maxDepth)).Distinct().ToList();
        }

        /***************************************************/

        public static List<string> UsedNamespaces(this Type type, bool onlyBHoM = false, int maxDepth = 10)
        {
            return type.UsedTypes(onlyBHoM).Select(x => ClipNamespace(x.Namespace, maxDepth)).Distinct().ToList();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string ClipNamespace(string nameSpace, int maxDepth)
        {
            return nameSpace.Split('.').Take(maxDepth).Aggregate((a, b) => a + '.' + b);
        }

        /***************************************************/
    }
}
