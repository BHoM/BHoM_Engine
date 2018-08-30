using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using BH.oM.Structure.Loads;
using BH.Engine.Geometry;

using System.Collections.Generic;
using System.Linq;
using System;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> PointGrid(this MeshFace face)
        {
            List<Point> pts = new List<Point>();

            for (int i = 0; i < face.Nodes.Count; i++)
            {
                pts.Add(face.Nodes[i].Position);

                int nextId = i < face.Nodes.Count - 1 ? i + 1 : 0;

                pts.Add((face.Nodes[i].Position + face.Nodes[nextId].Position) / 2);
            }

            pts.Add(pts.Average());

            return pts;
        }

        /***************************************************/

        public static List<Point> PointGrid(this PanelPlanar panel)
        {
            List<ICurve> curves = panel.ExternalEdgeCurves();

            List<PolyCurve> joined = curves.IJoin();

            Plane plane = joined.First().FitPlane();

            Vector z = Vector.ZAxis;

            double angle = plane.Normal.Angle(z);

            Vector axis = plane.Normal.CrossProduct(z);

            TransformMatrix matrix = Engine.Geometry.Create.RotationMatrix(Point.Origin, axis, angle);

            List<PolyCurve> rotated = joined.Select(x => x.Rotate(Point.Origin, axis,angle)).ToList();

            BoundingBox bounds = new BoundingBox();

            foreach (PolyCurve crv in rotated)
            {
                bounds += crv.Bounds();
            }

            double xMin = bounds.Min.X;
            double yMin = bounds.Min.Y;
            double zVal = bounds.Min.Z;

            int steps = 4;

            double xStep = (bounds.Max.X - xMin) / steps;
            double yStep = (bounds.Max.Y - yMin) / steps;

            List<Point> pts = new List<Point>();
            TransformMatrix transpose = matrix.Transpose();

            for (int i = 0; i < steps; i++)
            {
                double x = xMin + xStep * i;
                for (int j = 0; j < steps; j++)
                {
                    Point pt = new Point { X = x, Y = yMin + yStep * j, Z = zVal };
                    bool isInside = false;

                    pt = pt.Transform(transpose);

                    foreach (PolyCurve crv in joined)
                    {
                        if (isInside = crv.IsContaining(new List<Point> { pt }))
                            break;
                    }
                    if (isInside)
                        pts.Add(pt);
                }
            }


            //pts = pts.Select(x => x.Transform(transpose)).ToList();

            return pts;
        }

        /***************************************************/
        /**** Public Methods Interface                  ****/
        /***************************************************/

        public static List<Point> IPointGrid(this IAreaElement element)
        {
            return PointGrid(element as dynamic);
        }

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private static bool SimpleIsContaining(PolyCurve crv, Plane plane, Point pt)
        {
            Point end = pt.Translate(Engine.Geometry.Create.RandomVectorInPlane(plane, true));
            Line ray = new Line { Start = pt, End = end };
            ray.Infinite = true;

            List<Point> interPts = crv.Curves.SelectMany(x => x.ILineIntersections(ray, true)).ToList();

            return interPts.Count % 2 != 0;
        }

        /***************************************************/
    }

}
