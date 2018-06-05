using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IGeometry RandomGeometry(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomGeometry(rnd, box);
        }

        /***************************************************/

        public static IGeometry RandomGeometry(Random rnd, BoundingBox box = null)
        {
            int nb = rnd.Next(15);
            switch(nb)
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
                case 5:
                    return RandomPolyline(rnd, box);
                case 6:
                    return RandomExtrusion(rnd, box);
                case 7:
                    return RandomLoft(rnd, box);
                case 8:
                    return RandomNurbSurface(rnd, box);
                case 9:
                    return RandomPipe(rnd, box);
                case 10:
                    return RandomPolySurface(rnd, box);
                case 11:
                    return RandomMesh(rnd, box);
                case 12:
                    return RandomCompositeGeometry(rnd, box);
                case 13:
                    return RandomPlane(rnd, box);
                default:
                    return RandomPoint(rnd, box);
            }
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/



        /***************************************************/
    }
}
