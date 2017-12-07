using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Vectors                                   ****/
        /***************************************************/

        public static Point GetCentre(this IEnumerable<Point> points)
        {
            int count = points.Count();
            if (count < 1) return null;

            Point mean = new Point(0, 0, 0);

            foreach (Point pt in points)
                mean += pt;

            return mean /= count;
        }

        /***************************************************/
        /**** Curves                                    ****/
        /***************************************************/

        public static Point GetCentre(this Arc arc)
        {
            Vector v1 = arc.Start - arc.Middle;
            Vector v2 = arc.End - arc.Middle;
            Vector normal = v1.GetCrossProduct(v2);

            return Query.GetIntersection(
                new Line(arc.Middle + v1 / 2, v1.GetCrossProduct(normal)),
                new Line(arc.Middle + v2 / 2, v2.GetCrossProduct(normal))
            );
        }

        /***************************************************/

        public static Point GetCentre(this Polyline polyline)
        {
            return polyline.ControlPoints.GetCentre();
        }


        /***************************************************/
        /**** Surfaces                                    ****/
        /***************************************************/

        public static Point GetCentre(this BoundingBox box)
        {
            return new Point((box.Max.X + box.Min.X) / 2, (box.Max.Y + box.Min.Y) / 2, (box.Max.Z + box.Min.Z) / 2);
        }


        /***************************************************/
        /**** Mesh                                      ****/
        /***************************************************/

        public static List<Point> GetCentres(this Mesh mesh)
        {
            List<Face> faces = mesh.Faces;
            List<Point> vertices = mesh.Vertices;
            List<Point> centres = new List<Point>(faces.Count);
            for (int i = 0; i < faces.Count; i++)
            {
                Point pA = vertices[(faces[i].A)];
                Point pB = vertices[(faces[i].B)];
                Point pC = vertices[(faces[i].C)];
                if (!faces[i].IsQuad())
                {
                    centres.Add(new Point((pA.X + pB.X + pC.X) / 3, (pA.Y + pB.Y + pC.Y) / 3, (pA.Z + pB.Z + pC.Z) / 3));
                }
                else
                {
                    Point p4 = vertices[(faces[i].D)];
                    centres.Add(new Point((pA.X + pB.X + pC.X + p4.X) / 4, (pA.X + pB.X + pC.X + p4.Y) / 4, (pA.X + pB.X + pC.X + p4.Z) / 4));  // Assumption that if the face is quad, it is a flat quad.
                }
            }
            return centres;
        }
    }
}
