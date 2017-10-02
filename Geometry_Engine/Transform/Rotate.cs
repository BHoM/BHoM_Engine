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

        public static Point GetRotated(this Point pt, double rad, Vector axis)
        {
            throw new NotImplementedException(); //TODO: rotation of a point around arbitrary axis
        }

        /***************************************************/

        public static Vector GetRotated(this Vector vector, double rad, Vector axis)
        {
            // using Rodrigues' rotation formula
            axis = axis.GetNormalised();

            return vector * Math.Cos(rad) + axis.GetCrossProduct(vector) * Math.Sin(rad) + axis * (axis * vector) * (1 - Math.Cos(rad));
        }

        /***************************************************/

        public static Plane GetRotated(this Plane plane, double rad, Vector axis)
        {
            return new Plane(plane.Origin.GetRotated(rad, axis), plane.Normal.GetRotated(rad, axis));
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc GetRotated(this Arc arc, double rad, Vector axis)
        {
            return new Arc(arc.Start.GetRotated(rad, axis), arc.Middle.GetRotated(rad, axis), arc.End.GetRotated(rad, axis));
        }

        /***************************************************/

        public static Circle GetRotated(this Circle circle, double rad, Vector axis)
        {
            return new Circle(circle.Centre.GetRotated(rad, axis), circle.Normal.GetRotated(rad, axis), circle.Radius);
        }

        /***************************************************/

        public static Line GetRotated(this Line line, double rad, Vector axis)
        {
            return new Line(line.Start.GetRotated(rad, axis), line.End.GetRotated(rad, axis));
        }

        /***************************************************/

        public static NurbCurve GetRotated(this NurbCurve curve, double rad, Vector axis)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x.GetRotated(rad, axis)), curve.Weights, curve.Knots);
        }


        /***************************************************/

        public static PolyCurve GetRotated(this PolyCurve curve, double rad, Vector axis)
        {
            return new PolyCurve(curve.Curves.Select(x => x._GetRotated(rad, axis)));
        }

        /***************************************************/

        public static Polyline GetRotated(this Polyline curve, double rad, Vector axis)
        {
            return new Polyline(curve.ControlPoints.Select(x => x.GetRotated(rad, axis)));
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion GetRotated(this Extrusion surface, double rad, Vector axis)
        {
            return new Extrusion(surface.Curve._GetRotated(rad, axis), surface.Direction.GetRotated(rad, axis), surface.Capped);
        }

        /***************************************************/

        public static Loft GetRotated(this Loft surface, double rad, Vector axis)
        {
            return new Loft(surface.Curves.Select(x => x._GetRotated(rad, axis)));
        }

        /***************************************************/

        public static NurbSurface GetRotated(this NurbSurface surface, double rad, Vector axis)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x.GetRotated(rad, axis)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        public static Pipe GetRotated(this Pipe surface, double rad, Vector axis)
        {
            return new Pipe(surface.Centreline._GetRotated(rad, axis), surface.Radius, surface.Capped);
        }

        /***************************************************/

        public static PolySurface GetRotated(this PolySurface surface, double rad, Vector axis)
        {
            return new PolySurface(surface.Surfaces.Select(x => x._GetRotated(rad, axis)));
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh GetRotated(this Mesh mesh, double rad, Vector axis)
        {
            return new Mesh(mesh.Vertices.Select(x => x.GetRotated(rad, axis)), mesh.Faces.Select(x => x.GetClone()));
        }

        /***************************************************/

        public static CompositeGeometry GetRotated(this CompositeGeometry group, double rad, Vector axis)
        {
            return new CompositeGeometry(group.Elements.Select(x => x._GetRotated(rad, axis)));
        }



        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry _GetRotated(this IBHoMGeometry geometry, double rad, Vector axis)
        {
            return GetRotated(geometry as dynamic, rad, axis);
        }

        /***************************************************/

        public static ICurve _GetRotated(this ICurve geometry, double rad, Vector axis)
        {
            return GetRotated(geometry as dynamic, rad, axis);
        }

        /***************************************************/

        public static ISurface _GetRotated(this ISurface geometry, double rad, Vector axis)
        {
            return GetRotated(geometry as dynamic, rad, axis);
        }


    }
}
