using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point Point(double x = 0, double y = 0, double z = 0)
        {
            return new Point { X = x, Y = y, Z = z };
        }

        /***************************************************/

        public static Point Point(Vector v)
        {
            return new Point { X = v.X, Y = v.Y, Z = v.Z };
        }

        /***************************************************/

        public static Point RandomPoint(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomPoint(new Random(seed), box);
        }

        /***************************************************/

        public static Point RandomPoint(Random rnd, BoundingBox box = null)
        {
            if (box != null)
            {
                return new Point
                {
                    X = box.Min.X + rnd.NextDouble() * (box.Max.X - box.Min.X),
                    Y = box.Min.Y + rnd.NextDouble() * (box.Max.Y - box.Min.Y),
                    Z = box.Min.Z + rnd.NextDouble() * (box.Max.Z - box.Min.Z)
                };
            }
            else
            {
                return new Point { X = rnd.NextDouble(), Y = rnd.NextDouble(), Z = rnd.NextDouble() };
            }
        }

        /***************************************************/

        public static List<List<Point>> PointGrid(Point start, Vector dir1, Vector dir2, int nbPts1, int nbPts2)
        {
            List<List<Point>> pts = new List<List<Point>>();

            for (int i = 0; i < nbPts1; i++)
            {
                List<Point> row = new List<Point>();
                for (int j = 0; j < nbPts2; j++)
                    row.Add(start + i * dir1 + j * dir2);
                pts.Add(row);
            }
            return pts;
        }


        /***************************************************/
    }
}
