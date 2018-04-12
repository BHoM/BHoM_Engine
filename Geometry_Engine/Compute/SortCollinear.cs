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

        public static List<Point> SortCollinear(this List<Point> points, double tolerance = Tolerance.Distance)
        {
            List<Point> cPoints = points.Select(p => p.Clone()).ToList();
            for (int i = 1; i < cPoints.Count; i++)
            {
                if (Math.Abs(cPoints[0].X - cPoints[i].X) > tolerance)
                {
                    cPoints.Sort(delegate (Point p1, Point p2)
                        {
                            return p1.X.CompareTo(p2.X);
                        });
                    break;
                }
                else if (Math.Abs(cPoints[0].Y - cPoints[i].Y) > tolerance)
                {
                    cPoints.Sort(delegate (Point p1, Point p2)
                    {
                        return p1.Y.CompareTo(p2.Y);
                    });
                    break;
                }
                else if(Math.Abs(cPoints[0].Z - cPoints[i].Z) > tolerance)
                {
                    cPoints.Sort(delegate (Point p1, Point p2)
                    {
                        return p1.Z.CompareTo(p2.Z);
                    });
                    break;
                }
            }
            return cPoints;
        }
    }
}
