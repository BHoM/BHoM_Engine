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
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry GetRotated(this IBHoMGeometry geometry, double rad, Vector axis)
        {
            return _GetRotated(geometry as dynamic, rad, axis);
        }


        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        private static Point _GetRotated(this Point pt, double rad, Vector axis)
        {
            throw new NotImplementedException(); //TODO: rotation of a point around arbitrary axis
        }

        /***************************************************/

        private static Vector _GetRotated(this Vector vector, double rad, Vector axis)
        {
            // using Rodrigues' rotation formula
            axis = axis.GetNormalised();

            return vector * Math.Cos(rad) + axis.GetCrossProduct(vector) * Math.Sin(rad) + axis * (axis * vector) * (1 - Math.Cos(rad));
        }

        /***************************************************/

        private static Plane _GetRotated(this Plane plane, double rad, Vector axis)
        {
            return new Plane(plane.Origin._GetRotated(rad, axis), plane.Normal._GetRotated(rad, axis));
        }


        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        private static Arc _GetRotated(this Arc arc, double rad, Vector axis)
        {
            return new Arc(arc.Start._GetRotated(rad, axis), arc.Middle._GetRotated(rad, axis), arc.End._GetRotated(rad, axis));
        }

        /***************************************************/

        private static Circle _GetRotated(this Circle circle, double rad, Vector axis)
        {
            return new Circle(circle.Centre._GetRotated(rad, axis), circle.Normal._GetRotated(rad, axis), circle.Radius);
        }

        /***************************************************/

        private static Line _GetRotated(this Line line, double rad, Vector axis)
        {
            return new Line(line.Start._GetRotated(rad, axis), line.End._GetRotated(rad, axis));
        }

        /***************************************************/

        private static NurbCurve _GetRotated(this NurbCurve curve, double rad, Vector axis)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x._GetRotated(rad, axis)), curve.Weights, curve.Knots);
        }


        /***************************************************/

        private static PolyCurve _GetRotated(this PolyCurve curve, double rad, Vector axis)
        {
            return new PolyCurve(curve.Curves.Select(x => x.GetRotated(rad, axis) as ICurve));
        }

        /***************************************************/

        private static Polyline _GetRotated(this Polyline curve, double rad, Vector axis)
        {
            return new Polyline(curve.ControlPoints.Select(x => x._GetRotated(rad, axis)));
        }


        /***************************************************/
        /**** Private Methods - Surfaces                ****/
        /***************************************************/

        private static Extrusion _GetRotated(this Extrusion surface, double rad, Vector axis)
        {
            return new Extrusion(surface.Curve.GetRotated(rad, axis) as ICurve, surface.Direction._GetRotated(rad, axis), surface.Capped);
        }

        /***************************************************/

        private static Loft _GetRotated(this Loft surface, double rad, Vector axis)
        {
            return new Loft(surface.Curves.Select(x => x.GetRotated(rad, axis) as ICurve));
        }

        /***************************************************/

        private static NurbSurface _GetRotated(this NurbSurface surface, double rad, Vector axis)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x._GetRotated(rad, axis)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        private static Pipe _GetRotated(this Pipe surface, double rad, Vector axis)
        {
            return new Pipe(surface.Centreline.GetRotated(rad, axis) as ICurve, surface.Radius, surface.Capped);
        }

        /***************************************************/

        private static PolySurface _GetRotated(this PolySurface surface, double rad, Vector axis)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.GetRotated(rad, axis) as ISurface));
        }


        /***************************************************/
        /**** Private Methods - Others                  ****/
        /***************************************************/

        private static Mesh _GetRotated(this Mesh mesh, double rad, Vector axis)
        {
            return new Mesh(mesh.Vertices.Select(x => x._GetRotated(rad, axis)), mesh.Faces.Select(x => x.GetClone() as Face));
        }

        /***************************************************/

        private static GeometryGroup _GetRotated(this GeometryGroup group, double rad, Vector axis)
        {
            return new GeometryGroup(group.Elements.Select(x => x.GetRotated(rad, axis)));
        }
    }
}
