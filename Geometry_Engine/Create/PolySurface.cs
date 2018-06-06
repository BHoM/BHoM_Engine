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

        public static PolySurface PolySurface(IEnumerable<ISurface> surfaces)
        {
            return new PolySurface { Surfaces = surfaces.ToList() };
        }

        /***************************************************/

        public static PolySurface RandomPolySurface(int seed = -1, BoundingBox box = null, int minNbSurfaces = 2, int maxNbSurfaces = 10)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomPolySurface(rnd, box, minNbSurfaces, maxNbSurfaces);
        }

        /***************************************************/

        public static PolySurface RandomPolySurface(Random rnd, BoundingBox box = null, int minNbSurfaces = 2, int maxNbSurfaces = 10)
        {
            List<ISurface> surfaces = new List<ISurface>();
            for (int i = 0; i < rnd.Next(minNbSurfaces, maxNbSurfaces + 1); i++)
                surfaces.Add(RandomSurface(rnd, box));
            return new PolySurface { Surfaces = surfaces };
        }

        /***************************************************/
    }
}
