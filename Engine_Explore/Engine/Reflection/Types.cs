using Engine_Explore.BHoM.DataStructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Engine.Reflection
{
    public static class Types
    {
        public static Dictionary<Type, IList> GroupByType(IEnumerable<object> data)
        {
            Dictionary<Type, IList> groups = new Dictionary<Type, IList>();

            foreach (object item in data)
            {
                Type type = item.GetType();
                Type listType = typeof(List<>).MakeGenericType(type);

                if (!groups.ContainsKey(type))
                    groups.Add(type, Activator.CreateInstance(listType) as IList);
                groups[type].Add(item);
            }

            return groups;
        }

        /***************************************************/

        public static Tree<Type> GetTypeTree(Type type)
        {
            Tree<Type> tree = new Tree<Type>();

            tree.Value = type;

            foreach (PropertyInfo info in type.GetProperties())
                tree.Childrens.Add(GetTypeTree(info.PropertyType));

            return tree;
        }

        /***************************************************/

        public static List<Type> GetLinkedTypes(Type type)
        {
            HashSet<Type> set = new HashSet<Type>();

            foreach (PropertyInfo info in type.GetProperties())
            {
                foreach (Type linkedType in GetLinkedTypes(info.PropertyType))
                    set.Add(linkedType);
            }

            return set.ToList();
        }
    }
}
