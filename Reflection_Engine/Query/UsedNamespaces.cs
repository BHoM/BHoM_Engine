using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Mono.Cecil;
using Mono.Reflection;
using BH.Engine.Reflection.Convert;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<string> UsedNamespaces(this MethodBase method, bool onlyBHoM = false, int maxDepth = 10)
        {
            IEnumerable<string> namespaces = new List<string>();
            try
            {
                IEnumerable<object> operands = Disassembler.GetInstructions(method).Select(x => x.Operand);
                IEnumerable<string> methodsNS = operands.OfType<MethodBase>().Select(x => x.DeclaringType.Namespace).Distinct();

                IEnumerable<string> typesNS = method.UsedTypes(onlyBHoM).Select(x => x.Namespace);

                string nameSpace = method.DeclaringType.Namespace;
                namespaces = typesNS.Union(methodsNS)
                    .Where(x => x != nameSpace)
                    .Select(x => x.Split('.').Take(maxDepth).Aggregate((a, b) => a + '.' + b))
                    .Distinct();
            }
            catch (Exception e)
            {
                Compute.RecordWarning("Failed to get used namespaces for method " + method.ToText() + "/nError: " + e.ToString());
            }


            if (onlyBHoM)
                return namespaces.Where(x => x.StartsWith("BH.")).ToList();
            else
                return namespaces.ToList();
        }

        /***************************************************/
    }
}
