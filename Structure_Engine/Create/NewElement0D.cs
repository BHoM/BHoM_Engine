using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Common;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IElement0D NewElement0D(this Bar bar, Point point)
        {
            return new Node { Position = point.Clone() };
        }

        /***************************************************/
    }
}
