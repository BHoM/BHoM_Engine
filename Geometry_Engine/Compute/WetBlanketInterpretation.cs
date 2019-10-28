using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Takes a List of Polylines and creates a `squished` version with the same area at every X-location, (and hence in total as well). Required for some calculations")]
        [Input("pLines", "defined counter clockwise for positive area, should be on the XY-plane")]
        [Output("C", "A single Polyline oriented counter clockwise with the same area as the sum of all the polylines")]
        public static Polyline WetBlanketInterpretation(List<Polyline> pLines)
        {
            double tol = Tolerance.Distance;

            List<double> xes = new List<double>();
            foreach (Polyline pLine in pLines)
                xes.AddRange(pLine.ControlPoints.Select(x => x.X));


            // sort and cull point x-positions
            // TODO the "6" should be based on Tolerance things
            List<double> xValues = (new HashSet<double>(xes.Select(x => Math.Round(x, 6)))).ToList();
            xValues.Sort();

            //foreach linesegment
            // split by x-value
            List<Polyline> polyLines = new List<Polyline>();
            for (int k = 0; k < pLines.Count; k++)
            {
                polyLines.Add(new Polyline());
                double dMin;
                double dMax;
                polyLines[k].ControlPoints.Add(pLines[k].ControlPoints[0]);
                for (int i = 0; i < pLines[k].ControlPoints.Count - 1; i++)
                {
                    bool dir = pLines[k].ControlPoints[i].X < pLines[k].ControlPoints[i + 1].X;
                    if (dir)
                    {
                        dMin = pLines[k].ControlPoints[i].X + tol;
                        dMax = pLines[k].ControlPoints[i + 1].X - tol;
                    }
                    else
                    {
                        dMin = pLines[k].ControlPoints[i + 1].X + tol;
                        dMax = pLines[k].ControlPoints[i].X - tol;
                    }
                    List<Point> temp = new List<Point>();
                    for (int j = 0; j < xValues.Count; j++)
                    {
                        if (xValues[j] < dMin)
                            continue;
                        if (xValues[j] > dMax)
                            break;

                        temp.Add(PointAtX(pLines[k].ControlPoints[i], pLines[k].ControlPoints[i + 1], xValues[j]));
                    }
                    if (!dir)
                        temp.Reverse();

                    polyLines[k].ControlPoints.AddRange(temp);
                    polyLines[k].ControlPoints.Add(pLines[k].ControlPoints[i + 1]);
                }
            }

            Polyline result = new Polyline();
            // add leftmost point at x-axis
            result.ControlPoints.Add(new Point() { X = xValues[0] });

            // foreach x-value
            for (int i = 0; i < xValues.Count; i++)
            {
                // get all points with the said x-value
                List<Point> coLinPoints = new List<Point>();
                for (int k = 0; k < pLines.Count; k++)
                {
                    for (int j = 0; j < polyLines[k].ControlPoints.Count; j++)
                    {
                        if (polyLines[k].ControlPoints[j].X < xValues[i] + tol && polyLines[k].ControlPoints[j].X > xValues[i] - tol)
                        {
                            Point pt = polyLines[k].ControlPoints[j];
                            pt.Z = j * 100 + k;   // Hide the index in the point (we only care about planar cases)
                            pt.X = xValues[i];  // Line em up (makes sure the sort works later (deafaluts to sort by x))
                            coLinPoints.Add(pt);
                        }
                    }
                }
                coLinPoints.Sort();

                // how much "area" there is at the imidiate left respectivly the right side of the x-value
                double LeftAreaLength = 0;
                double RightAreaLength = 0;

                for (int j = 0; j < coLinPoints.Count - 1; j++)
                {
                    Point current = coLinPoints[j];
                    int k = (int)(current.Z % 100);
                    int index = (int)Math.Floor(current.Z * 0.01);
                    int indexBefore = index - 1 < 0 ? polyLines[k].ControlPoints.Count - 2 : index - 1;
                    int indexAfter = index + 1 > polyLines[k].ControlPoints.Count - 1 ? 1 : index + 1;
                    Point before = polyLines[k].ControlPoints[indexBefore];
                    Point after = polyLines[k].ControlPoints[indexAfter];

                    double d = Math.Abs(coLinPoints[j + 1].Y - current.Y);
                    // saves the results in a int's first and second number for the switch
                    int cornerCase = 0;
                    cornerCase += Direction(current, after);
                    cornerCase += Direction(current, before) * 10;

                    // cornerCase is a mesure of how the corner is in a way that lets us get if there is "area" at the imidiate left/right "above" it

                    if (cornerCase < -1)
                    {
                        // current and one of the point are the same
                        // TODO handle this?
                        Engine.Reflection.Compute.RecordWarning(xValues[i].ToString());
                    }
                    else
                    {
                        switch (cornerCase)
                        {
                            case 20:
                            case 23:
                            case 30:
                                LeftAreaLength += d;
                                RightAreaLength += d;
                                break;
                            case 21:
                            case 1:
                            case 31:
                                LeftAreaLength += d;
                                break;
                            case 12:
                            case 10:
                            case 13:
                                RightAreaLength += d;
                                break;

                            case 22:
                                if (before.Y > after.Y)
                                {
                                    LeftAreaLength += d;
                                    RightAreaLength += d;
                                }
                                break;
                            case 0:
                                if (after.Y > before.Y)
                                {
                                    LeftAreaLength += d;
                                    RightAreaLength += d;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (Math.Abs(LeftAreaLength - RightAreaLength) < Tolerance.Distance)
                {
                    result.ControlPoints.Add(new Point() { X = xValues[i], Y = -LeftAreaLength });
                }
                else
                {
                    result.ControlPoints.Add(new Point() { X = xValues[i], Y = -LeftAreaLength });
                    result.ControlPoints.Add(new Point() { X = xValues[i], Y = -RightAreaLength });
                }
            }

            result.ControlPoints.Add(new Point() { X = xValues[xValues.Count - 1] }); // the right most point on the x-axis
            result.ControlPoints.Add(new Point() { X = xValues[0] }); // closing the curve

            return result;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static int Direction(Point center, Point a)
        {
            if (center.X + Tolerance.Distance < a.X)
                return 0;   //to the right
            if (center.X - Tolerance.Distance > a.X)
                return 2;   //to the left
            if (center.Y + Tolerance.Distance < a.Y)
                return 1;   // over
            if (center.Y - Tolerance.Distance > a.Y)
                return 3;   // under
            return -100;  // the same
        }

        /***************************************************/

        private static Point PointAtX(Point a, Point b, double X)
        {
            return new Point()
            {
                X = X,
                Y = ((b.Y - a.Y) * (X - a.X)
                                     / (b.X - a.X)
                                     + a.Y)
            };
        }

        /***************************************************/

    }
}
