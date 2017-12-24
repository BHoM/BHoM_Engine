using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Polyline Polyline(IEnumerable<Point> points)
        {
            return new Polyline { ControlPoints = points.ToList() };
        }

        /***************************************************/
    }
}
