using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point GetTransformed(this Point pt, TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;
            
            return new Point(
                matrix[0, 0] * pt.X + matrix[0, 1] * pt.Y + matrix[0, 2] * pt.Z + matrix[0, 3],
                matrix[1, 0] * pt.X + matrix[1, 1] * pt.Y + matrix[1, 2] * pt.Z + matrix[1, 3],
                matrix[2, 0] * pt.X + matrix[2, 1] * pt.Y + matrix[2, 2] * pt.Z + matrix[2, 3]
            );
        }

        /***************************************************/

        public static Vector GetTransformed(this Vector vector, TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;

            return new Vector(
                matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z + matrix[0, 3],
                matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z + matrix[1, 3],
                matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z + matrix[2, 3]
            );
        }

        /***************************************************/

        public static Plane GetTransformed(this Plane plane, TransformMatrix transform)
        {
            return new Plane(plane.Origin.GetTransformed(transform), plane.Normal.GetTransformed(transform));
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc GetTransformed(this Arc arc, TransformMatrix transform)
        {
            return new Arc(arc.Start.GetTransformed(transform), arc.Middle.GetTransformed(transform), arc.End.GetTransformed(transform));
        }

        /***************************************************/

        public static Circle GetTransformed(this Circle circle, TransformMatrix transform)
        {
            return new Circle(circle.Centre.GetTransformed(transform), circle.Normal.GetTransformed(transform), circle.Radius);
        }

        /***************************************************/

        public static Line GetTransformed(this Line line, TransformMatrix transform)
        {
            return new Line(line.Start.GetTransformed(transform), line.End.GetTransformed(transform));
        }

        /***************************************************/

        public static NurbCurve GetTransformed(this NurbCurve curve, TransformMatrix transform)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x.GetTransformed(transform)), curve.Weights, curve.Knots);
        }


        /***************************************************/

        public static PolyCurve GetTransformed(this PolyCurve curve, TransformMatrix transform)
        {
            return new PolyCurve(curve.Curves.Select(x => x.IGetTransformed(transform)));
        }

        /***************************************************/

        public static Polyline GetTransformed(this Polyline curve, TransformMatrix transform)
        {
            return new Polyline(curve.ControlPoints.Select(x => x.GetTransformed(transform)));
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion GetTransformed(this Extrusion surface, TransformMatrix transform)
        {
            return new Extrusion(surface.Curve.IGetTransformed(transform), surface.Direction.GetTransformed(transform), surface.Capped);
        }

        /***************************************************/

        public static Loft GetTransformed(this Loft surface, TransformMatrix transform)
        {
            return new Loft(surface.Curves.Select(x => x.IGetTransformed(transform)));
        }

        /***************************************************/

        public static NurbSurface GetTransformed(this NurbSurface surface, TransformMatrix transform)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x.GetTransformed(transform)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        public static Pipe GetTransformed(this Pipe surface, TransformMatrix transform)
        {
            return new Pipe(surface.Centreline.IGetTransformed(transform), surface.Radius, surface.Capped);
        }

        /***************************************************/

        public static PolySurface GetTransformed(this PolySurface surface, TransformMatrix transform)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.IGetTransformed(transform)));
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh GetTransformed(this Mesh mesh, TransformMatrix transform)
        {
            return new Mesh(mesh.Vertices.Select(x => x.GetTransformed(transform)), mesh.Faces.Select(x => x.GetClone() as Face));
        }

        /***************************************************/

        public static CompositeGeometry GetTransformed(this CompositeGeometry group, TransformMatrix transform)
        {
            return new CompositeGeometry(group.Elements.Select(x => x.IGetTransformed(transform)));
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry IGetTransformed(this IBHoMGeometry geometry, TransformMatrix transform)
        {
            return GetTransformed(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ICurve IGetTransformed(this ICurve geometry, TransformMatrix transform)
        {
            return GetTransformed(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ISurface IGetTransformed(this ISurface geometry, TransformMatrix transform)
        {
            return GetTransformed(geometry as dynamic, transform);
        }


    }
}
