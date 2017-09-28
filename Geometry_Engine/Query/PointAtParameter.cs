using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point GetPointAtParameter(this ICurve curve, double t)
        {
            if (t > 1)
            {
                throw new ArgumentOutOfRangeException("Parameter must be less than the t of the curve"); // Turn into warning
            }
            return _GetPointAtParameter(curve as dynamic, t);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Point _GetPointAtParameter(this Arc curve, double t)
        {
            return GetPointAtLength(curve, t * curve.GetLength());
        }

        /***************************************************/

        private static Point _GetPointAtParameter(this Circle curve, double t)
        {
            return GetPointAtLength(curve, t * curve.GetLength());
        }

        /***************************************************/

        private static Point _GetPointAtParameter(this Line curve, double t)
        {
            Vector vector = curve.End - curve.Start;
            return (new Point(vector * t) + curve.Start);
        }

        /***************************************************/

        private static Point _GetPointAtParameter(this NurbCurve curve, double t)
        {
            throw new NotImplementedException(); // TODO NurbCurve.GetPointAtParameter()
        }

        /***************************************************/

        private static Point _GetPointAtParameter(this PolyCurve curve, double t)
        {
            throw new NotImplementedException(); // TODO Polycurve.GetPointAtParameter() Relies on NurbCurve PointAt method
        }

        /***************************************************/

        private static Point _GetPointAtParameter(this Polyline curve, double t)
        {
            return GetPointAtLength(curve, t * curve.GetLength());
        }
    }
}
