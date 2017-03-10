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
        public static Polyline Polyline(List<Point> pnts)
        {
            return new BHoM.Geometry.Polyline(pnts);
        }
    }

}
