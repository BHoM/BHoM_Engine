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
        public static Polyline Polyline(List<Point> pnts)
        {
            return new BH.oM.Geometry.Polyline(pnts);
        }
    }

}
