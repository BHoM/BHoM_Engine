using BH.oM.Base;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BHoMGroup<BHoMObject> BHoMGroup(IEnumerable<BHoMObject> elements)
        {
            return new BHoMGroup<BHoMObject>
            {
                Elements = elements.ToList()
            };
        }

        /***************************************************/

        public static BHoMGroup<T> BHoMGroup<T>(IEnumerable<T> elements) where T:IBHoMObject
        {
            return new BHoMGroup<T>
            {
                Elements = elements.ToList()
            };
        }

        /***************************************************/

        public static BHoMObject BHoMGroup(List<IBHoMObject> elements, Type typeHint = null, string name = "")
        {
            Type type;

            if (typeHint == null)
            {
                if(elements.Count == 0)
                    return new BHoMGroup<IBHoMObject>();

                type = elements[0].GetType();

                for (int i = 1; i < elements.Count; i++)
                {
                    if (elements[i].GetType() == type)
                        continue;
                    else
                    {
                        type = typeof(IBHoMObject);
                        break;
                    }
                }
            }
            else
                type = typeHint;

            var groupType = typeof(BHoMGroup<>).MakeGenericType(new Type[] { type });
            var group = Activator.CreateInstance(groupType);

            PropertyInfo info = groupType.GetProperty("Elements");
            var list = info.GetValue(group);
            var add = list.GetType().GetMethod("Add");

            foreach (IBHoMObject obj in elements)
            {
                add.Invoke(list, new object[] { obj });
            }

            return group as BHoMObject;
        }

        /***************************************************/
    }
}
