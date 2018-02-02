using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static List<Point> SortCollinear(this List<Point> points)
        {
            List<Point> cpoints = points.Select(p => p.Clone()).ToList();
            for (int i = 1; i < cpoints.Count; i++)
            {
                if (Math.Abs(cpoints[0].X - cpoints[i].X) > Tolerance.Distance)
                {
                    cpoints.Sort(delegate (Point p1, Point p2)
                        {
                            return p1.X.CompareTo(p2.X);
                        });
                    break;
                }
                else if (Math.Abs(cpoints[0].Y - cpoints[i].Y) > Tolerance.Distance)
                {
                    cpoints.Sort(delegate (Point p1, Point p2)
                    {
                        return p1.Y.CompareTo(p2.Y);
                    });
                    break;
                }
                else if(Math.Abs(cpoints[0].Z - cpoints[i].Z) > Tolerance.Distance)
                {
                    cpoints.Sort(delegate (Point p1, Point p2)
                    {
                        return p1.Z.CompareTo(p2.Z);
                    });
                    break;
                }
            }
            return cpoints;
        }
    }
}
