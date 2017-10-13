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

        public static Dictionary<string, List<Type>> TypeDictionary()
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
                            AddType(type.FullName, type);
                    }

                    //// Save full names for all dlls       // Let's see if we actually need more than the BHoM types
                    //foreach (Type type in types)
                    //    m_TypeDictionary[type.FullName] = type;
                }
                catch (Exception)
                {
                    Console.WriteLine("Cannot load types from assembly " + asm.GetName().Name);
                }
            }

            return m_TypeDictionary;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void AddType(string name, Type type)
        {
            if (m_TypeDictionary.ContainsKey(name))
                m_TypeDictionary[name].Add(type);
            else
            {
                List<Type> list = new List<Type>();
                list.Add(type);
                m_TypeDictionary[name] = list;
            }

            int firstDot = name.IndexOf('.');
            if (firstDot >= 0)
                AddType(name.Substring(firstDot + 1), type);
        }
        

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, List<Type>> m_TypeDictionary = new Dictionary<string, List<Type>>();

    }
}
