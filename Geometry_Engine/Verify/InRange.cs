using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsInRange(this BoundingBox box1, BoundingBox box2, double tolerance = 0)
        {
            return (box1.Min.X <= box2.Max.X + tolerance && box2.Min.X <= box1.Max.X + tolerance &&
                     box1.Min.Y <= box2.Max.Y + tolerance && box2.Min.Y <= box1.Max.Y + tolerance &&
                     box1.Min.Z <= box2.Max.Z + tolerance && box2.Min.Z <= box1.Max.Z + tolerance);
        }
    }
}
