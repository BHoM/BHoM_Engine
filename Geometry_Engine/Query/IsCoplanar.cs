using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vector                   ****/
        /***************************************************/

        public static bool IsCoplanar(this List<Point> points)
        {
            if (points.Count < 4) return true;

            double[,] vMatrix = new double[points.Count - 1, 3];
            for (int i = 0; i < points.Count - 1; i++)
            {
                vMatrix[i, 0] = points[i + 1].X - points[0].X;
                vMatrix[i, 1] = points[i + 1].Y - points[0].Y;
                vMatrix[i, 2] = points[i + 1].Z - points[0].Z;
            }

            double[,] rref = vMatrix.RowEchelonForm(false);
            int nonZeroRows = rref.CountNonZeroRows();
            return nonZeroRows < 3;
        }

        /***************************************************/

        public static bool IsCoplanar(this Plane plane1, Plane plane2)
        {
            return (Math.Abs(plane1.Origin.Distance(plane2)) <= Tolerance.Distance && plane1.Normal.IsParallel(plane2.Normal) != 0);
        }

        /***************************************************/

        public static bool IsCoplanar(this List<Plane> planes)
        {
            for (int i = 1; i < planes.Count; i++)
            {
                if (!planes[0].IsCoplanar(planes[i])) return false;
            }
            return true;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsCoplanar(this Line line1, Line line2)
        {
            List<Point> cPts = new List<Point> { line1.Start, line1.End, line2.Start, line2.End };
            return cPts.IsCoplanar();
        }

        /***************************************************/

        public static bool IsCoplanar(this Polyline curve1, Polyline curve2)
        {
            List<Point> cPts = curve1.Clone().ControlPoints;
            cPts.AddRange(curve2.Clone().ControlPoints);
            return cPts.IsCoplanar();
        }

        /***************************************************/
    }
}
