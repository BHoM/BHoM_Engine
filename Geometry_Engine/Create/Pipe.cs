using BH.oM.Geometry;
using System;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Pipe Pipe(ICurve centreline, double radius, bool capped = true)
        {
            return new Pipe
            {
                Centreline = centreline,
                Radius = radius,
                Capped = capped
            };
        }

        /***************************************************/

        public static Pipe RandomPipe(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomPipe(rnd, box);
        }

        /***************************************************/

        public static Pipe RandomPipe(Random rnd, BoundingBox box = null)
        {
            if (box == null)
            {
                return new Pipe
                {
                    Centreline = RandomCurve(rnd),
                    Radius = rnd.NextDouble(),
                    Capped = rnd.NextDouble() >= 0.5
                };
            }
            else
            {
                ICurve curve = RandomCurve(rnd, box);
                BoundingBox curveBox = curve.IBounds();
                double maxRadius = new double[]
                {
                    box.Max.X - curveBox.Max.X,
                    box.Max.Y - curveBox.Max.Y,
                    box.Max.Z - curveBox.Max.Z,
                    curveBox.Min.X - box.Min.X,
                    curveBox.Min.Y - box.Min.Y,
                    curveBox.Min.Z - box.Min.Z
                }.Min();

                return new Pipe
                {
                    Centreline = curve,
                    Radius = maxRadius * rnd.NextDouble(),
                    Capped = rnd.NextDouble() >= 0.5
                };
            }
        }

        /***************************************************/
    }
}
