using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector Direction(this Line line, double tolerance = Tolerance.Distance)
        {
            if (line.Start.SquareDistance(line.End) <= tolerance * tolerance) return new Vector { X = 0, Y = 0, Z = 0 };
            return new Vector { X = line.End.X - line.Start.X, Y = line.End.Y - line.Start.Y, Z = line.End.Z - line.Start.Z }.Normalise();
        }

        /***************************************************/

        public static Vector Normal(this Mesh mesh, Face face, double tolerance = Tolerance.Distance)
        {
            List<Point> vertices = mesh.Vertices;

            Point p1 = vertices[(face.A)];
            Point p2 = vertices[(face.B)];
            Point p3 = vertices[(face.C)];
            Vector v1 = p2 - p1;
            Vector v2 = p3 - p1;
            double sqTol = tolerance * tolerance;
            return v1.SquareLength() <= sqTol || v2.SquareLength() <= sqTol ? null : Query.CrossProduct(p2 - p1, p3 - p1);
        }

        /***************************************************/
    }
}
