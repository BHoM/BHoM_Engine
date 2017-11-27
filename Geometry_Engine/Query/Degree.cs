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
            for (int i = 1; i < surf.UKnots.Count; i++)
            {
                if (surf.UKnots[i - 1] == surf.UKnots[i]) uDegree++;
                else break;
            }
            int vDegree = 1;
            for (int i = 1; i < surf.VKnots.Count; i++)
            {
                if (surf.VKnots[i - 1] == surf.VKnots[i]) vDegree++;
                else break;
            }
            return new List<int>() { uDegree, vDegree };
        }
    }
}
