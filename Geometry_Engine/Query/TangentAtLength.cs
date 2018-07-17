using System;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Vector TangentAtLength(this Arc curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtLength(this Circle curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtLength(this Line curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtLength(this NurbCurve curve, double length, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Vector TangentAtLength(this PolyCurve curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtLength(this Polyline curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector ITangentAtLength(this ICurve curve, double length, double tolerance = Tolerance.Distance)
        {
            return TangentAtLength(curve as dynamic, length, tolerance);
        }

        /***************************************************/
    }
}
