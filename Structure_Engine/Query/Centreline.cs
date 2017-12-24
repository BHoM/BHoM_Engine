using BH.oM.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Line GetCentreline(this Bar bar)
        {
            return new Line(bar.StartNode.Point, bar.EndNode.Point);
        }

        /***************************************************/


    }
}
