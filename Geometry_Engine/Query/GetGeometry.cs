using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static Point GetGeometry(this Point point)
        {
            return point.Clone();
        }

        /***************************************************/

        public static ICurve GetGeometry(this Line curve)
        {
            return curve.Clone();
        }

        /***************************************************/

        public static ICurve GetGeometry(this Arc curve)
        {
            return curve.Clone();
        }

        /***************************************************/

        public static ICurve GetGeometry(this Circle curve)
        {
            return curve.Clone();
        }

        /***************************************************/

        public static ICurve GetGeometry(this Ellipse curve)
        {
            return curve.Clone();
        }

        /***************************************************/

        public static ICurve GetGeometry(this NurbCurve curve)
        {
            return curve.Clone();
        }

        /***************************************************/

        public static ICurve GetGeometry(this Polyline curve)
        {
            return curve.Clone();
        }

        /***************************************************/

        public static ICurve GetGeometry(this PolyCurve curve)
        {
            return curve.Clone();
        }


        /***************************************************/
        /****              Interface Methods            ****/
        /***************************************************/

        public static ICurve IGetGeometry(this ICurve curve)
        {
            return GetGeometry(curve as dynamic);
        }

        /***************************************************/
    }
}
