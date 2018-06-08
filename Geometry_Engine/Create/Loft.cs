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

        public static Loft Loft(IEnumerable<ICurve> curves)
        {
            return new Loft { Curves = curves.ToList() };
        }

        /***************************************************/

        public static Loft RandomLoft(int seed = -1, BoundingBox box = null, int minNbCurves = 2, int maxNbCurves = 10)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomLoft(rnd, box, minNbCurves, maxNbCurves);
        }

        /***************************************************/

        public static Loft RandomLoft(Random rnd, BoundingBox box = null, int minNbCurves = 2, int maxNbCurves = 10)
        {
            List<ICurve> curves = new List<ICurve>();
            for (int i = 0; i < rnd.Next(minNbCurves, maxNbCurves + 1); i++)
                curves.Add(RandomCurve(rnd, box));
            return new Loft { Curves = curves };
        }

        /***************************************************/
    }
}
