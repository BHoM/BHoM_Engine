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

        public static IntegrationSlice GetSliceAt(IList<ICurve> edges, double location, double width, Plane p)
        {
            List<Point> y = new List<Point>();
            double length = 0;
            Plane plane = new Plane(new Point(p.Normal * location), p.Normal);
            for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
            {
                y.AddRange(edges[edgeIndex].GetIntersections(plane, Tolerance.Distance));
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
            return new IntegrationSlice(width, length, location, isolatedCoords.ToArray());
        }

    }
}
