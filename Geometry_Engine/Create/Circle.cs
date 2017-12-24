using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Circle Circle(Point centre, double radius = 0)
        {
            return new Circle
            {
                Centre = centre,
                Radius = radius
            }; 
        }

        /***************************************************/

        public static Circle Circle(Point centre, Vector normal, double radius = 0)
        {
            return new Circle
            {
                Centre = centre,
                Normal = normal,
                Radius = radius
            };
        }
        
        /***************************************************/

        public static Circle Circle(Point pt1, Point pt2, Point pt3)
        {
            Vector v1 = pt1 - pt3;
            Vector v2 = pt2 - pt3;
            Vector normal = v1.CrossProduct(v2).Normalise();

            Point centre = Query.LineIntersection(
                Create.Line(pt3 + v1 / 2, v1.CrossProduct(normal)),
                Create.Line(pt3 + v2 / 2, v2.CrossProduct(normal)),
                true
            );

            return new Circle { Centre = centre, Normal = normal, Radius = pt1.GetDistance(centre) };
        }

        /***************************************************/
    }
}
