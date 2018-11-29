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
            return point;
        }

        /***************************************************/

        public static ICurve GetGeometry(this Line curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve GetGeometry(this Arc curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve GetGeometry(this Circle curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve GetGeometry(this Ellipse curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve GetGeometry(this NurbCurve curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve GetGeometry(this Polyline curve)
        {
            return curve;
        }

        /***************************************************/

        public static ICurve GetGeometry(this PolyCurve curve)
        {
            return curve;
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
