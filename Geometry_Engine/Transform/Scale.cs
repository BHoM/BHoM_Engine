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

        public static Point Scale(this Point pt, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return pt.GetTransformed(t);
        }

        /***************************************************/

        public static Vector Scale(this Vector vector, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return vector.GetTransformed(t);
        }

        /***************************************************/

        public static Plane Scale(this Plane plane, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return plane.GetTransformed(t);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/
    
        public static Arc Scale(this Arc arc, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return arc.GetTransformed(t);
        }

        /***************************************************/

        public static Circle Scale(this Circle circle, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return circle.GetTransformed(t);
        }

        /***************************************************/

        public static Line Scale(this Line line, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return line.GetTransformed(t);
        }

        /***************************************************/

        public static NurbCurve Scale(this NurbCurve curve, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return curve.GetTransformed(t);
        }

        /***************************************************/

        public static PolyCurve Scale(this PolyCurve curve, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return curve.GetTransformed(t);
        }

        /***************************************************/

        public static Polyline Scale(this Polyline curve, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return curve.GetTransformed(t);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/
        
        public static Extrusion Scale(this Extrusion surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }

        /***************************************************/

        public static Loft Scale(this Loft surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }

        /***************************************************/

        public static NurbSurface Scale(this NurbSurface surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }

        /***************************************************/

        public static Pipe Scale(this Pipe surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }

        /***************************************************/

        public static PolySurface Scale(this PolySurface surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Scale(this Mesh mesh, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return mesh.GetTransformed(t);
        }

        /***************************************************/

        public static CompositeGeometry Scale(this CompositeGeometry group, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return group.GetTransformed(t);
        }

        /***************************************************/

        public static IBHoMGeometry IScale(this IBHoMGeometry geometry, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return geometry.IGetTransformed(t);
        }

        /***************************************************/

        public static ICurve IScale(this ICurve geometry, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return geometry.IGetTransformed(t);
        }

        /***************************************************/

        public static ISurface IScale(this ISurface geometry, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return geometry.IGetTransformed(t);
        }

    }
}
