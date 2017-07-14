using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Clone
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry GetClone(this IBHoMGeometry geometry)
        {
            return _GetClone(geometry as dynamic);
        }

        /***************************************************/

        public static Face GetClone(this Face face)
        {
            return new Face(face.A, face.B, face.C, face.D);
        }

        /***************************************************/

        public static BoundingBox GetClone(this BoundingBox box)
        {
            return new BoundingBox(box.Min._GetClone(), box.Max._GetClone());
        }


        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        private static Plane _GetClone(this Plane plane)
        {
            return new Plane(plane.Origin._GetClone(), plane.Normal._GetClone());
        }

        /***************************************************/

        private static Point _GetClone(this Point point)
        {
            return new Point(point.X, point.Y, point.Z);
        }

        /***************************************************/

        private static Vector _GetClone(this Vector vector)
        {
            return new Vector(vector.X, vector.Y, vector.Z);
        }


        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        private static Arc _GetClone(this Arc arc)
        {
            return new Arc(arc.Start._GetClone(), arc.End._GetClone(), arc.Middle._GetClone());
        }

        /***************************************************/

        private static Circle _GetClone(this Circle circle)
        {
            return new Circle(circle.Centre._GetClone(), circle.Normal._GetClone(), circle.Radius);
        }

        /***************************************************/

        private static Line _GetClone(this Line line)
        {
            return new Line(line.Start._GetClone(), line.End._GetClone() );
        }

        /***************************************************/

        private static NurbCurve _GetClone(this NurbCurve curve)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x._GetClone()), curve.Weights, curve.Knots);
        }

        /***************************************************/

        private static PolyCurve _GetClone(this PolyCurve curve)
        {
            return new PolyCurve(curve.Curves.Select(x => x.GetClone() as ICurve));
        }

        /***************************************************/

        private static Polyline _GetClone(this Polyline curve)
        {
            return new Polyline(curve.ControlPoints.Select(x => x._GetClone()));
        }


        /***************************************************/
        /**** Private Methods - Surfaces                ****/
        /***************************************************/

        private static Extrusion _GetClone(this Extrusion surface)
        {
            return new Extrusion(surface.Curve.GetClone() as ICurve, surface.Direction._GetClone(), surface.Capped);
        }

        /***************************************************/

        private static Loft _GetClone(this Loft surface)
        {
            return new Loft(surface.Curves.Select(x => x.GetClone() as ICurve));
        }

        /***************************************************/

        private static NurbSurface _GetClone(this NurbSurface surface)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x._GetClone()), surface.Weights.ToList(), surface.UKnots.ToList(), surface.VKnots.ToList());
        }

        /***************************************************/

        private static Pipe _GetClone(this Pipe surface)
        {
            return new Pipe(surface.Centreline.GetClone() as ICurve, surface.Radius, surface.Capped);
        }

        /***************************************************/

        private static PolySurface _GetClone(this PolySurface surface)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.GetClone() as ISurface));
        }


        /***************************************************/
        /**** Private Methods - Others                  ****/
        /***************************************************/

        private static Mesh _GetClone(Mesh mesh)
        {
            return new Mesh(mesh.Vertices.Select(x => x._GetClone()), mesh.Faces.Select(x => x.GetClone()));
        }

    }
}
