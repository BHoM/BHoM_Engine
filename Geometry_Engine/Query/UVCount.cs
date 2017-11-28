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
        public static List<int> GetUVCount(this NurbSurface surf)
        {
            List<int> degrees = surf.GetDegrees();            
            return new List<int> { surf.UKnots.Count - degrees[0] + 1, surf.VKnots.Count - degrees[1] + 1 };
        }
    }
}
