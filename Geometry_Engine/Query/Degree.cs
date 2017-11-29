using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        public static List<int> GetDegrees(this NurbSurface surf)
        {
            int uDegree = 1;
            int vDegree = 1;
            while (surf.UKnots[uDegree - 1] == surf.UKnots[uDegree]) uDegree++;          
            while (surf.VKnots[vDegree - 1] == surf.VKnots[vDegree]) vDegree++;            
            return new List<int>() { uDegree, vDegree };
        }

        /***************************************************/

        public static int GetDegree(this NurbCurve curve)
        {
            int degree = 1;
            while (curve.Knots[degree - 1] == curve.Knots[degree]) degree++;
            return degree;
        }
    }
}
