using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static partial class Create
    {
        private static Surface SurfaceFrom4Points(Point p1, Point p2, Point p3, Point p4)
        {
            Surface surface = new Surface();

            List<Curve> edges = new List<Curve>();
            edges.Add(new Line(p1, p2));
            edges.Add(new Line(p2, p3));
            edges.Add(new Line(p3, p4));
            edges.Add(new Line(p4, p1));
            surface.TrimmingCurves = new Group<Curve>(CurveUtils.Join(edges));

            Curve c = surface.TrimmingCurves[0];

            surface.PointColumns = 2;
            surface.ControlPointVector = new double[4 * (surface.Dimensions + 1)];

            double[] row1 = CollectionUtils.SubArray<double>(c.ControlPointVector, 0, (surface.Dimensions + 1) * 2);
            double[] row2 = CollectionUtils.Reverse<double>(CollectionUtils.SubArray<double>(c.ControlPointVector, (surface.Dimensions + 1) * 2, (surface.Dimensions + 1) * 2), surface.Dimensions + 1);

            surface.ControlPointVector = CollectionUtils.Merge<double>(row1, row2);

            surface.Weights = new double[] { 1, 1, 1, 1 };

            surface.uKnots = new double[] { 0, 0, 1, 1 };
            surface.vKnots = new double[] { 0, 0, 1, 1 };

            return surface;
        }


        public static Surface SurfaceFromBoundary(Curve boundary)
        {
            if (boundary.IsPlanar() && boundary.IsClosed())
            {
                //surface.m_NakedEdges = new Group<Curve>(new List<Curve>() { boundary });

                Curve xY = boundary.DuplicateCurve();
                Plane plane = null;
                xY.TryGetPlane(out plane);

                Point centre = xY.Bounds().Centre;
                Vector axis = plane.Normal;
                double angle = VectorUtils.VectorAngle(BH.oM.Geometry.Vector.ZAxis(), axis);

                Transform t1 = Geometry.Transform.Rotation(centre, axis, angle);
                Transform t2 = Geometry.Transform.Translation(BH.oM.Geometry.Point.Origin - centre) * t1;

                xY.Transform(t2);
                Vector extents = xY.Bounds().Extents;

                Point p1 = new Point(extents.X, -extents.Y, 0);
                Point p2 = new Point(extents.X, extents.Y, 0);
                Point p3 = new Point(-extents.X, -extents.Y, 0);
                Point p4 = new Point(-extents.X, extents.Y, 0);

                Surface surface = SurfaceFrom4Points(p1, p2, p3, p4);

                surface.Transform(t2.Inverse());
                surface.TrimmingCurves.Add(boundary);
                return surface;
            }
            return null;
        }

        public static Surface SurfaceFromBoundaryCurves(Group<Curve> boundaryCurves)
        {
            List<Curve> temp = null;
            return SurfaceFromBoundaryCurves(boundaryCurves, out temp);
        }

        public static Surface SurfaceFromBoundaryCurves(Group<Curve> boundaryCurves, out List<Curve> unusedCurves)
        {
            List<Curve> curves = CurveUtils.Join(boundaryCurves);
            Plane plane = null;
            Group<Curve> closedCurves = new Group<Curve>();
            unusedCurves = new List<Curve>();
            foreach (Curve boundary in boundaryCurves)
            {
                if (boundary.IsPlanar() && boundary.IsClosed())
                {
                    Curve xY = boundary.DuplicateCurve();
                    if (plane == null && xY.TryGetPlane(out plane))
                    {
                        closedCurves.Add(xY);
                    }
                    else if (plane != null && plane.InPlane(xY.ControlPointVector, xY.Dimensions + 1, 0.001))
                    {
                        closedCurves.Add(xY);
                    }
                    else unusedCurves.Add(xY);
                }
                else
                {
                    unusedCurves.Add(boundary);
                }
            }
            if (plane != null)
            { 
                Point centre = closedCurves.Bounds().Centre;
                Vector axis = plane.Normal;
                double angle = VectorUtils.VectorAngle(BH.oM.Geometry.Vector.ZAxis(), axis);

                Transform t1 = Geometry.Transform.Rotation(centre, axis, angle);
                Transform t2 = Geometry.Transform.Translation(BH.oM.Geometry.Point.Origin - centre) * t1;

                closedCurves.Transform(t2);
                Vector extents = closedCurves.Bounds().Extents;

                Point p1 = new Point(extents.X, -extents.Y, 0);
                Point p2 = new Point(extents.X, extents.Y, 0);
                Point p3 = new Point(-extents.X, -extents.Y, 0);
                Point p4 = new Point(-extents.X, extents.Y, 0);

                Surface surface = SurfaceFrom4Points(p1, p2, p3, p4);

                surface.Transform(t2.Inverse());
                surface.TrimmingCurves.AddRange(closedCurves);
                return surface;
            }
            return null;
        }
    }
}
