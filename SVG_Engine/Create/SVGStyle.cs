using BH.oM.Graphics;
using System.Collections.Generic;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SVGStyle SVGStyle(double strokeWidth = 1, string strokeColor = "black", string fillColor = "none", double strokeOpacity = 1, double fillOpacity = 1, List<double> strokeDash = null)
        {
            return new SVGStyle
            {
                StrokeWidth = strokeWidth,
                StrokeColor = strokeColor,
                FillColor = fillColor,
                StrokeOpacity = strokeOpacity,
                FillOpacity = fillOpacity,
                StrokeDash = strokeDash == null ? new List<double>() { 0 } : strokeDash
            };
        }

        /***************************************************/
    }
}
