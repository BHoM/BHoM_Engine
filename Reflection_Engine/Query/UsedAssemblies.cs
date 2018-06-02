using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Mono.Cecil;
using Mono.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Assembly> UsedAssemblies(this Assembly assembly, bool onlyBHoM = false)
        {
            try
            {
                if (m_AllAssemblies == null || m_AllAssemblies.Count == 0)
                    ExtractAllAssemblies();

                IEnumerable<AssemblyName> assemblyNames = assembly.GetReferencedAssemblies();
                foreach (AssemblyName name in assemblyNames)
                    Compute.RecordWarning("Could not find the assembly with the name " + name);

                return assemblyNames.Where(x => m_BHoMAssemblies.ContainsKey(x.FullName)).Select(x => m_BHoMAssemblies[x.FullName]).ToList();
            }
            catch (Exception e)
            {
                Compute.RecordWarning("failed to get the assemblies used by " + assembly.FullName + ".\nError: " + e.ToString());
                return new List<Assembly>();
            }
        }
            

        /***************************************************/
    }
}
