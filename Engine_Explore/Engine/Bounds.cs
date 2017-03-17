using Engine_Explore.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Engine
{
    public static class Bounds
    {
        public static BoundingBox Calculate(BHoMGeometry geometry)
        {
            switch (geometry.GetGeometryType())
            {
                case BHoMGeometry.Type.Point:
                    return Calculate(geometry as Point);
                case BHoMGeometry.Type.Vector:
                    return Calculate(geometry as Vector);
                default:
                    return null;
            }
        }

        /***************************************************/

        public static BoundingBox Calculate(Point pt)
        {
            return new BoundingBox(pt, pt);
        }

        /***************************************************/

        public static BoundingBox Calculate(Vector v)
        {
            Point pt = new Point(v.X, v.Y, v.Z);
            return new BoundingBox(pt, pt);
        }
    }
}
