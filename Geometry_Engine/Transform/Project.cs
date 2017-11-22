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

        public static Point GetProjected(this Point pt, Plane p)
        {
            return pt - p.Normal.GetDotProduct(pt-p.Origin) * p.Normal;
        }

        /***************************************************/

        public static Point GetProjected(this Point pt, Line line)
        {
            Vector dir = line.GetDirection();
            double t = dir.GetDotProduct(pt - line.Start);
            return line.Start + dir * t;
        }

        /***************************************************/

        public static Vector GetProjected(this Vector vector, Plane p)
        {
            return vector - vector.GetDotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        public static Plane GetProjected(this Plane plane, Plane p)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc GetProjected(this Arc arc, Plane p)
        {
            return new Arc(arc.Start.GetProjected(p), arc.Middle.GetProjected(p), arc.End.GetProjected(p));
        }

        /***************************************************/

        public static Circle GetProjected(this Circle circle, Plane p)
        {
            if (circle.IIsInPlane(p))
                return new Circle(circle.Centre.GetClone() as Point, p.Normal.GetClone() as Vector, circle.Radius);
            throw new NotImplementedException(); //TODO: sort out project for a circle
        }

        /***************************************************/

        public static Line GetProjected(this Line line, Plane p)
        {
            return new Line(line.Start.GetProjected(p), line.End.GetProjected(p));
        }

        /***************************************************/

        public static NurbCurve GetProjected(this NurbCurve curve, Plane p)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x.GetProjected(p)), curve.Weights, curve.Knots);
        }


        /***************************************************/

        public static PolyCurve GetProjected(this PolyCurve curve, Plane p)
        {
            return new PolyCurve(curve.Curves.Select(x => x.IGetProjected(p)));
        }

        /***************************************************/

        public static Polyline GetProjected(this Polyline curve, Plane p)
        {
            return new Polyline(curve.ControlPoints.Select(x => x.GetProjected(p)));
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion GetProjected(this Extrusion surface, Plane p)
        {
            return new Extrusion(surface.Curve.IGetProjected(p), surface.Direction.GetProjected(p), surface.Capped);
        }

        /***************************************************/

        public static Loft GetProjected(this Loft surface, Plane p)
        {
            return new Loft(surface.Curves.Select(x => x.IGetProjected(p)));
        }

        /***************************************************/

        public static NurbSurface GetProjected(this NurbSurface surface, Plane p)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x.GetProjected(p)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        public static Pipe GetProjected(this Pipe surface, Plane p)
        {
            throw new NotImplementedException(); //TODO: implement projection of a pipe on a plane
        }

        /***************************************************/

        public static PolySurface GetProjected(this PolySurface surface, Plane p)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.IGetProjected(p)));
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh GetProjected(this Mesh mesh, Plane p)
        {
            return new Mesh(mesh.Vertices.Select(x => x.GetProjected(p)), mesh.Faces.Select(x => x.GetClone() as Face));
        }

        /***************************************************/

        public static CompositeGeometry GetProjected(this CompositeGeometry group, Plane p)
        {
            return new CompositeGeometry(group.Elements.Select(x => x.IGetProjected(p)));
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry IGetProjected(this IBHoMGeometry geometry, Plane p)
        {
            return GetProjected(geometry as dynamic, p);
        }

        /***************************************************/

        public static ICurve IGetProjected(this ICurve geometry, Plane p)
        {
            return GetProjected(geometry as dynamic, p);
        }

        /***************************************************/

        public static ISurface IGetProjected(this ISurface geometry, Plane p)
        {
            return GetProjected(geometry as dynamic, p);
        }


        /***************************************************/
        /****           Project to XY plane             ****/
        /***************************************************/

        public static Point ProjectToGround(this Point pt)
        {
            return new Point(pt.X, pt.Y, 0);
        }

        /***************************************************/

        public static Line ProjectToGround(this Line line)
        {
            return new Line(line.Start.ProjectToGround(), line.End.ProjectToGround());
        }
    }
}
