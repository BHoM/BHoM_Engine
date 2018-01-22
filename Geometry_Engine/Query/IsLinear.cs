using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsColinear(this List<Point> pts)
        {
            if (pts.Count < 3) return true;

            double[,] vMatrix = new double[pts.Count, 3];
            for (int i = 0; i < pts.Count; i++)
            {
                vMatrix[i, 0] = pts[i].X;
                vMatrix[i, 1] = pts[i].Y;
                vMatrix[i, 2] = pts[i].Z;
            }

            double[,] rref = vMatrix.RowEchelonForm(false);
            int nonZeroRows = rref.CountNonZeroRows();
            return nonZeroRows < 2;
        }

        /***************************************************/

        public static bool IsLinear(this Line line)
        {
            return true;
        }

        /***************************************************/

        public static bool IsLinear(this Arc arc)
        {
            return arc.ControlPoints().IsColinear();
        }

        /***************************************************/

        public static bool IsLinear(this Circle circle)
        {
            return circle.Radius == 0;
        }

        /***************************************************/

        public static bool IsLinear(this NurbCurve curve)
        {
            return curve.ControlPoints.IsColinear();
        }

        /***************************************************/

        public static bool IsLinear(this Polyline curve)
        {
            return curve.ControlPoints.IsColinear();
        }

        /***************************************************/

        public static bool IsLinear(this PolyCurve curve)
        {
            return curve.ControlPoints().IsColinear();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsLinear(this ICurve curve)
        {
            return IsLinear(curve as dynamic);
        }

        /***************************************************/
    }
}
