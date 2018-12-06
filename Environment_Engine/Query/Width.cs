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

        public static double Width(this BuildingElement element)
        {
            return element.PanelCurve.Width();
        }

        public static double Width(this Opening opening)
        {
            return opening.OpeningCurve.Width();
        }

        public static double Width(this ICurve panelCurve)
        {
            BoundingBox bBox = panelCurve.IBounds();

            double diffX = Math.Abs(bBox.Max.X - bBox.Min.X);
            double diffY = Math.Abs(bBox.Max.Y - bBox.Min.Y);

            return Math.Sqrt((diffX * diffX) + (diffY * diffY));
        }
    }
}
