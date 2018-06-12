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

        public static bool IsLinear(this Line line, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsLinear(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsLinear(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsLinear(this NurbCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsCollinear(tolerance);
        }

        /***************************************************/

        public static bool IsLinear(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsCollinear(tolerance);
        }

        /***************************************************/

        public static bool IsLinear(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints().IsCollinear(tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsLinear(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return IsLinear(curve as dynamic, tolerance);
        }

        /***************************************************/
    }
}
