using BH.oM.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.SVG
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ToSVGString(this List<SVGDocument> svgDocuments)
        {
            BoundingBox box = svgDocuments.GetBounds();

            string Width = (box.Max.X - box.Min.X).ToString();
            string Height = (box.Max.Y - box.Min.Y).ToString();

            string canvasString = "<svg width=\"_width\" height=\"_height\"/>" + Environment.NewLine;

            canvasString = canvasString.Replace("_width", Width);
            canvasString = canvasString.Replace("_height", Height);

            canvasString += "\" </svg>";

            return canvasString;
        }

        public static string ToSVGString(this List<SVGObject> svgObjects)
        {
            string geometryString = "";

            for (int i = 0; i < svgObjects.Count; i++)
            {
                geometryString += Convert.IToSVG(svgObjects[i].Geometry);
            }

            return geometryString;
        }

        public static string ToSVGString(this List<SVGStyle> svgStyles)
        {
            string styleString = "";

            for (int i = 0; i < svgStyles.Count; i++)
            {
                string StyleString = " stroke-width=\"_stroke-width\" stroke-color=\"_stroke-color\" fill-color=\"_fill-color\" stroke-opacity=\"_stroke-opacity\" fill-opacity=\"_fill-opacity\" stroke-dasharray=\"_stroke-dasharray\" ";

                styleString = styleString.Replace("_stroke-width", svgStyles[i].StrokeWidth.ToString());
                styleString = styleString.Replace("_stroke-color", svgStyles[i].StrokeColor.ToString());
                styleString = styleString.Replace("_fill-color", svgStyles[i].FillColor.ToString());
                styleString = styleString.Replace("_stroke-opacity", svgStyles[i].StrokeOpacity.ToString());
                styleString = styleString.Replace("_fill-opacity", svgStyles[i].FillOpacity.ToString());
                styleString = styleString.Replace("_stroke-dasharray", svgStyles[i].StrokeDash.ToString());

                styleString += StyleString;
            }

            return styleString;
        }
    }
}
