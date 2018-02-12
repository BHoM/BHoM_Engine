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

        public static bool IsLinear(this Line line)
        {
            return true;
        }

        /***************************************************/

        public static bool IsLinear(this Arc arc)
        {
            return arc.ControlPoints().IsCollinear();
        }

        /***************************************************/

        public static bool IsLinear(this Circle circle)
        {
            return circle.Radius == 0;
        }

        /***************************************************/

        public static bool IsLinear(this NurbCurve curve)
        {
            return curve.ControlPoints.IsCollinear();
        }

        /***************************************************/

        public static bool IsLinear(this Polyline curve)
        {
            return curve.ControlPoints.IsCollinear();
        }

        /***************************************************/

        public static bool IsLinear(this PolyCurve curve)
        {
            return curve.ControlPoints().IsCollinear();
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
