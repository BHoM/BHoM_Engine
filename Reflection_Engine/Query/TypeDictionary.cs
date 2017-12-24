using System;
using System.Collections.Generic;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<string, List<Type>> GetBHoMTypeDictionary()
        {
            // If the dictionary exists already return it
            if (m_BHoMTypeDictionary != null && m_BHoMTypeDictionary.Count > 0)
                return m_BHoMTypeDictionary;

            // Otherwise, create it
            m_BHoMTypeDictionary = new Dictionary<string, List<Type>>();
            ExtractAllTypes();

            return m_BHoMTypeDictionary;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void AddBHoMTypeToDictionary(string name, Type type)
        {
            if (m_BHoMTypeDictionary.ContainsKey(name))
                m_BHoMTypeDictionary[name].Add(type);
            else
            {
                List<Type> list = new List<Type>();
                list.Add(type);
                m_BHoMTypeDictionary[name] = list;
            }

            int firstDot = name.IndexOf('.');
            if (firstDot >= 0)
                AddBHoMTypeToDictionary(name.Substring(firstDot + 1), type);
        }
        

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, List<Type>> m_BHoMTypeDictionary = new Dictionary<string, List<Type>>();

    }
}
