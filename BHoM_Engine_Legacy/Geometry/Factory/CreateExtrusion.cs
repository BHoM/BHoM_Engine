using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static partial class Create
    {
        public static Extrusion Extrusion(Curve curve, Vector direction, bool capped)
        {
            return new BH.oM.Geometry.Extrusion(curve, direction, capped);
        }
    }

}
