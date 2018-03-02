using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsClockwise(this Polyline curve, Vector normal)
        {
            List<Point> cc = curve.ControlPoints;
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

        public static bool IsClockwise(this PolyCurve polyCurve, Point viewPoint)
        {
            List<Point> pts = DiscontinuityPoints(polyCurve);
            Plane plane = Create.Plane(pts[0], pts[1], pts[2]);

            /* Dot product of the normal and a vector from the center of the space. Positive dotproduct for clockwise
             * and negative for anticlockwise (but this depends on the handedness of the coordinate system)*/
            Vector centreVector = (pts[0] - viewPoint).Normalise();
            double dotProduct = plane.Normal * centreVector;
            if (dotProduct < 0)
                return false;

            return true;
        }

        /***************************************************/
    }
}
