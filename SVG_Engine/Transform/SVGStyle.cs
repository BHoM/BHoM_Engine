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
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string SVGStyleToString(this SVGStyle svgStyle)
        {
            string StyleString = "stroke-width=\"_stroke-width\" stroke-color=\"_stroke-color\" fill-color=\"_fill-color\" stroke-opacity=\"_stroke-opacity\" fill-opacity=\"_fill-opacity\" stroke-dasharray=\"_stroke-dasharray\" ";

            StyleString.Replace("_stroke-width", svgStyle.StrokeWidth.ToString());
            StyleString.Replace("_stroke-color", svgStyle.StrokeColor.ToString());
            StyleString.Replace("_fill-color", svgStyle.FillColor.ToString());
            StyleString.Replace("_stroke-opacity", svgStyle.StrokeOpacity.ToString());
            StyleString.Replace("_fill-opacity", svgStyle.FillOpacity.ToString());

            string DashArray = "";

            for (int i = 0; i < svgStyle.StrokeDash.Count; i++)
            {
                string a = svgStyle.StrokeDash[i].ToString();
                DashArray += a + " ";
            }

            StyleString.Replace("_stroke-dasharray", DashArray);

            return StyleString;
        }

        public static string SVGStyleToString(this List<SVGStyle> svgStyles)
        {
            string styleString = "";

            for (int i = 0; i < svgStyles.Count; i++)
            {
                string StyleString = "stroke-width=\"_stroke-width\" stroke-color=\"_stroke-color\" fill-color=\"_fill-color\" stroke-opacity=\"_stroke-opacity\" fill-opacity=\"_fill-opacity\" stroke-dasharray=\"_stroke-dasharray\" ";

                styleString = styleString.Replace("_stroke-width", svgStyles[i].StrokeWidth.ToString());
                styleString = styleString.Replace("_stroke-color", svgStyles[i].StrokeColor.ToString());
                styleString = styleString.Replace("_fill-color", svgStyles[i].FillColor.ToString());
                styleString = styleString.Replace("_stroke-opacity", svgStyles[i].StrokeOpacity.ToString());
                styleString = styleString.Replace("_fill-opacity", svgStyles[i].FillOpacity.ToString());

                string DashArray = "";

                for (int j = 0; i < svgStyles[i].StrokeDash.Count; i++)
                {
                    string a = svgStyles[i].StrokeDash[j].ToString();
                    DashArray += a + " ";
                }

                styleString = styleString.Replace("_stroke-dasharray", DashArray);

                styleString += StyleString;
            }
            return styleString;
        }

        //public static SVGStyle SVGStyle(double strokeWidth, string strokeColor, string fillColor, double strokeOpacity, double fillOpacity, double strokeDash)
        //{
        //    SVGStyle Style = new oM.Graphics.SVGStyle();
        //    SVGObject svg = new SVGObject();

        //    Style.StrokeWidth = strokeWidth;
        //    Style.StrokeColor = strokeColor;
        //    Style.FillColor = fillColor;
        //    Style.StrokeOpacity = strokeOpacity;
        //    Style.FillOpacity = fillOpacity;
        //    Style.StrokeDash = strokeDash;

        //    return Style;
        //}
    }
}
