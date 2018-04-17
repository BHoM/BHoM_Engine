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

        public static List<List<Line>> ClusterCollinear(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            List<List<Line>> lineClusters = new List<List<Line>>();
            foreach (Line l in lines)
            {
                bool collinear = false;
                foreach (List<Line> ll in lineClusters)
                {
                    if (l.IsCollinear(ll[0], tolerance))
                    {
                        ll.Add(l.Clone());
                        collinear = true;
                        break;
                    }
                }
                if (!collinear) lineClusters.Add(new List<Line> { l.Clone() });
            }
            return lineClusters;
        }
    }
}
