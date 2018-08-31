using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Curves                                    ****/
        /***************************************************/

        public static Point Centre(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return arc.CoordinateSystem.Origin;
        }

        /***************************************************/

        public static Point Centre(this Polyline polyline, double tolerance = Tolerance.Distance)
        {
            //TODO: this is an average point, not centroid - should be distinguished

            if (!polyline.IsClosed(tolerance)) return polyline.ControlPoints.Average(); // TODO: not true for a self-intersecting polyline?
            else return polyline.ControlPoints.GetRange(0, polyline.ControlPoints.Count - 1).Average();
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

        public static List<Point> Centres(this Mesh mesh)
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
