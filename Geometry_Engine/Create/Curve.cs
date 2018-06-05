using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ICurve RandomCurve(int seed = -1, BoundingBox box = null, bool closed = false)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomCurve(rnd, box, closed);
        }

        /***************************************************/

        public static ICurve RandomCurve(Random rnd, BoundingBox box = null, bool closed = false)
        {
            int nb = rnd.Next(6);
            switch (nb)
            {
                case 0:
                    return RandomArc(rnd, box);
                case 1:
                    return RandomCircle(rnd, box);
                case 2:
                    return RandomEllipse(rnd, box);
                case 3:
                    return RandomLine(rnd, box);
                case 4:
                    return RandomNurbCurve(rnd, box);
                default:
                    return RandomPolyline(rnd, box);
            }
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/



        /***************************************************/
    }
}
