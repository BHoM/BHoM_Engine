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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Takes a List of Polylines and creates a `squished` version with the same area at every X-location, and hence the same total area as well. Required for some calculations.")]
        [Input("pLines", "the outermost Polyline must be counter-clockwise. Clockwise ones are holes within it. Both Polylines should be on the XY-plane.")]
        [Input("tol", "The tolerance for considering two points as one. Note: resulting points will be moved to be on a multiple of the tolerance for its x-value.", typeof(Length))]
        [Output("C", "A single Polyline oriented counter clockwise with the same area as the sum of all the polylines.")]
        public static Polyline WetBlanketInterpretation(List<Polyline> pLines, double tol = Tolerance.Distance)
        {
            List<Polyline> clones = pLines.Select(x => x.RemoveShortSegments(tol, tol)).ToList();

            int digits = (int)Math.Floor(-Math.Log10(tol));

            List<decimal> xes = new List<decimal>();
            foreach (Polyline pLine in clones)
            {
                foreach (Point pt in pLine.ControlPoints)
                    pt.X = System.Convert.ToDouble(Math.Round(System.Convert.ToDecimal(pt.X), digits));

                xes.AddRange(pLine.ControlPoints.Select(x => System.Convert.ToDecimal(x.X)));
            }

            // sort and cull point x-positions
            List<double> xValues = xes.Distinct().Select(x => System.Convert.ToDouble(x)).ToList();
            xValues.Sort();

            List<Tuple<Point, int, int>> list = new List<Tuple<Point, int, int>>();
            //the Point, which Polyline, which index on the polyline

            List<Polyline> polyLines = new List<Polyline>();
            for (int k = 0; k < clones.Count; k++)
            {
                polyLines.Add(SplitPolylineAtXValues(clones[k], ref list, xValues, k, tol * 0.5));
            }

            // Orders it primarily by X and secundaryly by Y
            list.Sort(delegate (Tuple<Point, int, int> t1, Tuple<Point, int, int> t2)
                               { return t1.Item1.CompareTo(t2.Item1); });

            // Draw a new curve based on the area "covered" by the lines that are inside or outside of the region for each X-value
            return DrawBlanketCurve(list, polyLines, tol);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Polyline DrawBlanketCurve(List<Tuple<Point, int, int>> list, List<Polyline> polyLines, double tol = Tolerance.Distance)
        {
            Polyline result = new Polyline();
            double tolHalf = tol * 0.5;

            // how much "area" there is at the imidiate left respectivly the right side of the x-value
            double leftAreaLength = 0;
            double rightAreaLength = 0;
            Point before, after;
            double d;

            for (int i = 0; i < list.Count; i++)
            {
                // the last in a x "tier" should be ignored.
                if (i == list.Count - 1 || Math.Abs(list[i + 1].Item1.X - list[i].Item1.X) > tolHalf)   // last in list and single || single in tier
                {
                    result.ControlPoints.Add(new Point() { X = list[i].Item1.X });
                    continue;
                }

                GetLocationData(list, polyLines, i, out before, out after, out d);

                AreaLengthFromPoints(list[i].Item1, before, after, d, tolHalf, ref leftAreaLength, ref rightAreaLength);

                if ((i + 2) == list.Count || Math.Abs(list[i + 2].Item1.X - list[i].Item1.X) > tolHalf) // Is i + 2 on an other X-tier
                {
                    if (Math.Abs(leftAreaLength - rightAreaLength) < tol)
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

                    i++;    // skip the last point as there can't be any area above it
                }
            }

            result.ControlPoints.Add(new Point() { X = list[0].Item1.X }); // closing the curve

            return result;
        }

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

        [Description("Modifies a Polyline to have verticies at every xValue, i.e. lineIntersections at every x in xValues. \n" +
                     "Also creates a separate list of every point which also stores its index on the Polyline and k.")]
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
                if (dir)        // add a point at the curve at each x-value within the segments domain
                {
                    dMin = first.X + tol;
                    dMax = last.X - tol;
                    for (int j = 0; j < xValues.Count; j++) //could switch for a while loop?
                    {
                        if (xValues[j] < dMin)
                            continue;
                        if (xValues[j] > dMax)
                            break;

                        Point pt = Query.PointAtX(pLine.ControlPoints[i], pLine.ControlPoints[i + 1], xValues[j]);
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

                        Point pt = Query.PointAtX(pLine.ControlPoints[i], pLine.ControlPoints[i + 1], xValues[j]);
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
                Engine.Base.Compute.RecordWarning(
                    "WetBlanketInterpretation issue, points on top of each other");
            }
            else
            {           // All curves with positive area are oriented counter-clockwise, hence we can know which side of the curve is area
                switch (cornerCase)
                {
                    case 20:    // incoming left,   outgoing rigth
                    case 23:    // incoming left,   outgoing down
                    case 30:    // incoming below,  outgoing rigth
                        leftAreaLength += d;
                        rightAreaLength += d;
                        break;
                    case 21:    // incoming left,   outgoing up
                    case 1:     // incoming rigth,  outgoing up
                    case 31:    // incoming below,  outgoing up
                        leftAreaLength += d;
                        break;
                    case 12:    // incoming top,    outgoing left
                    case 10:    // incoming top,    outgoing rigth
                    case 13:    // incoming top,    outgoing down
                        rightAreaLength += d;
                        break;

                    case 22:    // incoming left,   outgoing left
                        if (before.Y > after.Y) // Going down?
                        {
                            leftAreaLength += d;
                            rightAreaLength += d;
                        }
                        break;
                    case 0:     // incoming rigth,  outgoing rigth
                        if (after.Y > before.Y) // Going up?
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

    }
}




