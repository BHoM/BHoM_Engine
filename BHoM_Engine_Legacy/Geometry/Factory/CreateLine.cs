using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static partial class Create
    {
        public static Line Line(Point p1, Point p2)
        {
            return new BH.oM.Geometry.Line(p1, p2);
        }
    }
}
