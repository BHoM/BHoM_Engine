/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
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
        /**** Public Methods - Curves                  ****/
        /***************************************************/

        [Description("Returns a PolyCurve representing the input Polyline with fillets of the given radius at each internal vertex.")]
        [Input("polyline", "The Polyline to fillet.")]
        [Input("radius", "The radius of the fillet arc at each corner.")]
        [Output("polyCurve", "A PolyCurve with fillets at each corner, or null if unsuccessful.")]

        public static PolyCurve FilletWithTrim(this Polyline polyline, double radius)
        {
            if (polyline == null || polyline.ControlPoints == null || polyline.ControlPoints.Count < 3)
                return null;

            List<ICurve> curves = new List<ICurve>();
            var pts = polyline.ControlPoints;
            bool isClosed = polyline.IsClosed();
            int n = pts.Count;

            int start = isClosed ? 0 : 1;
            int end = isClosed ? n : n - 1;

            // For each corner
            for (int i = start; i < end - 1; i++)
            {
                int prevIdx = (i - 1 + n) % n;
                int currIdx = i;
                int nextIdx = (i + 1) % n;

                Point P0 = pts[prevIdx];
                Point P1 = pts[currIdx];
                Point P2 = pts[nextIdx];

                // Directions
                Vector v1 = (P1 - P0).Normalise();
                Vector v2 = (P2 - P1).Normalise();

                // Arc start/end and center (simple construction)
                Point arcStart = P1 + v1 * radius;
                Point arcEnd = P1 + v2 * radius;
                Point arcCentre = P1 + v1 * radius + v2 * radius;

                // Build arc (assuming method ArcByCentre exists: start, centre, end)
                Arc filletArc = BH.Engine.Geometry.Create.ArcByCentre(arcStart, arcCentre, arcEnd);

                // --- Trim Previous Segment ---
                Line prevLine = BH.Engine.Geometry.Create.Line(P0, P1);
                var intPrev = BH.Engine.Geometry.Query.CurveIntersections(prevLine, filletArc);
                Point trimPrev = null;
                if (intPrev != null && intPrev.Count > 0)
                {
                    // Use the intersection point closest to P1
                    trimPrev = intPrev.OrderBy(pt => pt.Distance(P1)).First();
                }

                // --- Trim Next Segment ---
                Line nextLine = BH.Engine.Geometry.Create.Line(P1, P2);
                var intNext = BH.Engine.Geometry.Query.CurveIntersections(nextLine, filletArc);
                Point trimNext = null;
                if (intNext != null && intNext.Count > 0)
                {
                    trimNext = intNext.OrderBy(pt => pt.Distance(P1)).First();
                }

                // Add trimmed previous segment (if not at start of open polyline)
                if (i > 1 || isClosed)
                {
                    if (trimPrev != null)
                        curves.Add(BH.Engine.Geometry.Create.Line(P0, trimPrev));
                }
                else if (!isClosed && i == 1)
                {
                    // Add first segment of open polyline, untrimmed
                    curves.Add(BH.Engine.Geometry.Create.Line(P0, arcStart));
                }

                // Add fillet arc
                curves.Add(filletArc);

                // For last corner in open polyline, add trailing segment
                if (!isClosed && i == end - 2)
                {
                    if (trimNext != null)
                        curves.Add(BH.Engine.Geometry.Create.Line(trimNext, P2));
                    else
                        curves.Add(BH.Engine.Geometry.Create.Line(arcEnd, P2));
                }
            }

            return new PolyCurve { Curves = curves };
        }
    }
}