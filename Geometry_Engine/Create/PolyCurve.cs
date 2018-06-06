using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PolyCurve PolyCurve(IEnumerable<ICurve> curves)
        {
            return new PolyCurve { Curves = curves.ToList() };
        }

        /***************************************************/

        public static PolyCurve RandomPolyCurve(int seed = -1, BoundingBox box = null, int minNbCurves = 2, int maxNbCurves = 10)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomPolyCurve(rnd, box, minNbCurves, maxNbCurves);
        }

        /***************************************************/

        public static PolyCurve RandomPolyCurve(Random rnd, BoundingBox box = null, int minNbCurves = 2, int maxNbCurves = 10)
        {
            List<ICurve> curves = new List<ICurve>();
            for (int i = 0; i < rnd.Next(minNbCurves, maxNbCurves + 1); i++)
                curves.Add(RandomCurve(rnd, box));
            return new PolyCurve { Curves = curves };
        }

        /***************************************************/
    }
}
