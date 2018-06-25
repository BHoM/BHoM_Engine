using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<string> PropertyNames(this object obj)
        {
            if (obj is CustomObject)
                return PropertyNames(obj as CustomObject);
            else
                return obj.GetType().PropertyNames();
        }

        /***************************************************/

        public static List<string> PropertyNames(this Type type)
        {
            List<string> names = new List<string>();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead) continue;
                names.Add(prop.Name);
            }
            return names;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<string> PropertyNames(this CustomObject obj)
        {
            return obj.GetType().PropertyNames().Where(x => x != "CustomData").Concat(obj.CustomData.Keys.ToList()).ToList();
        }

        /***************************************************/
    }
}
