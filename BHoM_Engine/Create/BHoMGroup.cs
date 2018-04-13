using BH.oM.Base;
using System.Collections.Generic;
using System.Collections;
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

        public static IBHoMGroup BHoMGroup<T>(IEnumerable<T> elements, bool downCast = true, string name = "") where T : IBHoMObject
        {
            if (downCast)
            {
                List<T> elementList = elements.ToList();

                if (elementList.Count > 0)
                {
                    Type type = elementList[0].GetType();

                    if (type != typeof(T)) //if type is same as T no downcasting using reflection needed
                    {
                        bool sameType = true;

                        for (int i = 1; i < elementList.Count; i++)
                        {
                            if (elementList[i].GetType() != type)
                            {
                                sameType = false;
                                break;
                            }
                        }

                        if (sameType)
                            return BHoMGroup(elements, type, name);
                    }
                }
            }

            return new BHoMGroup<T>
            {
                Elements = elements.ToList(),
                Name = name
            };
        
        }

        /***************************************************/

        public static IBHoMGroup BHoMGroup<T>(IEnumerable<T> elements, Type type, string name = "") where T: IBHoMObject
        {

            var groupType = typeof(BHoMGroup<>).MakeGenericType(new Type[] { type });
            IBHoMGroup group = Activator.CreateInstance(groupType) as IBHoMGroup;

            PropertyInfo info = groupType.GetProperty("Elements");
            IList list = info.GetValue(group) as IList;

            foreach (IBHoMObject obj in elements)
            {
                list.Add(obj);
            }

            group.Name = name;

            return group as IBHoMGroup;
        }

        /***************************************************/
    }
}
