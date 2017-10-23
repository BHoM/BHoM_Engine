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
            ExtractAllTypes();

            return m_TypeDictionary;
        }

        /***************************************************/

        public static List<Type> TypeList()
        {
            // If the dictionary exists already return it
            if (m_TypeList != null && m_TypeList.Count > 0)
                return m_TypeList;

            // Otherwise, create it
            ExtractAllTypes();

            return m_TypeList;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractAllTypes()
        {
            m_TypeDictionary = new Dictionary<string, List<Type>>();
            m_TypeList = new List<System.Type>();

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
                        {
                            if (!type.IsInterface)
                            {
                                m_TypeList.Add(type);
                                AddTypeToDictionary(type.FullName, type);
                            }
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

        private static void AddTypeToDictionary(string name, Type type)
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
                AddTypeToDictionary(name.Substring(firstDot + 1), type);
        }
        

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, List<Type>> m_TypeDictionary = new Dictionary<string, List<Type>>();
        private static List<Type> m_TypeList = new List<System.Type>();

    }
}
