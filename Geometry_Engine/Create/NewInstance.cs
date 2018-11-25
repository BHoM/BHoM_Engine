using BH.oM.Geometry;
using System;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Arc NewInstance(this Arc curve)
        {
            return new Arc();
        }

        /***************************************************/

        public static Circle NewInstance(this Circle curve)
        {
            return new Circle();
        }

        /***************************************************/

        public static NurbCurve NewInstance(this NurbCurve curve)
        {
            return new NurbCurve();
        }

        /***************************************************/

        public static Line NewInstance(this Line curve)
        {
            return new Line();
        }

        /***************************************************/

        public static Polyline NewInstance(this Polyline curve)
        {
            return new Polyline();
        }

        /***************************************************/

        public static PolyCurve NewInstance(this PolyCurve curve)
        {
            return new PolyCurve();
        }

        /***************************************************/
    }
}
