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

        public static Arc Arc(Point start, Point middle, Point end)
        {
            return new Arc
            {
                Start = start,
                End = end,
                Middle = middle
            }; 
        }

        /***************************************************/

        public static Arc ArcByCentre(Point centre, Point start, Point end, double tolerance = Tolerance.Distance)
        {
            //Check that start and end points are the same distance from the centre point
            if (Math.Abs(start.Distance(centre) - end.Distance(centre)) > tolerance)
                return null;

            Vector v1 = start - centre;
            Vector v2 = end - centre;
            Vector normal = v1.CrossProduct(v2).Normalise();

            if (double.IsNaN(normal.X) || normal.Length() < tolerance)
                normal = oM.Geometry.Vector.ZAxis;

            double angle = v1.SignedAngle(v2, normal);
            Vector midDir = ((Vector)v1.Rotate(angle / 2, normal)).Normalise();
            double midRadius = (start.Distance(centre) + end.Distance(centre)) / 2;

            return new Arc { Start = start, Middle = centre + midRadius * midDir, End = end }; 
        }

        /***************************************************/

        public static Arc RandomArc(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomArc(rnd, box);
        }

        /***************************************************/

        public static Arc RandomArc(Random rnd, BoundingBox box = null)
        {
            Circle circle = RandomCircle(rnd, box);
            double length = circle.Length();
            double startLength = length * rnd.NextDouble();
            double endLength = length * rnd.NextDouble();

            return new Arc
            {
                Start = circle.PointAtLength(startLength),
                End = circle.PointAtLength(endLength),
                Middle = circle.PointAtLength((startLength+endLength)/2)
            };
        }

        /***************************************************/

        public static Arc RandomArc(Point from, int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomArc(from, rnd, box);
        }

        /***************************************************/

        public static Arc RandomArc(Point from, Random rnd, BoundingBox box = null)
        {
            Point centre;
            Vector normal;
            double radius;
            if (box == null)
            {
                centre = RandomPoint(rnd);
                normal = RandomVector(rnd).CrossProduct(centre - from).Normalise();
                radius = from.Distance(centre);
            }
            else
            {
                double maxRadius = new double[]
                {
                    box.Max.X - from.X,
                    box.Max.Y - from.Y,
                    box.Max.Z - from.Z,
                    from.X - box.Min.X,
                    from.Y - box.Min.Y,
                    from.Z - box.Min.Z
                }.Min()/2;

                radius = maxRadius * rnd.NextDouble();

                Vector v = RandomVector(rnd).Normalise();
                centre = from + v * radius;
                normal = RandomVector(rnd).CrossProduct(v).Normalise();

            }

            Circle circle = Circle(centre, normal, radius);


            double length = circle.Length();
            double endLength = length * rnd.NextDouble();

            return ArcByCentre(centre, from, circle.PointAtLength(endLength));

        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Random m_Random = new Random();


        /***************************************************/
    }
}
