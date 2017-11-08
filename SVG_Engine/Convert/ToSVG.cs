using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string IToSVG(this IBHoMGeometry geometry)
        {
            return ToSVG(geometry as dynamic);
        }

        public static string ToSVG(this Point point)
        {
            // Creates a one pixle wide circle for the point in order for it to be displayable

            string circleString = "<circle cx=\"__cx__\" cy=\"__cy__\" r=\"__radius__\"/>";
            double Rad = 0.5;

            circleString = circleString.Replace("__cx__", point.X.ToString())
                                       .Replace("__cy__", point.Y.ToString())
                                       .Replace("__radius__", Rad.ToString());

            return circleString;
        }

        public static string ToSVG(this Line line)
        {
            Point startPt = line.Start;
            Point endPt = line.End;

            string lineString = "<line x1=\"__x1__\" y1=\"__y1__\" x2=\"__x2__\" y2=\"__y2__\"/>";

            lineString = lineString.Replace("__x1__", startPt.X.ToString())
                                   .Replace("__y1__", startPt.Y.ToString())
                                   .Replace("__x2__", endPt.X.ToString())
                                   .Replace("__y2__", endPt.Y.ToString());

            return lineString;
        }

        public static string ToSVG(this Circle circle)
        {
            Point centerPt = circle.Centre;

            string circleString = "<circle cx=\"__cx__\" cy=\"__cy__\" r=\"__radius__\"/>";

            circleString = circleString.Replace("__cx__", centerPt.X.ToString())
                                       .Replace("__cy__", centerPt.Y.ToString())
                                       .Replace("__radius__", circle.Radius.ToString());

            return circleString;
        }

        public static string ToSVG(this Ellipse ellipse)
        {
            Point centerPt = ellipse.Centre;

            string ellipseString = "<ellipse cx=\"__cx__\" cy=\"__cy__\" rx=\"__xRadius__\" ry=\"__yRadius__\"";

            ellipseString = ellipseString.Replace("__cx__", centerPt.X.ToString())
                                         .Replace("__cy__", centerPt.Y.ToString())
                                         .Replace("__xRadius__", ellipse.Radius1.ToString())
                                         .Replace("__yRadius__", ellipse.Radius2.ToString());

            ellipseString += " transform=\"rotate(__angle__ __cX__ __cY__)\"";

            ellipseString = ellipseString.Replace("__angle__", ((Geometry.Query.GetAngle(ellipse.Axis1, Vector.XAxis)) * 180 / Math.PI).ToString())
                                         .Replace("__cX__", centerPt.X.ToString())
                                         .Replace("__cY__", centerPt.Y.ToString());

            ellipseString += "/>";

            return ellipseString;
        }

        public static string ToSVG(this Polyline polyline)
        {
            List<Point> controlPts = polyline.ControlPoints;

            string polylineString = "<polyline points=\"";

            for (int i = 0; i < controlPts.Count; i++)
            {
                polylineString += controlPts[i].X.ToString() + "," + controlPts[i].Y.ToString() + " ";
            }

            polylineString += "\"/>";

            return polylineString;
        }

        public static string ToSVG(this NurbCurve nurbCurve)
        {
            // TODO : SVG_Engine - Simplified method at the moment which needs further development

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
                    nurbString += "M" + controlPts[i].X.ToString() + " " + controlPts[i].Y.ToString();

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
                    nurbString += controlPts[i].X.ToString() + " " + controlPts[i].Y.ToString() + " ";
                }
            }

            nurbString += "\"/>";

            return nurbString;
        }

        public static string ToSVG(this List<Point> ptList)
        {
            string pathString = "<path d=\"";

            for (int i = 0; i < ptList.Count; i++)
            {
                if (i == 0)
                {
                    pathString += "M" + ptList[i].X.ToString() + " " + ptList[i].Y.ToString() + " L";
                }
                else
                {
                    pathString += ptList[i].X.ToString() + " " + ptList[i].Y.ToString() + " ";
                }
            }

            pathString += "\"/>";

            return pathString;
        }

        public static string ToSVG(this Arc arc)
        {
            // TODO : SVG_Engine - Implement conversion method for arc

            throw new NotImplementedException();
        }
    }
}
