using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> GetEdges(this PanelFreeForm contour)
        {
            if (contour.Surface != null)
                return contour.Surface.IGetExternalEdges();
            else
                return new List<ICurve>();
        }

        /***************************************************/


    }

}
