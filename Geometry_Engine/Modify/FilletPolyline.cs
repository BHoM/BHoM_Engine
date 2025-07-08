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

        public static PolyCurve Fillet(this Polyline polyline, double radius)
        {
            if (polyline == null || polyline.ControlPoints == null || polyline.ControlPoints.Count < 3)
            {
                Base.Compute.RecordError("incorrect polyline for filleting");
                return null;
            }

            List<ICurve> resultCurves = new List<ICurve>();
            IList<Point> pts = polyline.ControlPoints;
            bool isClosed = polyline.IsClosed();
            int ptCount = pts.Count;

            int segCount = isClosed ? ptCount : ptCount - 1;

            for (int i = 0; i < segCount; i++)
            {
                int prevIdx = (i - 1 + ptCount) % ptCount;
                int currIdx = i;
                int nextIdx = (i + 1) % ptCount;

                Point prev = pts[prevIdx];
                Point curr = pts[currIdx];
                Point next = pts[nextIdx];

                // For open polylines, add first and last segments unfilleted
                if (!isClosed && (i == 0 || i == ptCount - 1))
                {
                    if (i == 0)
                        resultCurves.Add(BH.Engine.Geometry.Create.Line(curr, next));
                    else if (i == ptCount - 1)
                        resultCurves.Add(BH.Engine.Geometry.Create.Line(prev, curr));
                    continue;
                }

                // Vectors from current to prev/next
                Vector v1 = (prev - curr).Normalise();
                Vector v2 = (next - curr).Normalise();

                double angle = Math.Acos(Math.Max(-1.0, Math.Min(1.0, v1.DotProduct(v2))));
                if (Math.Abs(angle) < 1e-8 || Math.Abs(Math.PI - angle) < 1e-8)
                {
                    // Colinear, just add straight segment
                    resultCurves.Add(BH.Engine.Geometry.Create.Line(curr, next));
                    continue;
                }

                // Find tangent points for fillet
                double filletDist = radius / Math.Tan(angle / 2.0);
                double lenPrev = curr.Distance(prev);
                double lenNext = curr.Distance(next);

                if (filletDist > 0.5 * lenPrev || filletDist > 0.5 * lenNext)
                {
                    // Not enough space for fillet, just use straight segment
                    resultCurves.Add(BH.Engine.Geometry.Create.Line(curr, next));
                    continue;
                }

                Point filletStart = curr.Translate(v1 * filletDist);
                Point filletEnd = curr.Translate(v2 * filletDist);

                // Add segment before fillet (except for closed loop first corner)
                if (i > 0 || isClosed)
                    resultCurves.Add(BH.Engine.Geometry.Create.Line(prev, filletStart));

                // Fillet arc
                Vector bisector = (v1 + v2).Normalise();
                double bisectorLength = radius / Math.Sin(angle / 2.0);
                Point arcCenter = curr.Translate(bisector * bisectorLength);

                Arc filletArc = BH.Engine.Geometry.Create.ArcByCentre(arcCenter, filletStart, filletEnd);

                resultCurves.Add(filletArc);

                // On last segment (open polyline), add segment after fillet
                if (!isClosed && i == ptCount - 2)
                    resultCurves.Add(BH.Engine.Geometry.Create.Line(filletEnd, next));
            }

            PolyCurve result = new PolyCurve
            {
                Curves = resultCurves
            };

            return result;
        }
    }
}