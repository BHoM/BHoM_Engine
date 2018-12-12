using BH.oM.Geometry;
using System.Collections.Generic;
using System;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsPolylinear(this PolyCurve curve)
        {
            foreach (ICurve c in curve.SubParts())
            {
                if (!(c is Line))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsPolylinear(this Arc curve)
        {
            return false;
        }

        /***************************************************/

        public static bool IsPolylinear(this Circle curve)
        {
            return false;
        }

        /***************************************************/

        public static bool IsPolylinear(this Line curve)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPolylinear(this Polyline curve)
        {
            return true;
        }

        /***************************************************/

        [NotImplemented]
        public static bool IsPolylinear(this NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        public static bool IIsPolylinear(this ICurve curve)
        {
            foreach (ICurve c in curve.ISubParts())
            {
                if (!(c is Line))
                    return false;
            }

            return true;
        }

        /***************************************************/
    }
}