/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
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
            double tolHalf = tol * 0.5;
            double tolDistance = Tolerance.Distance;

            int digits = (int)Math.Floor(-Math.Log10(tol)); //TODO fix for wider use

            List<double> xes = new List<double>();
            foreach (Polyline pLine in pLines)
            {
                foreach (Point pt in pLine.ControlPoints)
                    pt.X = Math.Round(pt.X, digits);

                xes.AddRange(pLine.ControlPoints.Select(x => x.X));
            }

            // sort and cull point x-positions
            List<double> xValues = (new HashSet<double>(xes)).ToList();
            xValues.Sort();

            List<Tuple<Point, int, int>> list = new List<Tuple<Point, int, int>>();
            //the Point, which Polyline, which index on the polyline

            List<Polyline> polyLines = new List<Polyline>();
            for (int k = 0; k < pLines.Count; k++)
            {
                polyLines.Add(SplitPolylineAtXValues(pLines[k], ref list, xValues, k, tolHalf));
            }

            // Orders it primarily by X and secundaryly by Y
            list.Sort(delegate (Tuple<Point, int, int> t1, Tuple<Point, int, int> t2)
            { return t1.Item1.CompareTo(t2.Item1); });

            Polyline result = new Polyline();

            // how much "area" there is at the imidiate left respectivly the right side of the x-value
            double leftAreaLength = 0;
            double rightAreaLength = 0;
            Point before, after;
            double d;

            for (int i = 0; i < list.Count; i++)
            {
                // the last in a x "tier" should be ignored"
                if (i == list.Count - 1 || Math.Abs(list[i + 1].Item1.X - list[i].Item1.X) > tolHalf)
                {
                    result.ControlPoints.Add(new Point() { X = list[i].Item1.X });
                    continue;
                }

                GetLocationData(list, polyLines, i, out before, out after, out d);

                AreaLengthFromPoints(list[i].Item1, before, after, d, tolHalf, ref leftAreaLength, ref rightAreaLength);

                if ((i + 2) == list.Count || Math.Abs(list[i + 2].Item1.X - list[i].Item1.X) > tolHalf)
                {
                    if (Math.Abs(leftAreaLength - rightAreaLength) < tolDistance)
                    {
                        result.ControlPoints.Add(new Point() { X = list[i].Item1.X, Y = -leftAreaLength });
                    }
                    else
                    {
                        result.ControlPoints.Add(new Point() { X = list[i].Item1.X, Y = -leftAreaLength });
                        result.ControlPoints.Add(new Point() { X = list[i].Item1.X, Y = -rightAreaLength });
                    }
                    leftAreaLength = 0;
                    rightAreaLength = 0;

                    i++;
                }
            }

            result.ControlPoints.Add(new Point() { X = xValues[0] }); // closing the curve

            return result;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void GetLocationData(List<Tuple<Point, int, int>> list, List<Polyline> polyLines, int i, out Point before, out Point after, out double d)
        {
            int k = list[i].Item2;
            int index = list[i].Item3;
            int indexBefore = index - 1 < 0 ? polyLines[k].ControlPoints.Count - 2 : index - 1;
            int indexAfter = index + 1 > polyLines[k].ControlPoints.Count - 1 ? 1 : index + 1;
            before = polyLines[k].ControlPoints[indexBefore];
            after = polyLines[k].ControlPoints[indexAfter];

            d = Math.Abs(list[i + 1].Item1.Y - list[i].Item1.Y);
        }

        /***************************************************/

        private static Polyline SplitPolylineAtXValues(Polyline pLine, ref List<Tuple<Point, int, int>> list, List<double> xValues, int k, double tol)
        {
            Polyline polyline = new Polyline();
            double dMin, dMax;

            polyline.ControlPoints.Add(pLine.ControlPoints[0]); //Add first of linesegment (only added here)
            list.Add(new Tuple<Point, int, int>(pLine.ControlPoints[0], k, polyline.ControlPoints.Count - 1));
            for (int i = 0; i < pLine.ControlPoints.Count - 1; i++)
            {
                Point first = pLine.ControlPoints[i];
                Point last = pLine.ControlPoints[i + 1];
                bool dir = first.X < last.X;
                if (dir)
                {
                    dMin = first.X + tol;
                    dMax = last.X - tol;
                    for (int j = 0; j < xValues.Count; j++) //could switch for a while loop?
                    {
                        if (xValues[j] < dMin)
                            continue;
                        if (xValues[j] > dMax)
                            break;

                        Point pt = PointAtX(pLine.ControlPoints[i], pLine.ControlPoints[i + 1], xValues[j]);
                        polyline.ControlPoints.Add(pt);
                        list.Add(new Tuple<Point, int, int>(pt, k, polyline.ControlPoints.Count - 1));
                    }
                }
                else
                {
                    dMin = last.X + tol;
                    dMax = first.X - tol;
                    for (int j = xValues.Count - 1; j >= 0; j--)
                    {
                        if (xValues[j] > dMax)
                            continue;
                        if (xValues[j] < dMin)
                            break;

                        Point pt = PointAtX(pLine.ControlPoints[i], pLine.ControlPoints[i + 1], xValues[j]);
                        polyline.ControlPoints.Add(pt);
                        list.Add(new Tuple<Point, int, int>(pt, k, polyline.ControlPoints.Count - 1));
                    }
                }

                polyline.ControlPoints.Add(pLine.ControlPoints[i + 1]); //Add last point of linesegment (within the loop)
                list.Add(new Tuple<Point, int, int>(pLine.ControlPoints[i + 1], k, polyline.ControlPoints.Count - 1));
            }
            return polyline;
        }

        /***************************************************/

        private static void AreaLengthFromPoints(this Point current, Point before, Point after, double d, double tol, ref double leftAreaLength, ref double rightAreaLength)
        {
            // saves the results in a int's first and second number for the switch
            int cornerCase = 0;
            cornerCase += Direction(current, after, tol);
            cornerCase += Direction(current, before, tol) * 10;

            // cornerCase is a mesure of how the corner is in a way that lets us get if there is "area" at the imidiate left/right "above" it

            if (cornerCase < -1)
            {
                // list[i] and one of the point are the same
                // TODO handle this?    
                // would ideally take another step along the polyline in the direction
                Engine.Reflection.Compute.RecordWarning(
                    "WetBlanketInterpretation issue, points ontop of eachother");
            }
            else
            {
                switch (cornerCase)
                {
                    case 20:
                    case 23:
                    case 30:
                        leftAreaLength += d;
                        rightAreaLength += d;
                        break;
                    case 21:
                    case 1:
                    case 31:
                        leftAreaLength += d;
                        break;
                    case 12:
                    case 10:
                    case 13:
                        rightAreaLength += d;
                        break;

                    case 22:
                        if (before.Y > after.Y)
                        {
                            leftAreaLength += d;
                            rightAreaLength += d;
                        }
                        break;
                    case 0:
                        if (after.Y > before.Y)
                        {
                            leftAreaLength += d;
                            rightAreaLength += d;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /***************************************************/

        private static int Direction(Point center, Point a, double tol = Tolerance.Distance)
        {
            if (center.X + tol < a.X)
                return 0;   //to the right
            if (center.X - tol > a.X)
                return 2;   //to the left
            if (center.Y + tol < a.Y)
                return 1;   // over
            if (center.Y - tol > a.Y)
                return 3;   // under
            return -100;  // the same
        }

        /***************************************************/

        public static Point PointAtX(Point a, Point b, double x)    //Public??
        {
            return new Point()
            {
                X = x,
                Y = ((b.Y - a.Y) * (x - a.X)
                                     / (b.X - a.X)
                                     + a.Y)
            };
        }

        /***************************************************/

    }
}
