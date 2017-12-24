using BH.Engine.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        //***************************************************/
        //**** Public Methods                            ****/
        //***************************************************/

        public static double GetLength(this Bar bar)
        {
            return bar.StartNode.Point.GetDistance(bar.EndNode.Point);
        }
    }
}
