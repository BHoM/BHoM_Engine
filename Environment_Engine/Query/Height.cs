using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Height(this BuildingElement element)
        {
            return element.PanelCurve.Height();
        }

        public static double Height(this Opening opening)
        {
            return opening.OpeningCurve.Height();
        }

        public static double Height(this ICurve panelCurve)
        {
            BoundingBox bBox = panelCurve.IBounds();

            return (bBox.Max.Z - bBox.Min.Z);
        }
    }
}
