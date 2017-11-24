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
                                  + "\" r=\"" + circle.Radius.ToString() +"\"/>";

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
                                   + "\" transform=\"rotate(" + ((Geometry.Query.GetAngle(ellipse.Axis1, Vector.XAxis)) * 180 / Math.PI).ToString() 
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
