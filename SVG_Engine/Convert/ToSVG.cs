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
            // Creates a small circle with the input point as its centerpoint and converts it into SVG

            string circleString = "<circle cx=\"_cx\" cy=\"_cy\" r=\"radius\" />";
            double Rad = 0.5;

            circleString = circleString.Replace("_cx", point.X.ToString());
            circleString = circleString.Replace("_cy", point.Y.ToString());
            circleString = circleString.Replace("radius", Rad.ToString());

            return circleString;

            // throw new NotImplementedException();
            //stroke=\"black\" stroke-width=\"\" opacity=\"\" 
        }

        public static string ToSVG(this Line line)
        {
            // Converts a BHoM Line into SVG

            Point startPt = line.Start;
            Point endPt = line.End;

            string lineString = "<line x1=\"_x1\" y1=\"_y1\" x2=\"_x2\" y2=\"_y2\" />" + System.Environment.NewLine;

            lineString = lineString.Replace("_x1", startPt.X.ToString());
            lineString = lineString.Replace("_y1", startPt.Y.ToString());
            lineString = lineString.Replace("_x2", endPt.X.ToString());
            lineString = lineString.Replace("_y2", endPt.Y.ToString());

            return lineString;

            //BH.oM.SVG.Object svgObject = new BH.oM.SVG.Object(lineString, boundingbox);
            //stroke=\"black\" stroke-width=\"\" opacity=\"\" 
        }

        public static string ToSVG(this Circle circle)
        {
            // Converts a BHoM Circle into SVG

            Point centerPt = circle.Centre;

            string circleString = "<circle cx=\"_cx\" cy=\"_cy\" r=\"radius\" />";

            circleString = circleString.Replace("_cx", centerPt.X.ToString());
            circleString = circleString.Replace("_cy", centerPt.Y.ToString());
            circleString = circleString.Replace("radius", circle.Radius.ToString());

            return circleString;

            //stroke=\"black\" stroke-width=\"\" fill=\"transparent\" stroke-opacity=\"\" fill-opacity=\"\" 
        }

        public static string ToSVG(this Ellipse ellipse)
        {
            // Converts a BHoM Ellipse into SVG

            Point centerPt = ellipse.Centre;

            string ellipseString = "<ellipse cx=\"_cx\" cy=\"_cy\" rx=\"xRadius\" ry=\"yRadius\" />";

            ellipseString = ellipseString.Replace("_cx", centerPt.X.ToString());
            ellipseString = ellipseString.Replace("_cy", centerPt.Y.ToString());
            ellipseString = ellipseString.Replace("xRadius", ellipse.XRadius.ToString());
            ellipseString = ellipseString.Replace("yRadius", ellipse.YRadius.ToString());

            return ellipseString;

            //stroke=\"black\" stroke-width=\"\" fill=\"transparent\" stroke-opacity=\"\" fill-opacity=\"\" 
        }

        public static string ToSVG(this List<BH.oM.Geometry.Point> ptList)
        {
            // Converts a Path into SVG

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

            return pathString;

            //, bool closed = false

            //if (closed)
            //{
            //    pathString += "Z";
            //}

            // pathString += "\" Stroke=\"black\" Stroke-width=\"\" Fill=\"transparent\" stroke-opacity=\"\" fill-opacity=\"\" />";
        }

        public static string ToSVG(this Polyline polyline)
        {
            // Converts a BHoM Polyline into SVG

            List<Point> controlPts = polyline.ControlPoints;

            string polylineString = "<polyline points=\" ";

            for (int i = 0; i < controlPts.Count; i++)
            {
                polylineString += controlPts[i].X.ToString() + "," + controlPts[i].Y.ToString() + " ";
            }

            //polylineString += "\" style=\"fill:none;stroke:black;stroke-width:;stroke-opacity:;fill-opacity:\" />"; <--- SPECIAL CASE

            return polylineString;
        }

        public static string ToSVG(this NurbCurve nurbCurve)
            {
            // Converts a nurbs curve into SVG

            List<Point> controlPts = nurbCurve.ControlPoints;
            List<Double> weights = nurbCurve.Weights;
            List<Double> knots = nurbCurve.Knots;

            string nurbString = "<path d=\"";

            double p = nurbCurve.ControlPoints.Count();
            bool a = new bool();

            //-----------------------------------------------

            if ( ((p-4)/3) % 1 == 0) // Cubic Curves
            {
                a = true;
            }
            else if ( ((p-3)/2) % 1 == 0) // Quadratic Curves
            {
                a = false;
            }
            else // If neither (one control point will not produce any geometry) - Choose Cubic
            {
                a = true;
            }

            //-----------------------------------------------

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
           
            // nurbString += "\" stroke=\"black\" stroke-width=\"\" fill=\"transparent\" stroke-opacity=\"\" fill-opacity=\"\" />";

            return nurbString;
            }

        public static string ToSVG(this Arc arc)
        {
            // Converts an arc into SVG

            //Point startPt = arc.Start;
            //Point midPt = arc.Middle;
            //Point endPt = arc.End;

            //Circle circle = new Circle();

            //String endPtX = arc.End.X.ToString();
            //String endPtY = arc.End.Y.ToString();

            //string arcString = "< path d=\" ";

            //Ex: < path d = "M50 200 A 30 50 0 0 1 162.55 162.45" />

            throw new NotImplementedException();
        }

        //public static string DrawSVGpath(this IBHoMGeometry geometry)
        //{
            
        //}

        //public static string ToSvg(this Arc arc)
        //{
        //    DrawSVGpath(arc);
        //}

        // Points, ellipses, arcs, nurbs (rectangles, polylines)
    }
}