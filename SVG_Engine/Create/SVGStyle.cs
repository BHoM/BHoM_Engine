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
            SVGStyle style = new oM.Graphics.SVGStyle();

            style.StrokeWidth = strokeWidth;
            style.StrokeColor = strokeColor;
            style.FillColor = fillColor;
            style.StrokeOpacity = strokeOpacity;
            style.FillOpacity = fillOpacity;
            style.StrokeDash = strokeDash;
            
            return style;
        }

        //TO DO: List<SVGStyle>
    }
}
