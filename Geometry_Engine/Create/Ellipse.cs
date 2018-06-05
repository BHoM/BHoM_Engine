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

        public static Ellipse Ellipse(Point centre, double radius1, double radius2)
        {
            return new Ellipse
            {
                Centre = centre,
                Radius1 = radius1,
                Radius2 = radius2
            };
        }

        /***************************************************/

        public static Ellipse Ellipse(Point centre, Vector axis1, Vector axis2, double radius1, double radius2)
        {
            return new Ellipse
            {
                Centre = centre,
                Axis1 = axis1,
                Axis2 = axis2,
                Radius1 = radius1,
                Radius2 = radius2
            };
        }

        /***************************************************/

        public static Ellipse RandomEllipse(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomEllipse(rnd, box);
        }

        /***************************************************/

        public static Ellipse RandomEllipse(Random rnd, BoundingBox box = null)
        {
            if (box == null)
            {
                Vector axis1 = RandomVector(rnd).Normalise();
                return new Ellipse
                {
                    Centre = RandomPoint(rnd),
                    Axis1 = axis1,
                    Axis2 = RandomVector(rnd).Project(new Plane { Normal = axis1 }).Normalise(),
                    Radius1 = rnd.NextDouble(),
                    Radius2 = rnd.NextDouble()
                };
            }
            else
            {
                Point centre = RandomPoint(rnd, box);
                Vector axis1 = RandomVector(rnd).Normalise();
                double maxRadius = new double[]
                {
                    box.Max.X - centre.X,
                    box.Max.Y - centre.Y,
                    box.Max.Z - centre.Z,
                    centre.X - box.Min.X,
                    centre.Y - box.Min.Y,
                    centre.Z - box.Min.Z
                }.Min();

                return new Ellipse
                {
                    Centre = centre,
                    Axis1 = axis1,
                    Axis2 = RandomVector(rnd).Project(new Plane { Normal = axis1 }).Normalise(),
                    Radius1 = maxRadius + rnd.NextDouble(),
                    Radius2 = maxRadius + rnd.NextDouble()
                };
            }

        }

        /***************************************************/
    }
}
