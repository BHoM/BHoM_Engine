using BH.oM.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static VennDiagram<T> VennDiagram<T>(IEnumerable<T> set1, IEnumerable<T> set2, IEqualityComparer<T> comparer) //where T : BH.oM.Base.IBHoMObject
        {
            VennDiagram<T> diagram = new VennDiagram<T>();

            foreach (T a in set1)
            {
                bool found = false;
                foreach (T b in set2)
                {
                    //Check if object exists
                    if (comparer.Equals(a, b))
                    {
                        diagram.Intersection.Add(new Tuple<T,T>(a, b));
                        found = true;
                        break;
                    }
                }
                if (!found)
                    diagram.OnlySet1.Add(a);
            }

            diagram.OnlySet2 = set2.Except(diagram.Intersection.Select(x => x.Item2)).ToList();

            return diagram;
        }

        /***************************************************/
    }
}
