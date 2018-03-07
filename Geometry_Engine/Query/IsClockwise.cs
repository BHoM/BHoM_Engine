using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsClockwise(this Polyline polyline, Vector normal)
        {
            if (!polyline.IsClosed()) throw new Exception("The polyline is not closed. IsClockwise method is relevant only to closed curves.");
            List<Point> cc = polyline.DiscontinuityPoints();
            Vector dir1 = (cc[0] - cc.Last()).Normalise();
            Vector dir2;
            double angleTot = 0;
            for (int i = 1; i < cc.Count; i++)
            {
                dir2 = (cc[i] - cc[i - 1]).Normalise();
                angleTot += dir1.SignedAngle(dir2, normal);
                dir1 = dir2.Clone();
            }
            return angleTot > 0;
        }

        /***************************************************/

        public static bool IsClockwise(this PolyCurve curve, Vector normal)
        {
            if (!curve.IsClosed()) throw new Exception("The curve is not closed. IsClockwise method is relevant only to closed curves.");
            List<Point> cc = curve.ControlPoints().CullDuplicates();
            Vector dir1 = (cc[0] - cc.Last()).Normalise();
            Vector dir2;
            double angleTot = 0;
            for (int i = 1; i < cc.Count; i++)
            {
                dir2 = (cc[i] - cc[i - 1]).Normalise();
                angleTot += dir1.SignedAngle(dir2, normal);
                dir1 = dir2.Clone();
            }
            return angleTot > 0;
        }

        /***************************************************/

        public static bool IsClockwise(this Polyline polyline, Point viewPoint)
        {
            Plane plane = polyline.FitPlane();

            Point projectedPoint = viewPoint.Project(plane);
            Vector vector = (projectedPoint - viewPoint).Normalise();

            return IsClockwise(polyline, vector);
        }

        /***************************************************/

        public static bool IsClockwise(this PolyCurve curve, Point viewPoint)
        {
            Plane plane = curve.FitPlane();

            Point projectedPoint = viewPoint.Project(plane);
            Vector vector = (projectedPoint - viewPoint).Normalise();

            return IsClockwise(curve, vector);
        }


         /***************************************************/
    }
}
