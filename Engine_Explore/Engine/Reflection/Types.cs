using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }
}
