using BH.oM.Environmental.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Opening Opening(Polyline contour)
        {
            return new Opening { Contour = contour };
        }

        /***************************************************/
    }
}
