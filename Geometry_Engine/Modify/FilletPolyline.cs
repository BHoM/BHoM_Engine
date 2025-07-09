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
using System.Net.NetworkInformation;

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

        public static PolyCurve Fillet(this Polyline polyline, double radius)
        {
            if (polyline == null || polyline.ControlPoints == null || polyline.ControlPoints.Count < 3)
            {
                Base.Compute.RecordError("incorrect polyline for filleting");
                return null;
            }

            //Return polyline as polycurve if radius is NaN or zero

            if (double.IsNaN(radius) || radius == 0)
            {
                // Convert the input polyline to a PolyCurve
                var originalCurves = new List<ICurve>();
                for (int i = 1; i < polyline.ControlPoints.Count; i++)
                    originalCurves.Add(BH.Engine.Geometry.Create.Line(polyline.ControlPoints[i - 1], polyline.ControlPoints[i]));
                if (polyline.IsClosed())
                    originalCurves.Add(BH.Engine.Geometry.Create.Line(polyline.ControlPoints.Last(), polyline.ControlPoints.First()));

                return new PolyCurve { Curves = originalCurves };
            }

            //Create for loop to iterate through segments and fillet each corner

            List<ICurve> resultCurves = new List<ICurve>();
            IList<Point> pts = polyline.ControlPoints;
            bool isClosed = polyline.IsClosed();
            int ptCount = pts.Count;
            int segCount = isClosed ? ptCount : ptCount - 1;

            Point lastTrimmedFilletEnd = null;

            for (int i = 0; i < segCount; i++)
            {
                int prevIdx = (i - 1 + ptCount) % ptCount;
                int currIdx = i;
                int nextIdx = (i + 1) % ptCount;

                Point prev = pts[prevIdx];
                Point curr = pts[currIdx];
                Point next = pts[nextIdx];

                if (!isClosed && (i == 0 || i == ptCount - 1))
                {
                    if (i == 0)
                        lastTrimmedFilletEnd = curr;
                    else if (i == ptCount - 1)
                        resultCurves.Add(BH.Engine.Geometry.Create.Line(prev, curr));
                    continue;
                }

                Vector v1 = (prev - curr).Normalise(); // Ensure vectors are normalised to avoid issues with small angles
                Vector v2 = (next - curr).Normalise(); // Ensure vectors are normalised to avoid issues with small angles

                double angle = Math.Acos(Math.Max(-1.0, Math.Min(1.0, v1.DotProduct(v2)))); //Find the angle between the two vectors, ensuring it is within valid range
                if (Math.Abs(angle) < 1e-8 || Math.Abs(Math.PI - angle) < 1e-8)
                {
                    resultCurves.Add(BH.Engine.Geometry.Create.Line(curr, next));
                    lastTrimmedFilletEnd = curr;
                    continue;
                }

                double lenPrev = curr.Distance(prev);
                double lenNext = curr.Distance(next);

                // Determine max radius: never allow tangent points to cross mid point of segment
                double maxRadius = Math.Min(lenPrev/2, lenNext/2) * Math.Tan(angle / 2.0);
                double usedRadius = Math.Min(radius, maxRadius);

                if (usedRadius < 1e-8)
                {
                    resultCurves.Add(BH.Engine.Geometry.Create.Line(curr, next));
                    lastTrimmedFilletEnd = curr;
                    continue;
                }

                double filletDist = usedRadius / Math.Tan(angle / 2.0);

                Point filletStart = curr.Translate(v1 * filletDist); // Translate the current point along the first vector by the fillet distance
                Point filletEnd = curr.Translate(v2 * filletDist);

                // Find the arc centre and create filletArc
                Vector bisector = (v1 + v2).Normalise();
                double bisectorLength = usedRadius / Math.Sin(angle / 2.0);
                Point arcCenter = curr.Translate(bisector * bisectorLength);

                Arc filletArc = BH.Engine.Geometry.Create.ArcByCentre(arcCenter, filletStart, filletEnd);

                // Trim line segments to arc intersection
                Line prevLine = BH.Engine.Geometry.Create.Line(prev, curr);
                var prevInts = BH.Engine.Geometry.Query.CurveIntersections(prevLine, filletArc);
                Point trimmedFilletStart = filletStart;
                if (prevInts != null && prevInts.Count > 0)
                    trimmedFilletStart = prevInts.OrderBy(pt => pt.Distance(curr)).First();

                Line nextLine = BH.Engine.Geometry.Create.Line(curr, next);
                var nextInts = BH.Engine.Geometry.Query.CurveIntersections(nextLine, filletArc);
                Point trimmedFilletEnd = filletEnd;
                if (nextInts != null && nextInts.Count > 0)
                    trimmedFilletEnd = nextInts.OrderBy(pt => pt.Distance(curr)).First();

                if (i > 0 || isClosed)
                {
                    Point segStart = lastTrimmedFilletEnd ?? prev;
                    resultCurves.Add(BH.Engine.Geometry.Create.Line(segStart, trimmedFilletStart));
                }

                resultCurves.Add(filletArc);

                lastTrimmedFilletEnd = trimmedFilletEnd;

                if (!isClosed && i == ptCount - 2)
                    resultCurves.Add(BH.Engine.Geometry.Create.Line(trimmedFilletEnd, next));
            }

            PolyCurve result = new PolyCurve
            {
                Curves = resultCurves
            };

            return result;
        }
    }
}