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

        public static bool IsCoplanar(this List<Point> pts)
        {
            if (pts.Count < 4) return true;

            double[,] vMatrix = new double[pts.Count - 1, 3];
            for (int i = 0; i < pts.Count - 1; i++)
            {
                vMatrix[i, 0] = pts[i + 1].X - pts[0].X;
                vMatrix[i, 1] = pts[i + 1].Y - pts[0].Y;
                vMatrix[i, 2] = pts[i + 1].Z - pts[0].Z;
            }

            double[,] rref = vMatrix.RowEchelonForm(false);
            int nonZeroRows = rref.CountNonZeroRows();
            return nonZeroRows < 3;
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
