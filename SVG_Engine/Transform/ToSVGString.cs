using BH.oM.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ToSVGString(SVGDocument svgDocument)
        {
            BoundingBox box = Query.GetSvgBounds(svgDocument);

            string Width = (box.Max.X - box.Min.X).ToString();
            string Height = (box.Max.Y - box.Min.Y).ToString();

            string canvasString = "<svg width=\"_width\" height=\"_height\"/>" + Environment.NewLine;

            canvasString = canvasString.Replace("_width", Width);
            canvasString = canvasString.Replace("_height", Height);

            for (int i = 0; i < svgDocument.SVGObjects.Count; i++)
            {
                canvasString += ToSVGString((svgDocument.SVGObjects[i]));
            }

            canvasString += "\" </svg>";

            return canvasString;
        }

        public static string ToSVGString(SVGObject svgObject)
        {
            string geometryString = "<g \"__Style__\">" + Environment.NewLine;

            geometryString.Replace("__Style__", "");

            for (int i = 0; i < svgObject.Geometry.Count; i++)
            {
                geometryString += svgObject.Geometry[i].IToSVG() + Environment.NewLine;
            }
            geometryString += "</g>" + Environment.NewLine;

            return geometryString;
        }

        public static string ToSVGString(SVGStyle svgStyle)
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

        public static string ToSVGString(List<SVGStyle> svgStyles)
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























        //public static string ToSVGString(this SVGStyle svgStyle)
        //{
        //    string StyleString = "stroke-width=\"_stroke-width\" stroke-color=\"_stroke-color\" fill-color=\"_fill-color\" stroke-opacity=\"_stroke-opacity\" fill-opacity=\"_fill-opacity\" stroke-dasharray=\"_stroke-dasharray\" ";

        //    StyleString.Replace("_stroke-width", svgStyle.StrokeWidth.ToString());
        //    StyleString.Replace("_stroke-color", svgStyle.StrokeColor.ToString());
        //    StyleString.Replace("_fill-color", svgStyle.FillColor.ToString());
        //    StyleString.Replace("_stroke-opacity", svgStyle.StrokeOpacity.ToString());
        //    StyleString.Replace("_fill-opacity", svgStyle.FillOpacity.ToString());
        //    StyleString.Replace("_stroke-dasharray", svgStyle.StrokeDash.ToString());

        //    return StyleString;
        //}

        //public static string ToSVGString(this List<SVGStyle> svgStyles)
        //{
        //    string styleString = "";

        //    for (int i = 0; i < svgStyles.Count; i++)
        //    {
        //        string StyleString = "stroke-width=\"_stroke-width\" stroke-color=\"_stroke-color\" fill-color=\"_fill-color\" stroke-opacity=\"_stroke-opacity\" fill-opacity=\"_fill-opacity\" stroke-dasharray=\"_stroke-dasharray\" ";

        //        styleString = styleString.Replace("_stroke-width", svgStyles[i].StrokeWidth.ToString());
        //        styleString = styleString.Replace("_stroke-color", svgStyles[i].StrokeColor.ToString());
        //        styleString = styleString.Replace("_fill-color", svgStyles[i].FillColor.ToString());
        //        styleString = styleString.Replace("_stroke-opacity", svgStyles[i].StrokeOpacity.ToString());
        //        styleString = styleString.Replace("_fill-opacity", svgStyles[i].FillOpacity.ToString());
        //        styleString = styleString.Replace("_stroke-dasharray", svgStyles[i].StrokeDash.ToString());

        //        styleString += StyleString;
        //    }
        //    return styleString;
        //}

        //public static string ToSVGString(this SVGObject svgObject)
        //{
        //    string geometryString = "<g __Style__>" + System.Environment.NewLine;

        //    geometryString.Replace("__Style__", BH.Engine.SVG.Create.ToSVGString(svgObject.Style));

        //    for (int i = 0; i < svgObject.Geometry.Count; i++)
        //    {
        //        geometryString += Convert.IToSVG(svgObject.Geometry[i]);
        //    }
        //    geometryString += "</g>";

        //    return geometryString;
        //}


    }
}
