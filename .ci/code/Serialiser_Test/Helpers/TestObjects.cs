using BH.Engine.Reflection;
using BH.Engine.Serialiser;
using BH.Engine.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Test.Serialiser
{
    public static partial class Helpers
    {
        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        public static List<object> TestObjects(Type type)
        {
            if (m_TestObjects.ContainsKey(type))
                return m_TestObjects[type];
            else
                return new List<object>();
        }

        /*************************************/

        public static void AddTestObjects(Type type, List<object> objects)
        {
            if (m_TestObjects.ContainsKey(type))
                m_TestObjects[type].AddRange(objects);
            else
                m_TestObjects[type] = objects.ToList();
        }

        /*************************************/

        public static void ClearTestObjects(Type type = null)
        {
            if (type == null)
                m_TestObjects.Clear();
            else if (m_TestObjects.ContainsKey(type))
                m_TestObjects[type].Clear();
        }


        /*************************************/
        /**** Private Fields              ****/
        /*************************************/

        private static Dictionary<Type, List<object>> m_TestObjects = new Dictionary<Type, List<object>>();

        /*************************************/
    }
}
