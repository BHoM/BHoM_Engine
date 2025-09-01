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

        [Description("Fillet a Polyline by inserting circular arcs at internal vertices. Trim length on each side of every filleted vertex is capped at half of each original adjacent segment length so arcs never extend beyond midpoints. For open polylines, endpoints are not filleted.")]
        [Input("polyline", "Polyline to fillet.")]
        [Input("radius", "Target fillet radius (> 0). Actual radius may reduce if trim-length cap is reached.")]
        [Input("distTol", "Distance tolerance.")]
        [Input("angleTol", "Angle tolerance (radians) below which a joint is considered straight and not filleted).")]
        [Output("polyCurve", "Resulting PolyCurve composed of trimmed Lines and Arc fillets.")]

        public static PolyCurve Fillet(this Polyline polyline, double radius,
            double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            //Test for null
            if (polyline == null)
            {
                Base.Compute.RecordError("Polyline is null.");
                return null;
            }

            //Return Polyline as PolyCurve if insufficient control points
            IList<Point> pts = polyline.ControlPoints;
            if (pts == null || pts.Count < 2)
            {
                Base.Compute.RecordError("Polyline has insufficient control points.");
                return new PolyCurve { Curves = new List<ICurve>() };
            }

            //Return Polyline as Polycurve if radius is invalid
            if (double.IsNaN(radius) || radius <= 0)
            {
                Base.Compute.RecordNote("Radius <= 0 or NaN: returning original as PolyCurve.");
                return new PolyCurve { Curves = polyline.SubParts().Cast<ICurve>().ToList() };
            }

            //Test if polyline is closed with respect to the distance tolerance provided
            bool closed = polyline.IsClosed(distTol);

            // Collect distinct vertices (remove consecutive duplicates)
            List<Point> vertices = new List<Point>();
            for (int i = 0; i < pts.Count; i++)
            {
                if (i == 0 || !pts[i].Distance(pts[i - 1]).IsZero(distTol))
                    vertices.Add(pts[i]);
            }
            if (closed && vertices.Count > 1 && vertices[0].Distance(vertices.Last()).IsZero(distTol))
                vertices.RemoveAt(vertices.Count - 1);

            int nVerts = vertices.Count;
            if (nVerts < 3)
            {
                Base.Compute.RecordNote("Not enough vertices to fillet. Returning original.");
                return new PolyCurve { Curves = polyline.SubParts().Cast<ICurve>().ToList() };
            }

            int segCount = closed ? nVerts : nVerts - 1;

            // Build original segments and lengths
            List<Line> segments = new List<Line>(segCount);
            double[] origLengths = new double[segCount];
            for (int s = 0; s < segCount; s++)
            {
                Point a = vertices[s];
                Point b = vertices[(s + 1) % nVerts];
                Line ln = new Line { Start = a, End = b };
                segments.Add(ln);
                origLengths[s] = ln.Length();
            }

            // Per-vertex data (vertex i involves segments (i-1) and i)
            int jointCount = closed ? nVerts : nVerts;
            double[] angle = new double[jointCount];
            double[] tanHalf = new double[jointCount];
            double[] trim = new double[jointCount];      // chosen trim distance
            double[] radiusUsed = new double[jointCount];
            bool[] fillet = new bool[jointCount];

            // First pass: per-vertex local cap on trim length
            for (int i = 0; i < jointCount; i++)
            {
                bool isEndpoint = (!closed) && (i == 0 || i == jointCount - 1);
                if (isEndpoint)
                {
                    fillet[i] = false;
                    continue;
                }

                int prevSeg = (i - 1 + segCount) % segCount;
                int nextSeg = i % segCount;

                Point pPrev = vertices[(i - 1 + nVerts) % nVerts];
                Point pCurr = vertices[i];
                Point pNext = vertices[(i + 1) % nVerts];

                Vector v1 = (pPrev - pCurr);
                Vector v2 = (pNext - pCurr);
                double len1 = v1.Length();
                double len2 = v2.Length();
                if (len1 < distTol || len2 < distTol)
                {
                    fillet[i] = false;
                    continue;
                }

                v1 /= len1;
                v2 /= len2;

                double d = Math.Max(-1.0, Math.Min(1.0, v1.DotProduct(v2)));
                double theta = Math.Acos(d);
                if (theta < angleTol || Math.Abs(Math.PI - theta) < angleTol)
                {
                    fillet[i] = false;
                    continue;
                }

                angle[i] = theta;
                double thHalf = Math.Tan(theta / 2.0);
                tanHalf[i] = thHalf;
                if (Math.Abs(thHalf) < 1e-12)
                {
                    fillet[i] = false;
                    continue;
                }

                // Desired trim from requested radius
                double tDesired = radius / thHalf;

                // Cap trim to half of each ORIGINAL adjacent segment
                double tCapHalf = Math.Min(0.5 * origLengths[prevSeg], 0.5 * origLengths[nextSeg]);

                double t = Math.Min(tDesired, tCapHalf);

                if (t <= distTol)
                {
                    fillet[i] = false;
                    continue;
                }

                trim[i] = t;
                radiusUsed[i] = t * thHalf; // actual radius after trim cap
                fillet[i] = true;
            }

            // Global pass: ensure for each segment s between vertex s and vertex s+1 (open indexing)
            // trim[s] + trim[s+1] <= origLengths[s] - margin
            double margin = distTol * 2.0;
            int maxPass = 8;
            for (int pass = 0; pass < maxPass; pass++)
            {
                bool changed = false;
                for (int s = 0; s < segCount; s++)
                {
                    int vStart = s;
                    int vEnd = (s + 1) % nVerts;

                    double t1 = (vStart < jointCount && fillet[vStart]) ? trim[vStart] : 0.0;
                    double t2 = (vEnd < jointCount && fillet[vEnd]) ? trim[vEnd] : 0.0;

                    double maxAllowed = origLengths[s] - margin;
                    if (maxAllowed < distTol)
                        maxAllowed = origLengths[s] * 0.99;

                    if (t1 + t2 > maxAllowed && (t1 > distTol || t2 > distTol))
                    {
                        double sum = t1 + t2;
                        double scale = maxAllowed / sum;

                        if (fillet[vStart])
                        {
                            trim[vStart] *= scale;
                            if (trim[vStart] <= distTol)
                            {
                                fillet[vStart] = false;
                                trim[vStart] = 0;
                                radiusUsed[vStart] = 0;
                            }
                            else
                                radiusUsed[vStart] = trim[vStart] * tanHalf[vStart];
                        }

                        if (fillet[vEnd])
                        {
                            trim[vEnd] *= scale;
                            if (trim[vEnd] <= distTol)
                            {
                                fillet[vEnd] = false;
                                trim[vEnd] = 0;
                                radiusUsed[vEnd] = 0;
                            }
                            else
                                radiusUsed[vEnd] = trim[vEnd] * tanHalf[vEnd];
                        }

                        changed = true;
                    }
                }
                if (!changed) break;
            }

            // Build output: for each segment create trimmed line, then add arc at its end vertex (avoiding duplicates).
            List<ICurve> output = new List<ICurve>();

            for (int s = 0; s < segCount; s++)
            {
                int vStart = s;
                int vEnd = (s + 1) % nVerts;

                Point pA = vertices[s];
                Point pB = vertices[(s + 1) % nVerts];

                Vector dir = (pB - pA);
                double L = dir.Length();
                if (L < distTol) continue;
                dir /= L;

                double tStart = (vStart < jointCount && fillet[vStart]) ? trim[vStart] : 0.0;
                double tEnd = (vEnd < jointCount && fillet[vEnd]) ? trim[vEnd] : 0.0;

                // Safety: if tStart + tEnd >= L reduce both proportionally or drop fillets.
                if (tStart + tEnd >= L - distTol)
                {
                    if (fillet[vStart] || fillet[vEnd])
                    {
                        if (tStart >= tEnd)
                        {
                            fillet[vEnd] = false; tEnd = 0; trim[vEnd] = 0; radiusUsed[vEnd] = 0;
                        }
                        else
                        {
                            fillet[vStart] = false; tStart = 0; trim[vStart] = 0; radiusUsed[vStart] = 0;
                        }
                        if (tStart + tEnd >= L - distTol)
                        {
                            if (fillet[vStart]) { fillet[vStart] = false; tStart = 0; trim[vStart] = 0; radiusUsed[vStart] = 0; }
                            if (fillet[vEnd]) { fillet[vEnd] = false; tEnd = 0; trim[vEnd] = 0; radiusUsed[vEnd] = 0; }
                        }
                    }
                }

                Point newStart = pA + dir * tStart;
                Point newEnd = pB - dir * tEnd;
                output.Add(new Line { Start = newStart, End = newEnd });

                // Add arc at end vertex if fillet and not an endpoint of open polyline
                bool isEndEndpoint = (!closed) && (vEnd == nVerts - 1);
                if (fillet[vEnd] && !isEndEndpoint)
                {
                    Point joint = vertices[vEnd];
                    Point prevPt = vertices[(vEnd - 1 + nVerts) % nVerts];
                    Point nextPt = vertices[(vEnd + 1) % nVerts];

                    Vector v1 = (prevPt - joint).Normalise();
                    Vector v2 = (nextPt - joint).Normalise();

                    double t = trim[vEnd];
                    if (t <= distTol) continue;

                    Point filletStart = joint + v1 * t;
                    Point filletEnd = joint + v2 * t;

                    Vector bis = (v1 + v2);
                    double bisLen = bis.Length();
                    if (bisLen < distTol) continue;
                    bis /= bisLen;

                    double R = radiusUsed[vEnd];
                    if (R <= distTol) continue;

                    double centreDist = R / Math.Sin(angle[vEnd] / 2.0);
                    Point centre = joint + bis * centreDist;

                    Arc arc = Create.ArcByCentre(centre, filletStart, filletEnd);
                    if (arc != null)
                        output.Add(arc);
                }
            }

            if (closed)
            {
                Point startPoint = output.First().IStartPoint();
                Point endPoint = output.Last().IEndPoint();
                if (!startPoint.Distance(endPoint).IsZero(distTol))
                    output.Add(Create.Line(endPoint, startPoint));
            }

            return new PolyCurve { Curves = output };
        }

        private static bool IsZero(this double value, double tol)
        {
            return Math.Abs(value) <= tol;
        }
    }
}