using BH.oM.Geometry;
using System;

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
    }
}
