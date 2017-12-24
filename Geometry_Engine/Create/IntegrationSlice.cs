using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        public static List<IntegrationSlice> CreateSlices(List<ICurve> edges, Vector direction, double increment = 0.001)
        {
            List<IntegrationSlice> slices = new List<IntegrationSlice>();

            List<double> cutAt = new List<double>();
            List<double> sliceSegments = new List<double>();
            Plane p = new BH.oM.Geometry.Plane(Point.Origin, direction);

            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = 0; j < edges[i].IGetControlPoints().Count; j++)
                {
                    cutAt.Add(Query.GetDotProduct(new Vector(edges[i].IGetControlPoints()[j]), p.Normal));
                }
            }

            cutAt.Sort();
            cutAt = cutAt.Distinct<double>().ToList();

            double currentValue = Query.GetDotProduct(new Vector(Query.GetBounds(new PolyCurve(edges)).Min), p.Normal);
            double max = Query.GetDotProduct(new Vector(Query.GetBounds(new PolyCurve(edges)).Max), p.Normal);
            int index = 0;

            while (currentValue < max)
            {
                if (cutAt.Count > index && currentValue > cutAt[index])
                {
                    sliceSegments.Add(cutAt[index]);
                    index++;
                }
                else
                {
                    sliceSegments.Add(currentValue);
                    currentValue += increment;
                }
            }

            sliceSegments.Add(max);

            for (int i = 0; i < sliceSegments.Count - 1; i++)
            {
                if (sliceSegments[i] == sliceSegments[i + 1])
                {
                    continue;
                }

                currentValue = (sliceSegments[i] + sliceSegments[i + 1]) / 2;
                slices.Add(Query.GetSliceAt(edges, currentValue, -sliceSegments[i] + sliceSegments[i + 1], p));
            }
            return slices;
        }

        /***************************************************/

    }
}
