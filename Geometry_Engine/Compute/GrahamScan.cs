/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Implements the GrahamScan algorithm to determine the convex hull of a list of points contained within the XY Plane.")]
        [Input("pts", "The points to determine the convex hull contained within the XY Plane.")]
        [Output("c", "The convex hull of the point list.")]
        public static List<Point> GrahamScan(List<Point> pts, double tolerance = Tolerance.MicroDistance)
        {
            if (pts.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The point list is null or empty.");
                return pts;
            }
            if (pts.Count < 3)
            {
                Base.Compute.RecordError("The point list must be greater than three to determine the Convex Hull.");
                return pts;
            }

            // Check that the points are all wihtin the same plane 
            Plane plane = pts.FitPlane(tolerance);
            if (!Query.IsInPlane(pts, Plane.XY))
            {
                Base.Compute.RecordError("The point list needs to be within the XY Plane.");
                return pts;
            }

            // Find the point with the lowest Y coordinate
            IOrderedEnumerable<Point> orderedPts = pts.OrderBy(pt => pt.Y);

            Point p = orderedPts.First();

            // Check if there is more than one point with the lowest Y
            if (Math.Abs(p.Y - ((Point)orderedPts.IItem(1)).Y) < tolerance)
            {
                // Get points with lowest Y coordinate and select the point with the lowest X
                pts = orderedPts.ThenBy(x => x.X).ToList();
                p = orderedPts.First();
            }
            else
                pts = orderedPts.ToList();

            // Remove the P from the list of points
            pts.Remove(p);

            // Sort by the increasing order of the angle the point and P make with the x-axis
            pts = pts.OrderBy(pt => Create.Vector(p, pt).DotProduct(Vector.XAxis) / Create.Vector(p, pt).Length()).ToList();

            // Group by angle between P and the points
            IEnumerable<IGrouping<double, Point>> groupedPts = pts.GroupBy(pt => Create.Vector(p,pt).DotProduct(Vector.XAxis)/Create.Vector(p, pt).Length());

            // Add to the start of selPts as it has been removed from pts
            List<Point> selPts = new List<Point>() { p };

            bool duplicateAngle = false;

            // Check for points that have the same angle 
            if (groupedPts.Where(grp => grp.Count() > 1).Any())
            {
                // For each group, sort by distance from P and select the furthest point
                pts = groupedPts.Select(g => new { s = g.OrderByDescending(i => i.Distance(p)) }).Select(x => x.s.First()).ToList();
                duplicateAngle = true;
            }

            // Iterate through the algorithim
            while (pts.Count > 0)
            {
                GrahamScanSolver(ref pts, ref selPts);
            };

            // Due to the grouping, it is necessary to add the first point to close the polyline
            if (duplicateAngle)
                selPts.Add(selPts.First());

            return selPts;
        }

        private static List<Point> GrahamScanSolver(ref List<Point> pts, ref List<Point> selPts)
        {
            if (pts.Count > 0)
            {
                // Update the reference as the method moves along the curve
                var p = pts[0];

                // Add the next point along, needs a minimum of two points to begin (the third coming from p)
                if (selPts.Count <= 1)
                {
                    selPts.Add(p);
                    pts.RemoveAt(0);
                }
                else
                {
                    Point pt1 = selPts[selPts.Count - 1];
                    Point pt2 = selPts[selPts.Count - 2];
                    Vector dir1 = pt1 - pt2;
                    Vector dir2 = p - pt1;

                    if (Query.CrossProduct(dir1, dir2).Z < 0)
                    {
                        // Less than zero, therefore clockwise and the point is within the boundary
                        selPts.RemoveAt(selPts.Count - 1);
                    }
                    else
                    {
                        // Greater than zero, therefore anticlockwise - so point is not within the boundary
                        selPts.Add(p);
                        pts.RemoveAt(0);
                    }
                }
            }
            return selPts;
        }
    }
}




