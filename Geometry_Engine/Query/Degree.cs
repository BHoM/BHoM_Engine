using System.Collections.Generic;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static int Degree(this NurbCurve curve)
        {
            return curve.Knots.Count - curve.ControlPoints.Count - 1;
        }

        /***************************************************/

        public static List<int> Degrees(this NurbSurface surf)
        {
            int uDegree = 1;
            int vDegree = 1;
            while (surf.UKnots[uDegree - 1] == surf.UKnots[uDegree]) uDegree++;          
            while (surf.VKnots[vDegree - 1] == surf.VKnots[vDegree]) vDegree++;            
            return new List<int>() { uDegree, vDegree };
        }

        /***************************************************/
    }
}
