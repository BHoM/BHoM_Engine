using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [NotImplemented]
        public static int Degree(this NurbsCurve curve)
        {
            return curve.Knots.Count - curve.ControlPoints.Count + 1;
        }

        /***************************************************/

        [NotImplemented]
        public static List<int> Degrees(this NurbsSurface surf)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}
