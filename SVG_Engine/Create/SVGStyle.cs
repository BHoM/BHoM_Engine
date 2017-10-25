using System;
using BH.oM.Geometry;
using BH.Engine.SVG;
using BH.Engine.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Graphics;

namespace BH.Engine.SVG
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SVGStyle SVGStyle(double strokeWidth, string strokeColor, string fillColor, double strokeOpacity, double fillOpacity, double strokeDash)
        {
            SVGStyle Style = new oM.Graphics.SVGStyle();

            Style.StrokeWidth = strokeWidth;
            Style.StrokeColor = strokeColor;
            Style.FillColor = fillColor;
            Style.StrokeOpacity = strokeOpacity;
            Style.FillOpacity = fillOpacity;
            Style.StrokeDash = strokeDash;
            
            return Style;
        }
    }
}
