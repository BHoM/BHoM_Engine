/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the segments of a line that lie inside a closed planar region.")]
        [Input("region", "Closed planar region.")]
        [Input("line", "Line to clip against the region.")]
        [Input("acceptOnEdges", "If true, segments that lie on the region boundary are kept.")]
        [Input("tolerance", "Distance tolerance.", typeof(Length))]
        [Output("lines", "Segments of the line inside the region.")]
        public static List<Line> IntersectionLines(this PolyCurve region, Line line, bool acceptOnEdges = false, double tolerance = Tolerance.Distance)
        {
            if (region == null || line == null)
                return new List<Line>();

            // Honor line.Infinite. A finite line must not grow out to the infinite chord through the region.
            List<Point> pts = region.LineIntersections(line, false, tolerance) ?? new List<Point>();
            if (!line.Infinite)
            {
                if (line.Start != null && region.IsContaining(new List<Point> { line.Start }, acceptOnEdges, tolerance))
                    pts.Add(line.Start);
                if (line.End != null && region.IsContaining(new List<Point> { line.End }, acceptOnEdges, tolerance))
                    pts.Add(line.End);
            }

            pts = pts.Where(p => p != null).ToList().CullDuplicates(tolerance);
            if (pts.Count < 2)
                return new List<Line>();

            Vector dir = line.Direction();
            if (dir.SquareLength() <= tolerance * tolerance)
                return new List<Line>();

            pts = pts.OrderBy(p => (p - line.Start) * dir).ToList();

            List<Line> result = new List<Line>();
            for (int i = 0; i < pts.Count - 1; i++)
            {
                Point mid = (pts[i] + pts[i + 1]) * 0.5;
                if (!region.IsContaining(new List<Point> { mid }, acceptOnEdges, tolerance))
                    continue;

                Line segment = new Line { Start = pts[i], End = pts[i + 1] };
                if (segment.Length() > tolerance)
                    result.Add(segment);
            }

            return result;
        }

        /***************************************************/

        [Description("Returns the intersection lines of a closed planar region with a cutting plane.")]
        [Input("region", "Closed planar region.")]
        [Input("plane", "Cutting plane.")]
        [Input("tolerance", "Distance tolerance.", typeof(Length))]
        [Output("lines", "Intersection segments of the region with the plane.")]
        public static List<Line> IntersectionLines(this PolyCurve region, Plane plane, double tolerance = Tolerance.Distance)
        {
            if (region == null || plane == null)
                return new List<Line>();

            Plane regionPlane = region.FitPlane(tolerance);
            if (regionPlane == null)
                return new List<Line>();

            Line cut = regionPlane.PlaneIntersection(plane, tolerance);
            if (cut == null)
                return new List<Line>();

            cut.Infinite = true;
            return region.IntersectionLines(cut, false, tolerance);
        }

        /***************************************************/
    }
}
