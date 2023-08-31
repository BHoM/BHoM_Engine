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
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Implements the GrahamScan algorithm to determine the convex hull of a list of points contained within a single Plane.")]
        [Input("points", "The points to determine the convex hull contained within a single Plane.")]
        [Input("tolerance", "Geometrical tolerance to be used in the method.", typeof(Length))]
        [Output("c", "The convex hull of the point list, no repeat points are returned.")]
        public static List<Point> GrahamScan(List<Point> points, double tolerance = Tolerance.MacroDistance)
        {
            if(points.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The point list is either null or empty.");
                return points;
            }

            
            List<Point> pts = points.ShallowClone();

            pts.CullDuplicates(tolerance);

            if (pts.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The point list is null or empty.");
                return pts;
            }

            if (pts.Count < 3)
            {
                Base.Compute.RecordError("The point list (excluding duplicates) must be greater than three to determine the Convex Hull.");
                return pts;
            }

            // Check that the points are all wihtin the same plane 
            Plane plane = pts.FitPlane(tolerance);
            if (!Query.IsInPlane(pts, plane, tolerance))
            {
                pts.Select(x => x.Project(plane));
                Base.Compute.RecordWarning("The points needs to be within a single Plane. They have been projected to the plane that fits the points.");
            }

            Cartesian origin = new Cartesian();
            Cartesian cartesian = null;

            // Check if the points are located in the XY plane, otherwise map them to XY 
            if(plane != Plane.XY)
            {
                Vector locY = pts.FitLine(tolerance).Direction();
                cartesian = Create.CartesianCoordinateSystem(pts.Average(), locY.CrossProduct(plane.Normal), locY);
                pts = pts.Select(x => x.Orient(cartesian, origin)).ToList();
            }

            // Find the point with the lowest Y coordinate
            IOrderedEnumerable<Point> orderedPts = pts.OrderBy(pt => pt.Y);

            pts = orderedPts.ToList();
            Point p = pts[0];

            // Check if there is more than one point with the lowest Y
            if (Math.Abs(p.Y - pts[1].Y) < tolerance)
            {
                // Get points with lowest Y coordinate and select the point with the lowest X
                pts = orderedPts.ThenBy(x => x.X).ToList();
                p = pts.First();
            }

            // Remove the P from the list of points
            pts.Remove(p);

            // Calculate the angles between p and each pt
            List<double> angles = pts.Select(pt => pt - p).Select(v => v.DotProduct(Vector.XAxis) / v.Length()).ToList();

            // Combine both lists, sort by angle and then extract the points
            pts = pts.Zip(angles, (a, b) => new { pt = a, angle = b }).OrderBy(c => c.angle).Select(x => x.pt).Reverse().ToList();

            // Group by angle between P and the points
            IEnumerable<IGrouping<double, Point>> groupedPts = pts.GroupBy(pt => Create.Vector(p, pt).DotProduct(Vector.XAxis) / Create.Vector(p, pt).Length());

            // Check for points that have the same angle 
            if (groupedPts.Where(grp => grp.Count() > 1).Any())
            {
                // For each group, sort by distance from P and select the furthest point
                pts = groupedPts.Select(g => g.OrderByDescending(i => i.SquareDistance(p)).First()).ToList();
            }

            // Add to the start of selPts as it has been removed from pts
            List<Point> selPts = new List<Point>() { p };

            // Iterate through the algorithim
            while (pts.Count > 0)
            {
                GrahamScanSolver(ref pts, ref selPts);
            };

            // Remap the points to their original orientation
            if (plane != Plane.XY)
            {
                selPts = selPts.Select(x => x.Orient(origin, cartesian)).ToList();
            }

            return selPts;
        }

        [Description("Take the first point from pts (p), and the last two points from selPts to determine if the three points form an anticlockwise turn. If the turn is anticlockwise, p is added to selPts, otherwise the " +
            "last selPts is removed. This method is used iteratively to determine the convex hull of a point list.")]
        private static void GrahamScanSolver(ref List<Point> pts, ref List<Point> selPts)
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
        }
    }
}




