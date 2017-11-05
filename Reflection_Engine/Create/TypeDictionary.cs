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

        public static List<Type> AdapterTypeList()
        {
            // If the dictionary exists already return it
            if (m_AdapterTypeList != null && m_AdapterTypeList.Count > 0)
                return m_AdapterTypeList;

            // Otherwise, create it
            ExtractAllTypes();

            return m_AdapterTypeList;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractAllTypes()
        {
            m_TypeDictionary = new Dictionary<string, List<Type>>();
            m_TypeList = new List<Type>();
            m_AdapterTypeList = new List<Type>();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // Save BHoM objects only
                    string name = asm.GetName().Name;
                    if (name == "BHoM" || name.EndsWith("_oM"))
                    {
                        foreach (Type type in asm.GetTypes())
                        {
                            if (!type.IsInterface)
                            {
                                m_TypeList.Add(type);
                                AddTypeToDictionary(type.FullName, type);
                            }
                        }
                    }
                    // Save adapters
                    else if (name.EndsWith("_Adapter"))
                    {
                        foreach (Type type in asm.GetTypes())
                        {
                            if (!type.IsInterface)
                            {
                                m_AdapterTypeList.Add(type);
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
        private static List<Type> m_TypeList = new List<Type>();
        private static List<Type> m_AdapterTypeList = new List<Type>();

    }
}
