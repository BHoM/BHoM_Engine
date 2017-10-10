using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        public static Polyline Polyline(this Point pointA, Point pointB)
        {
            return new Polyline(new List<Point> { pointA, pointB });
        }
    }
}
