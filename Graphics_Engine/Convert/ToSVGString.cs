/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BH.Engine.Geometry;

namespace BH.Engine.Graphics
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods - Graphics                 ****/
        /***************************************************/

        public static string ToSVGString(this SVGStyle svgStyle)
        {
            if (svgStyle == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null SVG Style.");
                return "";
            }

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

        public static string ToSVGString(this SVGObject svgObject)
        {
            if (svgObject == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null SVG object.");
                return "";
            }

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

        public static string ToSVGString(this SVGDocument svgDocument)
        {
            if (svgDocument == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null SVG Document.");
                return "";
            }

            BoundingBox box = Query.Bounds(svgDocument);

            string Width = (box.Max.X - box.Min.X).ToString();
            string Height = (box.Max.Y - box.Min.Y).ToString();

            string canvasString = "<svg width=\"" + Width + "\" height=\"" + Height + "\">\n";

            double h = (box.Max.Y - box.Min.Y);

            string xTrans = (-box.Min.X).ToString();
            string yTrans = (-box.Min.Y).ToString();

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

        public static string IToSVGString(this IGeometry geometry)
        {
            return ToSVGString(geometry as dynamic);
        }

        /***************************************************/

        public static string ToSVGString(this Point point)
        {
            if (point == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null point to SVG string.");
                return "";
            }

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
            if (line == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null line to SVG string.");
                return "";
            }

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
            if (circle == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null circle to SVG string.");
                return "";
            }

            Point centerPt = circle.Centre;

            string circleString = "<circle cx=\"" + centerPt.X.ToString()
                                  + "\" cy=\"" + centerPt.Y.ToString()
                                  + "\" r=\"" + circle.Radius.ToString() + "\"/>";

            return circleString;
        }


        /***************************************************/

        public static string ToSVGString(this Arc arc)
        {
            if (arc == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null arc to SVG string.");
                return "";
            }

            int largeArcFlag = System.Convert.ToInt32((arc.Angle() > Math.PI));
            int sweepFlag = System.Convert.ToInt32(!arc.IsClockwise(Vector.ZAxis));
            Point start = arc.StartPoint();
            Point end = arc.EndPoint();
            string arcString = "<path d=\"M" + start.X.ToString()
                                + "," + start.Y.ToString()
                                + " A" + arc.Radius + "," + arc.Radius
                                + " 0"
                                + " " + largeArcFlag
                                + "," + sweepFlag
                                + " " + end.X.ToString()
                                + "," + end.Y.ToString() + "\"/>";

            return arcString;
        }

        /***************************************************/

        public static string ToSVGString(this Ellipse ellipse)
        {
            if (ellipse == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null ellipse to SVG string.");
                return "";
            }

            Point centerPt = ellipse.CoordinateSystem.Origin;

            string ellipseString = "<ellipse cx=\"" + centerPt.X.ToString()
                                   + "\" cy=\"" + centerPt.Y.ToString()
                                   + "\" rx=\"" + ellipse.Radius1.ToString()
                                   + "\" ry=\"" + ellipse.Radius2.ToString()
                                   + "\" transform=\"rotate(" + ((Engine.Geometry.Query.Angle(ellipse.CoordinateSystem.X, Vector.XAxis)) * 180 / Math.PI).ToString()
                                   + " " + centerPt.X.ToString()
                                   + " " + centerPt.Y.ToString() + ")\"/>";

            return ellipseString;
        }

        /***************************************************/

        public static string ToSVGString(this Polyline polyline)
        {
            if (polyline == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert null polyline to SVG string.");
                return "";
            }

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

        public static string ToSVGString(this NurbsCurve nurbCurve)
        {
            if (nurbCurve == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert null nurb curve to SVG string.");
                return "";
            }

            // TODO : SVG_Engine - Further developing of the method for converting NurbsCurves to SVG 

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
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static string ToSVGString(this IGeometry geometry)
        {
            if (geometry == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null to SVG string.");
                return "";
            }
            else
            {
                BH.Engine.Base.Compute.RecordError($"Cannot convert a {geometry.GetType().Name} to SVG string.");
                return "";
            }
        }

        /***************************************************/
    }
}





