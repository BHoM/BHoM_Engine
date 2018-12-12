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

        public static CompositeGeometry CompositeGeometry(IEnumerable<IGeometry> elements)
        {
            return new CompositeGeometry { Elements = elements.ToList() };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static CompositeGeometry RandomCompositeGeometry(int seed = -1, BoundingBox box = null, int minNbParts = 1, int maxNbParts = 10)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomCompositeGeometry(rnd, box, minNbParts, maxNbParts);
        }

        /***************************************************/

        public static CompositeGeometry RandomCompositeGeometry(Random rnd, BoundingBox box = null, int minNbParts = 1, int maxNbParts = 10)
        {
            List<IGeometry> elements = new List<IGeometry>();
            for (int i = 0; i < rnd.Next(minNbParts, maxNbParts + 1); i++)
                elements.Add(RandomCurve(rnd, box));
            return new CompositeGeometry { Elements = elements };
        }

        /***************************************************/
    }
}
