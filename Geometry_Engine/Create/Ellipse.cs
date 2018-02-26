using BH.oM.Geometry;

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
    }
}
