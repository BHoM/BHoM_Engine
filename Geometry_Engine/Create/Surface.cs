using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISurface RandomSurface(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomSurface(rnd, box);
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static ISurface RandomSurface(Random rnd, BoundingBox box = null)
        {
            int nb = rnd.Next(5);
            switch (nb)
            {
                case 0:
                    return RandomExtrusion(rnd, box);
                case 1:
                    return RandomLoft(rnd, box);
                case 2:
                    return RandomNurbsSurface(rnd, box);
                case 3:
                    return RandomPipe(rnd, box);
                default:
                    return RandomPolySurface(rnd, box);
            }
        }
        
        /***************************************************/
    }
}
