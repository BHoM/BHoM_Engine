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

        [Description("")]
        [Input("pLine", "defined counter clockwise")]
        [Output("C", "Polyline")]
        public static Polyline WetBlanketInterpretation(Polyline pLine)
        {
            double tol = Tolerance.Distance;

            // sort and cull point x-positions
            // TODO the "6" should be based on Tolerance things
            List<double> xValues = (new HashSet<double>(pLine.ControlPoints.Select(x => Math.Round(x.X, 6)))).ToList();
            xValues.Sort();

            //foreach linesegment
            // split by x-value
            Polyline polyLine = new Polyline();
            double dMin;
            double dMax;
            polyLine.ControlPoints.Add(pLine.ControlPoints[0]);
            for (int i = 0; i < pLine.ControlPoints.Count - 1; i++)
            {
                bool dir = pLine.ControlPoints[i].X < pLine.ControlPoints[i + 1].X;
                if (dir)
                {
                    dMin = pLine.ControlPoints[i].X + tol;
                    dMax = pLine.ControlPoints[i + 1].X - tol;
                }
                else
                {
                    dMin = pLine.ControlPoints[i + 1].X + tol;
                    dMax = pLine.ControlPoints[i].X - tol;
                }
                List<Point> temp = new List<Point>();
                for (int j = 0; j < xValues.Count; j++)
                {
                    if (xValues[j] < dMin)
                        continue;
                    if (xValues[j] > dMax)
                        break;

                    temp.Add(PointAtX(pLine.ControlPoints[i], pLine.ControlPoints[i + 1], xValues[j]));
                }
                if (!dir)
                    temp.Reverse();

                polyLine.ControlPoints.AddRange(temp);
                polyLine.ControlPoints.Add(pLine.ControlPoints[i + 1]);
            }

            Polyline result = new Polyline();
            result.ControlPoints.Add(new Point() { X = xValues[0] });
            // foreach x-value
            for (int i = 0; i < xValues.Count; i++)
            {
                List<Point> coLinPoints = new List<Point>();

                for (int j = 0; j < polyLine.ControlPoints.Count; j++)
                {
                    if (polyLine.ControlPoints[j].X < xValues[i] + tol && polyLine.ControlPoints[j].X > xValues[i] - tol)
                    {
                        Point pt = polyLine.ControlPoints[j];
                        pt.Z = j;   // Hide the index in the point
                        pt.X = xValues[i];  // Line em up (makes sure the sort works later (deafaluts to sort by x))
                        coLinPoints.Add(pt);
                    }
                }

                coLinPoints.Sort();

                double LeftAreaLength = 0;
                double RightAreaLength = 0;

                for (int j = 0; j < coLinPoints.Count - 1; j++)
                {
                    Point current = coLinPoints[j];
                    int indexBefore = (int)current.Z - 1 < 0 ? polyLine.ControlPoints.Count - 2 : (int)current.Z - 1;
                    int indexAfter = (int)current.Z + 1 > polyLine.ControlPoints.Count - 1 ? 1 : (int)current.Z + 1;
                    Point before = polyLine.ControlPoints[indexBefore];
                    Point after = polyLine.ControlPoints[indexAfter];

                    double d = Math.Abs(coLinPoints[j + 1].Y - current.Y);
                    int cornerCase = 0;
                    cornerCase += Direction(current, after);
                    cornerCase += Direction(current, before) * 10;
                    if (cornerCase < -1)
                    {
                        // current and one of the point are the same
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

            result.ControlPoints.Add(new Point() { X = xValues[xValues.Count - 1] });
            result.ControlPoints.Add(new Point() { X = xValues[0] });

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
