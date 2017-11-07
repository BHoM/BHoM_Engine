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

            string circleString = "<circle cx=\"_cx\" cy=\"_cy\" r=\"radius\"/>";
            double Rad = 0.5;

            circleString = circleString.Replace("_cx", point.X.ToString());
            circleString = circleString.Replace("_cy", point.Y.ToString());
            circleString = circleString.Replace("radius", Rad.ToString());

            return circleString;
        }

        public static string ToSVG(this Line line)
        {
            Point startPt = line.Start;
            Point endPt = line.End;

            string lineString = "<line x1=\"_x1\" y1=\"_y1\" x2=\"_x2\" y2=\"_y2\"/>";

            lineString = lineString.Replace("_x1", startPt.X.ToString());
            lineString = lineString.Replace("_y1", startPt.Y.ToString());
            lineString = lineString.Replace("_x2", endPt.X.ToString());
            lineString = lineString.Replace("_y2", endPt.Y.ToString());

            return lineString;
        }

        public static string ToSVG(this Circle circle)
        {
            Point centerPt = circle.Centre;

            string circleString = "<circle cx=\"_cx\" cy=\"_cy\" r=\"radius\"/>";

            circleString = circleString.Replace("_cx", centerPt.X.ToString());
            circleString = circleString.Replace("_cy", centerPt.Y.ToString());
            circleString = circleString.Replace("radius", circle.Radius.ToString());

            return circleString;
        }

        public static string ToSVG(this Ellipse ellipse)
        {
            Point centerPt = ellipse.Centre;

            string ellipseString = "<ellipse cx=\"_cx\" cy=\"_cy\" rx=\"xRadius\" ry=\"yRadius\"/>";

            ellipseString = ellipseString.Replace("_cx", centerPt.X.ToString());
            ellipseString = ellipseString.Replace("_cy", centerPt.Y.ToString());
            ellipseString = ellipseString.Replace("xRadius", ellipse.Radius1.ToString());
            ellipseString = ellipseString.Replace("yRadius", ellipse.Radius2.ToString());

            return ellipseString;
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
            List<Point> controlPts = nurbCurve.ControlPoints;
            List<Double> weights = nurbCurve.Weights;
            List<Double> knots = nurbCurve.Knots;

            string nurbString = "<path d=\"";

            //    //List<Point> newControlPoints = new List<Point>();
            //    //if (controlPts.Count > 4)
            //    //{
            //    //    List<Point> newControlPts = new List<Point>();
            //    //    newControlPts = Geometry.Transform.GetDivided(nurbCurve, 2);
            //    //    newControlPoints = newControlPts;
            //    //}
            //    //else
            //    //{
            //    //    newControlPoints = controlPts; 
            //    //}

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

        public static string ToSVG(this Arc arc)
        {
            throw new NotImplementedException();
        }
    }
}
