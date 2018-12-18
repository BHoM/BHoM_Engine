using System;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Vector TangentAtPoint(this Arc curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtPoint(this Circle curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtPoint(this Line curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        [NotImplemented]
        public static Vector TangentAtPoint(this NurbsCurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Vector TangentAtPoint(this PolyCurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtPoint(this Polyline curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector ITangentAtPoint(this ICurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            return Query.TangentAtPoint(curve as dynamic, point, tolerance);
        }

        /***************************************************/
    }
}
