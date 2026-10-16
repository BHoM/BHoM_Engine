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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Removes consecutive near-duplicate points from an ordered point list while keeping the first and last points.")]
        [Input("points", "Ordered points to cull.")]
        [Input("minSegmentLength", "Minimum kept segment length.")]
        [Output("points", "Culled point list.")]
        public static List<Point> CullSortedDuplicates(this List<Point> points, double minSegmentLength = Tolerance.Distance)
        {
            if (points.Count == 0)
                return points;

            List<Point> output = new List<Point> { points.First() };
            double squaredMinLength = minSegmentLength * minSegmentLength;

            for (int i = 1; i < points.Count - 1; i++)
            {
                Point pnt = points[i];

                if (pnt.SquareDistance(output.Last()) < squaredMinLength
                    || pnt.SquareDistance(points.Last()) < squaredMinLength)
                    continue;

                output.Add(pnt);
            }

            if (output.Last().SquareDistance(points.Last()) > Tolerance.Distance * Tolerance.Distance)
                output.Add(points.Last());

            return output;
        }

        /***************************************************/

        [Description("Removes consecutive near-duplicate points from an ordered point list and reports skipped indexes.")]
        [Input("points", "Ordered points to cull.")]
        [Input("skippedIndexes", "Indexes of removed interior points.")]
        [Input("minSegmentLength", "Minimum kept segment length.")]
        [Output("points", "Culled point list.")]
        public static List<Point> CullSortedDuplicates(this List<Point> points, out List<int> skippedIndexes, double minSegmentLength = Tolerance.Distance)
        {
            skippedIndexes = new List<int>();
            if (points.Count == 0)
                return points;

            List<Point> output = new List<Point> { points.First() };
            double squaredMinLength = minSegmentLength * minSegmentLength;

            for (int i = 1; i < points.Count - 1; i++)
            {
                Point pnt = points[i];

                if (pnt.SquareDistance(output.Last()) < squaredMinLength
                    || pnt.SquareDistance(points.Last()) < squaredMinLength)
                {
                    skippedIndexes.Add(i);
                    continue;
                }

                output.Add(pnt);
            }

            if (output.Last().SquareDistance(points.Last()) > Tolerance.Distance * Tolerance.Distance)
                output.Add(points.Last());

            return output;
        }

        /***************************************************/
    }
}
