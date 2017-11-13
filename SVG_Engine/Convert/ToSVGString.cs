using BH.oM.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ToSVGString(SVGStyle svgStyle)
        {
            string styleString = "stroke-width=\"" + svgStyle.StrokeWidth.ToString()
                                 + "\" stroke=\"__stroke-color__\" fill=\"__fill-color__\" stroke-opacity=\""
                                 + svgStyle.StrokeOpacity.ToString() + "\" fill-opacity=\""
                                 + svgStyle.FillOpacity.ToString() + "\" stroke-dasharray=\"__stroke-dasharray__\"";

            string strokeColor = svgStyle.StrokeColor.ToString();

            Regex regex = new Regex(@"(\d{1,3}),(\d{1,3}),(\d{1,3})");
            Match matchStroke = regex.Match(strokeColor);

            if (matchStroke.Success)
            {
                string rgb1 = "rgb(" + svgStyle.StrokeColor.ToString() + ")";

                styleString = styleString.Replace("__stroke-color__", rgb1);
            }
            else
            {
                styleString = styleString.Replace("__stroke-color__", svgStyle.StrokeColor.ToString());
            }

            string fillColor = svgStyle.FillColor.ToString();
            Match matchFill = regex.Match(fillColor);

            if (matchFill.Success)
            {
                string rgb2 = "rgb(" + svgStyle.FillColor.ToString() + ")";
                styleString = styleString.Replace("__fill-color__", rgb2);
            }
            else
            {
                styleString = styleString.Replace("__fill-color__", svgStyle.FillColor.ToString());
            }

            string DashArray = "";

            for (int i = 0; i < svgStyle.StrokeDash.Count; i++)
            {
                string a = svgStyle.StrokeDash[i].ToString();

                if (i == 0)
                    DashArray += a;
                else
                    DashArray += " " + a;
            }

            styleString = styleString.Replace("__stroke-dasharray__", DashArray);

            return styleString;
        }

        /***************************************************/

        public static string ToSVGString(SVGObject svgObject)
        {
            string geometryString = "<g " + ToSVGString(svgObject.Style) + ">\n";

            for (int i = 0; i < svgObject.Geometry.Count; i++)
            {
                geometryString += svgObject.Geometry[i].IToSVG();

                if (svgObject.Geometry.Count > 1)
                {
                    geometryString += "\n";
                }
            }

            if (svgObject.Geometry.Count == 1)
            {
                geometryString += "\n";
            }

            geometryString += "</g>\n";

            return geometryString;
        }

        /***************************************************/

        public static string ToSVGString(SVGDocument svgDocument)
        {
            BoundingBox box = Query.GetSvgBounds(svgDocument);

            double extraMargin = 10;

            string Width = (box.Max.X - box.Min.X + extraMargin).ToString();
            string Height = (box.Max.Y - box.Min.Y + extraMargin).ToString();

            string canvasString = "<svg width=\"" + Width + "\" height=\"" + Height + "\">\n";

            double halfMargin = extraMargin / 2;
            double h = (box.Max.Y - box.Min.Y + extraMargin);

            string xTrans = (-(box.Min.X - halfMargin)).ToString();
            string yTrans = (-(box.Min.Y - halfMargin)).ToString();

            canvasString += "<g transform=\"translate(" + "0," + h + ") scale(1,-1) translate(" + xTrans + "," + yTrans + ")\">\n";

            for (int i = 0; i < svgDocument.SVGObjects.Count; i++)
            {
                canvasString += ToSVGString((svgDocument.SVGObjects[i]));
            }

            canvasString += "</g>\n</svg>";

            return canvasString;
        }

    }
}
