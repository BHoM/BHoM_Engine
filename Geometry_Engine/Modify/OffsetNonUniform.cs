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

using BH.Engine.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
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

        [Description("Offset each segment of a polyline by a value from the input list of distances.")]
        [Input("pLine", "The polyline to offset non-uniformly.")]
        [Input("distances", "Distances to use when offsetting individual segments of the polyline.")]
        [Input("normal", "The normal vector of a plane to use during the offset operation.")]
        [Input("tolerance", "The tolerance to use during the offset operation.")]
        [Output("polyline", "The input polyline once each of its segments has been offset by the respective distance.")]
        public static Polyline OffsetNonUniform(this Polyline pLine, List<double> distances, Vector normal = null, double tolerance = Tolerance.Distance)
        {
            if (pLine == null || pLine.Length() < tolerance)
                return null;

            if (distances.Any(x => x < 0))
            {
                Base.Compute.RecordError("This method currently supports non-negative offset distances only.");
                return null;
            }

            if (!pLine.IsPlanar(tolerance))
            {
                Base.Compute.RecordError("Offset works only on planar curves");
                return null;
            }

            if (pLine.SubParts().Count != distances.Count)
            {
                Base.Compute.RecordError("Count of offset distances must match the number of segments in the polyline");
                return null;
            }

            if (!pLine.IsClosed(tolerance))
            {
                Base.Compute.RecordError("Polyline must be closed");
                return null;
            }

            if (distances.All(x => x == 0))
                return pLine;

            List<Line> segments = pLine.SubParts();
            List<Line> newLines = new List<Line>();
            normal = normal?.Normalise() ?? pLine.Normal();
            Point averagePnt = pLine.Bounds().Centre();

            for (int i = 0; i < segments.Count; i++)
            {
                Line line = segments[i];
                double distance = distances[i];
                Line offsetLine = line.Offset(distances[i], normal);

                Vector directionToCenter = averagePnt - line.Start;
                Vector newDirectionToCenter = averagePnt - offsetLine.Start;
                bool directionMaintained = directionToCenter.Angle(newDirectionToCenter) < Math.PI / 2;

                if (!directionMaintained)
                    offsetLine = line.Offset(distances[i], -normal);
                else
                {
                    double distanceToCenter = line.Start.Distance(averagePnt);
                    double newDistanceToCenter = offsetLine.Start.Distance(averagePnt);
                    bool newLineIsOutside = newDistanceToCenter > distanceToCenter;

                    if ((distance > 0 && !newLineIsOutside) || (distance < 0 && newLineIsOutside))
                        offsetLine = line.Offset(distances[i], -normal);
                }

                offsetLine.Infinite = true;
                newLines.Add(offsetLine);
            }

            List<Line> nextLines = newLines.ShiftList(1);
            List<Point> intersections = new List<Point>();

            for (int i = 0; i < newLines.Count; i++)
                intersections.Add(newLines[i].LineIntersection(nextLines[i], true));

            return new Polyline { ControlPoints = intersections }.Close();
        }

        /***************************************************/
    }
}
