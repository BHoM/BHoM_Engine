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

        public static List<MethodInfo> GetBHoMMethodList()
        {
            // If the dictionary exists already return it
            if (m_BHoMMethodList != null && m_BHoMMethodList.Count > 0)
                return m_BHoMMethodList;

            // Otherwise, create it
            ExtractAllMethods();

            return m_BHoMMethodList;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractAllMethods()
        {
            m_BHoMMethodList = new List<MethodInfo>();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // Save BHoM objects only
                    string name = asm.GetName().Name;
                    if (name.EndsWith("_Engine"))
                    {
                        foreach (Type type in asm.GetTypes())
                        {
                            if (!type.IsInterface && type.IsAbstract)
                                m_BHoMMethodList.AddRange(type.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static));
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Cannot load types from assembly " + asm.GetName().Name);
                }
            }
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<MethodInfo> m_BHoMMethodList = new List<MethodInfo>();

    }
}
