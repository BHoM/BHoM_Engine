using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Vectors                                   ****/
        /***************************************************/

        public static Point Centre(this IEnumerable<Point> points)
        {
            int count = points.Count();
            if (count < 1) return null;

            Point mean = new Point { X = 0, Y = 0, Z = 0 };

            foreach (Point pt in points)
                mean += pt;

            return mean /= count;
        }

        /***************************************************/
        /**** Curves                                    ****/
        /***************************************************/

        public static Point Centre(this Arc arc)
        {
            Vector v1 = arc.Start - arc.Middle;
            Vector v2 = arc.End - arc.Middle;
            Vector normal = v1.CrossProduct(v2);

            return Query.LineIntersection(
                Create.Line(arc.Middle + v1 / 2, v1.CrossProduct(normal)),
                Create.Line(arc.Middle + v2 / 2, v2.CrossProduct(normal))
            );
        }

        /***************************************************/

        public static Point Centre(this Polyline polyline)
        {
            return polyline.ControlPoints.Centre();
        }


        /***************************************************/
        /**** Surfaces                                    ****/
        /***************************************************/

        public static Point Centre(this BoundingBox box)
        {
            return new Point { X = (box.Max.X + box.Min.X) / 2, Y = (box.Max.Y + box.Min.Y) / 2, Z = (box.Max.Z + box.Min.Z) / 2 };
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
                    centres.Add(new Point { X = (pA.X + pB.X + pC.X) / 3, Y = (pA.Y + pB.Y + pC.Y) / 3, Z = (pA.Z + pB.Z + pC.Z) / 3 });
                }
                else
                {
                    Point p4 = vertices[(faces[i].D)];
                    centres.Add(new Point { X = (pA.X + pB.X + pC.X + p4.X) / 4, Y = (pA.X + pB.X + pC.X + p4.Y) / 4, Z = (pA.X + pB.X + pC.X + p4.Z) / 4 });  // Assumption that if the face is quad, it is a flat quad.
                }
            }
            return centres;
        }

        /***************************************************/
    }
}
