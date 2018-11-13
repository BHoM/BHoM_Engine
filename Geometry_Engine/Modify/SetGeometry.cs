using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static ICurve SetGeometry(this Line curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        public static ICurve SetGeometry(this Arc curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        public static ICurve SetGeometry(this Circle curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        public static ICurve SetGeometry(this Ellipse curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        public static ICurve SetGeometry(this NurbCurve curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        public static ICurve SetGeometry(this Polyline curve, ICurve newCurve)
        {
            return newCurve;
        }

        /***************************************************/

        public static ICurve SetGeometry(this PolyCurve curve, ICurve newCurve)
        {
            return newCurve;
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
