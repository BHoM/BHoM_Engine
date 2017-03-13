using BHoM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
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
                double angle = VectorUtils.VectorAngle(BHoM.Geometry.Vector.ZAxis(), axis);

                Transform t1 = Geometry.Transform.Rotation(centre, axis, angle);
                Transform t2 = Geometry.Transform.Translation(BHoM.Geometry.Point.Origin - centre) * t1;

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
    }


    public static class XSurface
    {
        internal static void Transform(Surface surface, Transform t)
        {
            XBrep.Transform(surface, t);
            surface.ControlPointVector = ArrayUtils.MultiplyMany(t, surface.ControlPointVector);
            surface.Update();
        }

        internal static void Translate(Surface surface, Vector v)
        {
            XBrep.Translate(surface, v);
            surface.ControlPointVector = ArrayUtils.Add(surface.ControlPointVector, v);
            surface.Update();
        }

        internal static void Mirror(Surface surface, Plane p)
        {
            XBrep.Mirror(surface, p);
            surface.ControlPointVector = ArrayUtils.Add(ArrayUtils.Multiply(p.ProjectionVectors(surface.ControlPointVector), 2), surface.ControlPointVector);
            surface.Update();
        }

        internal static void Project(Surface surface, Plane p)
        {
            XBrep.Project(surface, p);
            surface.ControlPointVector = ArrayUtils.Add(p.ProjectionVectors(surface.ControlPointVector), surface.ControlPointVector);
            surface.Update();
        }
    }
}
