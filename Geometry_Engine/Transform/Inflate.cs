using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BoundingBox GetInflated(this BoundingBox box, double amount)
        {
            Vector extents = new Vector(amount, amount, amount);
            return new BoundingBox(box.Min - extents, box.Max + extents);
        }
    }
}
