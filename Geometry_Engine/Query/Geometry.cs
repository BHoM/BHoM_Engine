using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static Point Geometry(this Point point)
        {
            return point;
        }

        /***************************************************/

        public static ICurve Geometry(this Line curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this Arc curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this Circle curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this Ellipse curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this NurbCurve curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this Polyline curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve Geometry(this PolyCurve curve)
        {
            return curve;
        }


        /***************************************************/
        /****              Interface Methods            ****/
        /***************************************************/

        public static ICurve IGeometry(this ICurve curve)
        {
            return Geometry(curve as dynamic);
        }

        /***************************************************/
    }
}
