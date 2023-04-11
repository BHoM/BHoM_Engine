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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;
using BH.oM.Geometry.CoordinateSystem;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Find segments of a line lying inside the region defined by a polyline.")]
        [Input("line", "A line to check for intersections with a region defined by the input polylines.")]
        [Input("pLine", "A polyline defining a closed region that the input line potentially intersects.")]
        [Input("tolerance", "Minimum length required of new intersection lines.")]
        [Output("intersections", "Segments of the input line that lie inside the region defined by the input polyline.")]
        public static List<Line> RegionIntersection(this Polyline region, Line line, double minLineLength = Tolerance.Distance, bool acceptOnEdge = true)
        {
            List<Line> intersections = new List<Line>();

            if (!line.IsInRange(region.Bounds()))
                return intersections;

            if (region.IsContaining(line))
                return new List<Line> { line };

            List<Point> intersectionPnts = region.LineIntersections(line);
            if (!intersectionPnts.Any())
                return intersections;

            List<double> pointParameters = intersectionPnts.Select(x => line.ParameterAtPoint(x)).ToList();

            List<Tuple<Point, double>> sortedPointParamPairs = intersectionPnts
                .Zip(pointParameters, (point, param) => Tuple.Create( point, param ))
                .OrderBy(x => x.Item2)
                .Select(x => x).ToList();

            Point startPnt = line.PointAtParameter(0);
            if (region.IsContaining(new List<Point> { startPnt }))
            {
                Point intPnt = sortedPointParamPairs.First().Item1;
                if (intPnt.Distance(startPnt) > minLineLength)
                {
                    intersections.Add(new Line() { Start = startPnt, End = intPnt });
                }
            }

            Point endPnt = line.PointAtParameter(1);
            if (region.IsContaining(new List<Point> { endPnt }))
            {
                Point intPnt = sortedPointParamPairs.Last().Item1;
                if (intPnt.Distance(endPnt) > minLineLength)
                {
                    intersections.Add(new Line() { Start = intPnt, End = endPnt });
                }
            }

            for (int i = 0; i < (sortedPointParamPairs.Count - 1); i++)
            {
                Tuple<Point, double> entry1 = sortedPointParamPairs[i];
                Tuple<Point, double> entry2 = sortedPointParamPairs[i + 1];
                Point point1 = entry1.Item1;
                Point point2 = entry2.Item1;

                if (point1.Distance(point2) < minLineLength)
                    continue;

                double averageParam = (entry1.Item2 + entry2.Item2) / 2;
                List<Point> midPoints = new List<Point> { line.PointAtParameter(averageParam) };

                if (region.IsContaining(midPoints, acceptOnEdge))
                {
                    intersections.Add(new Line() { Start = point1, End = point2 });
                }
            }

            return intersections;
        }

        /***************************************************/
    }
}



