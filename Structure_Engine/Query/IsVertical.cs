using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using System;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsVertical(this Bar bar)
        {
            return IsVertical(bar.StartNode.Position, bar.EndNode.Position);
        }

        /***************************************************/

        public static bool IsVertical(this FramingElement element)
        {
            return IsVertical(element.LocationCurve.IStartPoint(), element.LocationCurve.IEndPoint()); //TODO: is this correct? what is the framing element is curved?
        }

        /***************************************************/

        public static bool IsVertical(this Line line)
        {
            return IsVertical(line.Start, line.End);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool IsVertical(Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;

            return Math.Sqrt(dx * dx + dy * dy) < 0.0001;
        }


        /***************************************************/
    }
}