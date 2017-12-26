using BH.oM.Graphics;
using System.Collections.Generic;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SVGStyle SVGStyle(double strokeWidth, string strokeColor, string fillColor, double strokeOpacity, double fillOpacity, List<double> strokeDash)
        {
            return new SVGStyle
            {
                StrokeWidth = strokeWidth,
                StrokeColor = strokeColor,
                FillColor = fillColor,
                StrokeOpacity = strokeOpacity,
                FillOpacity = fillOpacity,
                StrokeDash = strokeDash
            };
        }

        /***************************************************/
    }
}
