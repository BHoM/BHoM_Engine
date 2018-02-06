using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/

        public static List<List<Line>> ClusterCollinear(this List<Line> lines)
        {
            List<List<Line>> lineClusters = new List<List<Line>>();
            foreach (Line l in lines)
            {
                bool colinear = false;
                foreach (List<Line> ll in lineClusters)
                {
                    if (l.IsColinear(ll[0]))
                    {
                        ll.Add(l.Clone());
                        colinear = true;
                        break;
                    }
                }
                if (!colinear) lineClusters.Add(new List<Line> { l.Clone() });
            }
            return lineClusters;
        }
    }
}
