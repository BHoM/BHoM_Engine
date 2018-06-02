using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Assembly> BHoMAssemblyList()
        {
            if (m_BHoMAssemblies == null || m_BHoMAssemblies.Count == 0)
                ExtractAllAssemblies();

            return m_BHoMAssemblies.Values.ToList();
        }

        /***************************************************/

        public static List<Assembly> AllAssemblyList()
        {
            if (m_AllAssemblies == null || m_AllAssemblies.Count == 0)
                ExtractAllAssemblies();

            return m_AllAssemblies.Values.ToList();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractAllAssemblies()
        {
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
            m_BHoMAssemblies = assemblies.Where(x => x.IsBHoM()).ToDictionary(x => x.FullName);
            m_AllAssemblies = assemblies.ToDictionary(x => x.FullName);
        }

        /***************************************************/

        private static bool IsBHoM(this Assembly assembly)
        {
            string name = assembly.GetName().Name;
            return name.EndsWith("oM") || name.EndsWith("_Engine") || name.EndsWith("_Adapter") || name.EndsWith("_UI") || name.EndsWith("_Test");
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, Assembly> m_BHoMAssemblies = null;
        private static Dictionary<string, Assembly> m_AllAssemblies = null;

        /***************************************************/
    }
}
