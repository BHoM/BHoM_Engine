using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Line Line(Point start, Point end)
        {
            return new Line
            {
                Start = start,
                End = end
            };
        }

        /***************************************************/

        public static Line Line(Point start, Vector direction)
        {
            return new Line
            {
                Start = start,
                End = start + direction,
                Infinite = true
            };
        }

        /***************************************************/
    }
}
