using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static partial class Create
    {
        public static Circle Circle(Point p1, Point p2, Point p3)
        {
            Arc arc = new Arc(p1, p2, p3);
            Plane plane = null;
            arc.TryGetPlane(out plane);
            return new Circle(arc.Radius(), plane);
        }
    }

}
