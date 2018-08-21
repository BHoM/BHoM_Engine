using BH.oM.Geometry;
using BH.oM.Structure.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Line Centreline(this Bar bar)
        {
            return new Line { Start = bar.StartNode.Position, End = bar.EndNode.Position };
        }

        /***************************************************/
    }
}
