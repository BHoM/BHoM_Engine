using System.Collections.Generic;

using BH.oM.Geometry;
using BH.oM.Structure.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> DistributedPoints(Bar bar, int divisions, double startLength = 0, double endLength = 0)
        {
            Point startPos;
            Vector tan;
            if (startLength == 0 && endLength == 0)
            {
                startPos = bar.StartNode.Position;
                tan = (bar.EndNode.Position - bar.StartNode.Position) / (double)divisions;
            }
            else
            {
                double length = bar.Length();
                tan = (bar.EndNode.Position - bar.StartNode.Position) / length;
                startPos = bar.StartNode.Position + tan * startLength;

                tan *= (length - endLength - startLength) / (double)divisions;
            }

            List<Point> pts = new List<Point>();

            for (int i = 0; i <= divisions; i++)
            {
                pts.Add(startPos + tan * i);
            }
            return pts;
        }

        /***************************************************/
    }
}
