using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Geometry
{
    public class BoundingBox
    {
        public Point Min { get; set; } = new Point();
        public Point Max { get; set; } = new Point();

        public BoundingBox() { }

        public BoundingBox(Point min, Point max)
        {
            Min = min;
            Max = max;
        }
    }
}
