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
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Point GetPointAtParameter(this Arc curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            return GetPointAtLength(curve, t * curve.GetLength());
        }

        /***************************************************/

        public static Point GetPointAtParameter(this Circle curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            return GetPointAtLength(curve, t * curve.GetLength());
        }

        /***************************************************/

        public static Point GetPointAtParameter(this Line curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            Vector vector = curve.End - curve.Start;
            return (new Point(vector * t) + curve.Start);
        }

        /***************************************************/

        public static Point GetPointAtParameter(this NurbCurve curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            throw new NotImplementedException(); // TODO NurbCurve.GetPointAtParameter()
        }

        /***************************************************/

        public static Point GetPointAtParameter(this PolyCurve curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            throw new NotImplementedException(); // TODO Polycurve.GetPointAtParameter() Relies on NurbCurve PointAt method
        }

        /***************************************************/

        public static Point GetPointAtParameter(this Polyline curve, double t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            return GetPointAtLength(curve, t * curve.GetLength());
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point IGetPointAtParameter(this ICurve curve, double t)
        {
            return GetPointAtParameter(curve as dynamic, t);
        }
    }
}
