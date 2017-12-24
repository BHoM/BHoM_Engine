using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<int> Side(this Plane plane, List<Point> points, double tolerance = Tolerance.Distance)
        {
            List<double> result = points.Select(x => new Vector(x).DotProduct(plane.Normal)).ToList();
            int[] sameSide = new int[result.Count];

            double d = -plane.Normal.DotProduct(new Vector(plane.Origin));

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] + d > tolerance)
                {
                    sameSide[i] = 1;
                }
                else if (result[i] + d < -tolerance)
                {
                    sameSide[i] = -1;
                }
                else
                {
                    sameSide[i] = 0;
                }
            }
            return sameSide.ToList();
        }

        /***************************************************/
    }
}
