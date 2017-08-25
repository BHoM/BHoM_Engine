using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        private static Dictionary<string, Type> CreateTypeDictionary()
        {
            // If the dictionary exists already return it
            if (m_TypeDictionary != null && m_TypeDictionary.Count > 0)
                return m_TypeDictionary;

            // Otherwise, create it
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    Type[] types = asm.GetTypes();

                    // Save shorter names for BHoM objects only
                    string name = asm.GetName().Name;
                    if (name == "BHoM" || name.EndsWith("_oM"))
                    {
                        foreach (Type type in types)
                            m_TypeDictionary[type.Name] = type;
                    }

                    // Save full names for all dlls
                    foreach (Type type in types)
                        m_TypeDictionary[type.FullName] = type;
                }
                catch (Exception)
                {
                    Console.WriteLine("Cannot load types from assembly " + asm.GetName().Name);
                }
            }

            return m_TypeDictionary;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, Type> m_TypeDictionary = new Dictionary<string, Type>();

    }
}
