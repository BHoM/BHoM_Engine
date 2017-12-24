using BH.oM.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BH.Engine.Graphics
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods - Graphics                 ****/
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

            for (int i = 0; i < svgObject.Shapes.Count; i++)
            {
                geometryString += svgObject.Shapes[i].IToSVGString();

                if (svgObject.Shapes.Count > 1)
                {
                    geometryString += "\n";
                }
            }

            if (svgObject.Shapes.Count == 1)
            {
                geometryString += "\n";
            }

            geometryString += "</g>\n";

            return geometryString;
        }

        /***************************************************/

        public static string ToSVGString(SVGDocument svgDocument)
        {
            BoundingBox box = Query.GetBounds(svgDocument);

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


        /***************************************************/
        /**** Public Methods - Geometry                 ****/
        /***************************************************/

        public static string IToSVGString(this IBHoMGeometry geometry)
        {
            return ToSVGString(geometry as dynamic);
        }

        /***************************************************/

        public static string ToSVGString(this Point point)
        {
            // Creates a one pixle wide circle for the point in order for it to be displayable

            double Rad = 0.5;
            string circleString = "<circle cx=\"" + point.X.ToString()
                                  + "\" cy=\"" + point.Y.ToString()
                                  + "\" r=\"" + Rad.ToString() + "\"/>";

            return circleString;
        }

        /***************************************************/

        public static string ToSVGString(this Line line)
        {
            Point startPt = line.Start;
            Point endPt = line.End;

            string lineString = "<line x1=\"" + startPt.X.ToString()
                                + "\" y1=\"" + startPt.Y.ToString()
                                + "\" x2=\"" + endPt.X.ToString()
                                + "\" y2=\"" + endPt.Y.ToString() + "\"/>";

            return lineString;
        }

        /***************************************************/

        public static string ToSVGString(this Circle circle)
        {
            Point centerPt = circle.Centre;

            string circleString = "<circle cx=\"" + centerPt.X.ToString()
                                  + "\" cy=\"" + centerPt.Y.ToString()
                                  + "\" r=\"" + circle.Radius.ToString() + "\"/>";

            return circleString;
        }

        /***************************************************/

        public static string ToSVGString(this Ellipse ellipse)
        {
            Point centerPt = ellipse.Centre;

            string ellipseString = "<ellipse cx=\"" + centerPt.X.ToString()
                                   + "\" cy=\"" + centerPt.Y.ToString()
                                   + "\" rx=\"" + ellipse.Radius1.ToString()
                                   + "\" ry=\"" + ellipse.Radius2.ToString()
                                   + "\" transform=\"rotate(" + ((Engine.Geometry.Query.GetAngle(ellipse.Axis1, Vector.XAxis)) * 180 / Math.PI).ToString()
                                   + " " + centerPt.X.ToString()
                                   + " " + centerPt.Y.ToString() + ")\"/>";

            return ellipseString;
        }

        /***************************************************/

        public static string ToSVGString(this Polyline polyline)
        {
            List<Point> controlPts = polyline.ControlPoints;

            string polylineString = "<polyline points=\"";

            for (int i = 0; i < controlPts.Count; i++)
            {
                if (i == 0)
                {
                    polylineString += controlPts[i].X.ToString() + " " + controlPts[i].Y.ToString();
                }
                else
                {
                    polylineString += " " + controlPts[i].X.ToString() + " " + controlPts[i].Y.ToString();
                }
            }

            polylineString += "\"/>";

            return polylineString;
        }

        /***************************************************/

        public static string ToSVGString(this NurbCurve nurbCurve)
        {
            // TODO : SVG_Engine - Further developing of the method for converting NurbCurves to SVG 

            List<Point> controlPts = nurbCurve.ControlPoints;
            List<Double> weights = nurbCurve.Weights;
            List<Double> knots = nurbCurve.Knots;

            string nurbString = "<path d=\"";

            double p = nurbCurve.ControlPoints.Count();
            bool a = new bool();

            if (((p - 4) / 3) % 1 == 0) // Cubic Curves
            {
                a = true;
            }
            else if (((p - 3) / 2) % 1 == 0) // Quadratic Curves
            {
                a = false;
            }
            else // If neither (one control point will not produce any geometry) - Choose Cubic
            {
                a = true;
            }

            for (int i = 0; i < controlPts.Count; i++)
            {
                if (i == 0)
                {
                    nurbString += "M " + controlPts[i].X.ToString() + " " + controlPts[i].Y.ToString();

                    if (a == true)
                    {
                        nurbString += " C";
                    }
                    else
                    {
                        nurbString += " Q";
                    }
                }
                else
                {
                    nurbString += " " + controlPts[i].X.ToString() + " " + controlPts[i].Y.ToString();
                }
            }

            nurbString += "\"/>";

            return nurbString;
        }

        /***************************************************/

        public static string ToSVGString(this Arc arc)
        {
            // TODO : SVG_Engine - Implement conversion method for arc

            throw new NotImplementedException();
        }
    }
}
