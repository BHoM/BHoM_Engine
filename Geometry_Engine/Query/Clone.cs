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
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Plane GetClone(this Plane plane)
        {
            return new Plane(plane.Origin.GetClone(), plane.Normal.GetClone());
        }

        /***************************************************/

        public static Point GetClone(this Point point)
        {
            return new Point(point.X, point.Y, point.Z);
        }

        /***************************************************/

        public static Vector GetClone(this Vector vector)
        {
            return new Vector(vector.X, vector.Y, vector.Z);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc GetClone(this Arc arc)
        {
            return new Arc(arc.Start.GetClone(), arc.Middle.GetClone(), arc.End.GetClone());
        }

        /***************************************************/

        public static Circle GetClone(this Circle circle)
        {
            return new Circle(circle.Centre.GetClone(), circle.Normal.GetClone(), circle.Radius);
        }

        /***************************************************/

        public static Line GetClone(this Line line)
        {
            return new Line(line.Start.GetClone(), line.End.GetClone() );
        }

        /***************************************************/

        public static NurbCurve GetClone(this NurbCurve curve)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x.GetClone()), curve.Weights, curve.Knots);
        }

        /***************************************************/

        public static PolyCurve GetClone(this PolyCurve curve)
        {
            return new PolyCurve(curve.Curves.Select(x => x._GetClone()));
        }

        /***************************************************/

        public static Polyline GetClone(this Polyline curve)
        {
            return new Polyline(curve.ControlPoints.Select(x => x.GetClone()));
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion GetClone(this Extrusion surface)
        {
            return new Extrusion(surface.Curve._GetClone(), surface.Direction.GetClone(), surface.Capped);
        }

        /***************************************************/

        public static Loft GetClone(this Loft surface)
        {
            return new Loft(surface.Curves.Select(x => x._GetClone()));
        }

        /***************************************************/

        public static NurbSurface GetClone(this NurbSurface surface)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x.GetClone()), surface.Weights.ToList(), surface.UKnots.ToList(), surface.VKnots.ToList());
        }

        /***************************************************/

        public static Pipe GetClone(this Pipe surface)
        {
            return new Pipe(surface.Centreline._GetClone(), surface.Radius, surface.Capped);
        }

        /***************************************************/

        public static PolySurface GetClone(this PolySurface surface)
        {
            return new PolySurface(surface.Surfaces.Select(x => x._GetClone()));
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh GetClone(this Mesh mesh)
        {
            return new Mesh(mesh.Vertices.Select(x => x.GetClone()), mesh.Faces.Select(x => x.GetClone()));
        }

        /***************************************************/

        public static Face GetClone(this Face face)
        {
            return new Face(face.A, face.B, face.C, face.D);
        }

        /***************************************************/

        public static BoundingBox GetClone(this BoundingBox box)
        {
            return new BoundingBox(box.Min.GetClone(), box.Max.GetClone());
        }

        /***************************************************/

        public static CompositeGeometry GetClone(this CompositeGeometry group)
        {
            return new CompositeGeometry(group.Elements.Select(x => x._GetClone()));
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry _GetClone(this IBHoMGeometry geometry)
        {
            return GetClone(geometry as dynamic);
        }

        /***************************************************/

        public static ICurve _GetClone(this ICurve curve)
        {
            return GetClone(curve as dynamic);
        }

        /***************************************************/

        public static ISurface _GetClone(this ISurface surface)
        {
            return GetClone(surface as dynamic);
        }

    }
}
