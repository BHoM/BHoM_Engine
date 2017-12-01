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

        public static Point GetScaled(this Point pt, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return pt.GetTransformed(t);
        }

        /***************************************************/

        public static Vector GetScaled(this Vector vector, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return vector.GetTransformed(t);
        }

        /***************************************************/

        public static Plane GetScaled(this Plane plane, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return plane.GetTransformed(t);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/
    
        public static Arc GetScaled(this Arc arc, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return arc.GetTransformed(t);
        }

        /***************************************************/

        public static Circle GetScaled(this Circle circle, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return circle.GetTransformed(t);
        }

        /***************************************************/

        public static Line GetScaled(this Line line, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return line.GetTransformed(t);
        }

        /***************************************************/

        public static NurbCurve GetScaled(this NurbCurve curve, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return curve.GetTransformed(t);
        }

        /***************************************************/

        public static PolyCurve GetScaled(this PolyCurve curve, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return curve.GetTransformed(t);
        }

        /***************************************************/

        public static Polyline GetScaled(this Polyline curve, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return curve.GetTransformed(t);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/
        
        public static Extrusion GetScaled(this Extrusion surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }

        /***************************************************/

        public static Loft GetScaled(this Loft surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }

        /***************************************************/

        public static NurbSurface GetScaled(this NurbSurface surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }

        /***************************************************/

        public static Pipe GetScaled(this Pipe surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }

        /***************************************************/

        public static PolySurface GetScaled(this PolySurface surface, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return surface.GetTransformed(t);
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh GetScaled(this Mesh mesh, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return mesh.GetTransformed(t);
        }

        /***************************************************/

        public static CompositeGeometry GetScaled(this CompositeGeometry group, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return group.GetTransformed(t);
        }

        /***************************************************/

        public static IBHoMGeometry IGetScaled(this IBHoMGeometry geometry, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return geometry.IGetTransformed(t);
        }

        /***************************************************/

        public static ICurve IGetScaled(this ICurve geometry, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return geometry.IGetTransformed(t);
        }

        /***************************************************/

        public static ISurface IGetScaled(this ISurface geometry, Point about, Vector factor)
        {
            TransformMatrix t = Create.ScaleMatrix(about, factor);
            return geometry.IGetTransformed(t);
        }

    }
}
