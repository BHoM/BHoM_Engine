using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM_Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector GetExtents(this BoundingBox box)
        {
            return new Vector(box.Max.X - box.Min.X, box.Max.Y - box.Min.Y, box.Max.Z - box.Min.Z);
        }

        /***************************************************/

        public static Point GetCentre(this BoundingBox box)
        {
            return new Point((box.Max.X + box.Min.X) / 2, (box.Max.Y + box.Min.Y) / 2, (box.Max.Z + box.Min.Z) / 2);
        }
    }
}
