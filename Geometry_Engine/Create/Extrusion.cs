using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Extrusion Extrusion(ICurve curve, Vector direction, bool capped = true)
        {
            return new Extrusion
            {
                Curve = curve,
                Direction = direction,
                Capped = capped
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Extrusion RandomExtrusion(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomExtrusion(rnd, box);
        }

        /***************************************************/

        public static Extrusion RandomExtrusion(Random rnd, BoundingBox box = null)
        {
            if (box == null)
            {
                return new Extrusion
                {
                    Curve = RandomCurve(rnd),
                    Direction = RandomVector(rnd),
                    Capped = rnd.NextDouble() >= 0.5
                };
            }
            else
            {
                ICurve curve = RandomCurve(rnd, box);
                return new Extrusion
                {
                    Curve = curve,
                    Direction = RandomPoint(rnd, box) - curve.IBounds().Centre(),
                    Capped = rnd.NextDouble() >= 0.5
                };
            }  
        }

        /***************************************************/
    }
}
