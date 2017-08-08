using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Verify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsContaining(this BoundingBox box1, BoundingBox box2)
        {
            return (box1.Min.X <= box2.Min.X && box1.Min.Y <= box2.Min.Y && box1.Min.Z <= box2.Min.Z && box1.Max.X >= box2.Max.X && box1.Max.Y >= box2.Max.Y && box1.Max.Z >= box2.Max.Z);
        }

        /***************************************************/

        public static bool IsContaining(this BoundingBox box1, IBHoMGeometry geometry)
        {
            return _IsContaining(box1, geometry as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool _IsContaining(this BoundingBox box, IBHoMGeometry geometry)
        {
            return box.IsContaining(geometry.GetBounds());
        }

        /***************************************************/

        private static bool _IsContaining(this BoundingBox box, Point pt)
        {
            Point max = box.Max;
            Point min = box.Min;

            return (pt.X <= max.X && pt.X >= min.X && pt.Y <= max.Y && pt.Y >= min.Y && pt.Z <= max.Z && pt.Z >= min.Z);
        }
    }
}
