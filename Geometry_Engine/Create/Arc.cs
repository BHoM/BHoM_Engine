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

        public static Arc Arc(CoordinateSystem coordinateSystem, double radius, double angle)
        {
            return new Arc
            {
                CoordinateSystem = coordinateSystem,
                Radius = radius,
                Angle = angle
            };
        }

        /***************************************************/

        public static Arc Arc(Point start, Point middle, Point end, double tolerance = Tolerance.Distance)
        {
            Vector v1 = start - middle;
            Vector v2 = end - middle;
            Vector normal = v1.CrossProduct(v2);

            Point centre =  Query.LineIntersection(
                Create.Line(middle + v1 / 2, v1.CrossProduct(normal)),
                Create.Line(middle + v2 / 2, v2.CrossProduct(normal)),
                true,
                tolerance
            );

            Vector stVec = start - centre;
            Vector enVec = end - centre;


            CoordinateSystem cs = CoordinateSystem(centre, stVec, enVec);

            double angle = stVec.SignedAngle(enVec, cs.Z);

            if (angle < 0)
                angle = Math.PI * 2 - angle;

            return new Arc
            {
                CoordinateSystem = cs,
                Radius = stVec.Length(),
                Angle = angle
            };

        }

        /***************************************************/

        public static Arc ArcByCentre(Point centre, Point start, Point end, double tolerance = Tolerance.Distance)
        {
            double radius = start.Distance(centre);

            //Check that start and end points are the same distance from the centre point
            if (Math.Abs(radius - end.Distance(centre)) > tolerance)
            {
                Reflection.Compute.RecordError("Start and end points need to have the same distance from the centre point");
                return null;
            }

            Vector v1 = start - centre;
            Vector v2 = end - centre;

            CoordinateSystem system = CoordinateSystem(centre, v1, v2);

            double angle = v1.SignedAngle(v2, system.Z);

            if (angle < 0)
                angle = Math.PI * 2 - angle;

            return new Arc
            {
                CoordinateSystem = system,
                Angle = angle,
                Radius = radius
            };
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

            //TODO: Can be made more efficient with new definition of arc
            return Arc( circle.PointAtLength(startLength),
                        circle.PointAtLength((startLength + endLength) / 2),
                        circle.PointAtLength(endLength)
            );
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
