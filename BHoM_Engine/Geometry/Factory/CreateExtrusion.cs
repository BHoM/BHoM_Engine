using BHoM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{
    public static partial class Create
    {
        public static Extrusion Extrusion(Curve curve, Vector direction, bool capped)
        {
            return new BHoM.Geometry.Extrusion(curve, direction, capped);
        }
    }

}
