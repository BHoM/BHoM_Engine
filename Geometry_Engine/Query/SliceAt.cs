using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IntegrationSlice SliceAt(IList<ICurve> edges, double location, double width, Plane p)
        {
            List<Point> y = new List<Point>();
            double length = 0;
            Plane plane = new Plane { Origin = Create.Point(p.Normal * location), Normal = p.Normal };
            for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
            {
                y.AddRange(edges[edgeIndex].IPlaneIntersections(plane, Tolerance.Distance));
            }

            y.RemoveAll(x => x == null);

            List<double> isolatedCoords = new List<double>();

            for (int point = 0; point < y.Count; point++)
            {
                if (p.Normal.X > 0)
                {
                    isolatedCoords.Add(y[point].Y);
                }
                else
                {
                    isolatedCoords.Add(y[point].X);
                }
            }

            isolatedCoords.Sort();

            if (isolatedCoords.Count % 2 != 0)
            {
                for (int k = 0; k < isolatedCoords.Count - 1; k++)
                {
                    if (isolatedCoords[k] == isolatedCoords[k + 1])
                    {
                        isolatedCoords.RemoveAt(k + 1);
                    }
                }
            }

            for (int j = 0; j < isolatedCoords.Count - 1; j += 2)
            {
                length = length + isolatedCoords[j + 1] - isolatedCoords[j];
            }
            return Create.IntegrationSlice(width, length, location, isolatedCoords.ToArray());
        }

        /***************************************************/
    }
}
