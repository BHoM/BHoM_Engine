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

        public static bool IsCollinear(this List<Point> pts, double tolerance = Tolerance.Distance)
        {
            if (pts.Count < 3)
                return true;

            double[,] vMatrix = new double[pts.Count - 1, 3];
            for (int i = 0; i < pts.Count - 1; i++)
            {
                vMatrix[i, 0] = pts[i + 1].X - pts[0].X;
                vMatrix[i, 1] = pts[i + 1].Y - pts[0].Y;
                vMatrix[i, 2] = pts[i + 1].Z - pts[0].Z;
            }

            double REFTolerance = vMatrix.REFTolerance(tolerance);
            double[,] rref = vMatrix.RowEchelonForm(false, REFTolerance);
            int nonZeroRows = rref.CountNonZeroRows(REFTolerance);
            return nonZeroRows < 2;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsCollinear(this Line line1, Line line2, double tolerance = Tolerance.Distance)
        {
            List<Point> cPts = new List<Point> { line1.Start, line1.End, line2.Start, line2.End };
            return cPts.IsCollinear(tolerance);
        }

        /***************************************************/
    }
}
