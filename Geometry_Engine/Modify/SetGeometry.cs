using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static Point SetGeometry(this Point point, Point newPoint)
        {
            return newPoint.Clone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Line curve, ICurve newCurve)
        {
            return newCurve.IClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Arc curve, ICurve newCurve)
        {
            return newCurve.IClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Circle curve, ICurve newCurve)
        {
            return newCurve.IClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Ellipse curve, ICurve newCurve)
        {
            return newCurve.IClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this NurbsCurve curve, ICurve newCurve)
        {
            return newCurve.IClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this Polyline curve, ICurve newCurve)
        {
            return newCurve.IClone();
        }

        /***************************************************/

        public static ICurve SetGeometry(this PolyCurve curve, ICurve newCurve)
        {
            return newCurve.IClone();
        }


        /***************************************************/
        /****              Interface Methods            ****/
        /***************************************************/

        public static ICurve ISetGeometry(this ICurve curve, ICurve newCurve)
        {
            return SetGeometry(curve as dynamic, newCurve);
        }

        /***************************************************/
    }
}
