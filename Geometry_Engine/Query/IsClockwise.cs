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
            List<Point> pts = DiscontinuityPoints(polyCurve).CullDuplicates();
            Vector centreVector = (pts[0] - viewPoint).Normalise();


            /* Dot product of the normal and a vector from the center of the space. Positive dotproduct for clockwise
               and negative for anticlockwise (but this depends on the handedness of the coordinate system)*/
            List<double> dotProducts = new List<double>();
            for (int i = 0; i < pts.Count - 2; i++)
            {
                Plane plane = Create.Plane(pts[i], pts[i + 1], pts[i + 2]);
                dotProducts.Add(plane.Normal.DotProduct(centreVector));
            }

            List<double> pos = new List<double>();
            List<double> neg = new List<double>();
            pos.AddRange(dotProducts.Where(x => x > 0));
            neg.AddRange(dotProducts.Where(x => x < 0));

            //Since we can have local concavity in a global convex curve we have to check all the normal vectors.
            //More positive than negative dotproducts => clockwise curve.
            if (pos.Count > neg.Count)
                return true;
            return false;


        }

        /***************************************************/
    }
}
