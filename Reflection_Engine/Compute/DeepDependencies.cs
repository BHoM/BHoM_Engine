using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<MethodBase>> DeepDependencies(List<MethodBase> methods, int maxDepth = 10)
        {
            List<MethodBase> current = methods;
            IEnumerable<MethodBase> union = methods;
            List<List<MethodBase>> dependencies = new List<List<MethodBase>>();

            int depth = 0;
            while (current.Count() > 0 && depth < maxDepth)
            {
                IEnumerable<MethodBase> used = current.SelectMany(x => x.UsedMethods(true)).Distinct();
                IEnumerable<MethodBase> newUsed = used.Except(current);

                current = newUsed.Except(union).ToList();
                union = union.Union(used);
                depth++;

                if (current.Count > 0)
                    dependencies.Add(current);
            }

            return dependencies;
        }

        /***************************************************/
    }
}
