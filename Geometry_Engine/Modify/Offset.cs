/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base.Debugging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Quantities.Attributes;
using System.Diagnostics;
using Microsoft.SqlServer.Server;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [PreviousVersion("7.2", "BH.Engine.Geometry.Modify.Offset(BH.oM.Geometry.Line, System.Double, BH.oM.Geometry.Vector, System.Boolean, System.Double)")]
        [Description("Creates an offset of a curve")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc")]
        [Output("curve", "Resulting offset")]
        public static Line Offset(this Line curve, double offset, Vector normal, OffsetOptions options = null, double tolerance = Tolerance.Distance)
        {
            return curve.Translate(normal.CrossProduct(curve.Start - curve.End).Normalise() * offset);
        }

        /***************************************************/

        [PreviousVersion("7.2", "BH.Engine.Geometry.Modify.Offset(BH.oM.Geometry.Arc, System.Double, BH.oM.Geometry.Vector, System.Boolean, System.Double)")]
        [Description("Creates an offset of a curve")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc")]
        [Output("curve", "Resulting offset")]
        public static Arc Offset(this Arc curve, double offset, Vector normal, OffsetOptions options = null, double tolerance = Tolerance.Distance)
        {
            if (offset == 0)
                return curve;

            double radius = curve.Radius;

            if (normal == null)
                normal = curve.Normal();

            if (normal.DotProduct(curve.Normal()) > 0)
                radius += offset;
            else
                radius -= offset;

            Arc result = curve.DeepClone();

            if (radius > 0)
            {
                result.Radius = radius;
                return result;
            }
            else
            {
                Base.Compute.RecordError("Offset value is greater than arc radius");
                return null;
            }
        }

        /***************************************************/

        [PreviousVersion("7.2", "BH.Engine.Geometry.Modify.Offset(BH.oM.Geometry.Circle, System.Double, BH.oM.Geometry.Vector, System.Boolean, System.Double)")]
        [Description("Creates an offset of a curve")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc")]
        [Output("curve", "Resulting offset")]
        public static Circle Offset(this Circle curve, double offset, Vector normal = null, OffsetOptions options = null, double tolerance = Tolerance.Distance)
        {
            if (offset == 0)
                return curve;

            double radius = curve.Radius;

            if (normal == null)
                normal = curve.Normal;

            if (normal.DotProduct(curve.Normal) > 0)
                radius += offset;
            else
                radius -= offset;

            Circle result = curve.DeepClone();

            if (radius > 0)
            {
                result.Radius = radius;
                return result;
            }
            else
            {
                Base.Compute.RecordError("Offset value is greater than circle radius");
                return null;
            }
        }


        /***************************************************/

        [PreviousVersion("7.2", "BH.Engine.Geometry.Modify.Offset(BH.oM.Geometry.Polyline, System.Double, BH.oM.Geometry.Vector, System.Boolean, System.Double)")]
        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("options", "Options for the offset operation. Default values is used if nothing is provided.")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves")]
        [Output("curve", "Resulting offset")]
        public static Polyline Offset(this Polyline curve, double offset, Vector normal = null, OffsetOptions options = null, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            if (curve == null || curve.Length() < distTol)
                return null;

            if (offset == 0)
                return curve;
            if (!curve.IsPlanar(distTol))
            {
                BH.Engine.Base.Compute.RecordError("Offset works only on planar curves");
                return null;
            }

            options = options ?? new OffsetOptions();   //Initialise to default values if nothing is provided
            bool isClosed = curve.IsClosed(distTol);

            List<Point> cPts = new List<Point>(curve.ControlPoints);

            if (normal == null)
                if (!isClosed)
                {
                    int i = 2;
                    Vector v1 = cPts[1] - cPts[0];
                    do
                    {
                        normal = v1.CrossProduct(cPts[i] - cPts[0]);
                        i++;
                        if (i >= cPts.Count)
                        {
                            Base.Compute.RecordError("Normal vector is required to be provided for a linear curve.");
                            return null;
                        }
                    } while (normal.Length() < distTol);
                }
                else
                    normal = curve.Normal();
            else
                normal = normal.Normalise();

            if (offset < 0) //Always work with a positive offset value, makes it possible to make correct assumptions for various edge cases
            {
                offset = -offset;
                normal = -normal;
            }
            
            //Construct list of segment data for each line peice. Segment data contains
            // - Length of the segments
            // - Tangent vector of the segment
            // - Orthogonal vector of the segment
            // - Length change of the segment for a offset with a magnitude equal to 1
            List<OffsetSegment> segments = new List<OffsetSegment>();
            for (int i = 0; i < cPts.Count - 1; i++)
            {
                Vector tan = (cPts[i + 1] - cPts[i]);
                double length = tan.Length();
                if (options.RemoveShortSegments && length < distTol) //Check for short segments and remove
                {
                    cPts.RemoveAt(i + 1);
                    i--;
                }
                else
                {
                    if (options.RemoveShortSegments || length > 0)
                        tan = tan * (1 / length);

                    //Set length, tangent and orthogonal vector. Length change for the segment is computed in a later step
                    segments.Add(new OffsetSegment() { Length = length, Tangent = tan, Orthogonal = tan.CrossProduct(normal).Normalise(), ComputeLengthChange = options.HandleCreatedLocalSelfIntersections });
                }
            }

            if (isClosed)
                cPts.RemoveAt(cPts.Count - 1);
            
            //Construct list of vertex data for each control point where the vertex data contains information about
            // - Position of the vertex
            // - Offset direction for the vertex as a vector with a vector scaled to an offset of magnitude equal to 1
            // - Length change for adjecent segments for an offset with a magnitude equal to 1
            // - Maxmimum allowed offset until the vertex hits a segment
            // - Segment hit after maximum offset
            List<OffsetVertex> vertices = cPts.Select(x => new OffsetVertex() { Position = x, ComputeTranlation = true, ComputeIntersection = options.HandleGeneralCreatedSelfIntersections }).ToList();

            //Tolerance used for checking if two adjecent segments are anti-parallel and hence creating a acute corner
            double sinTol = Math.Pow(Math.Sin(angleTol / 2), 2);

            ////Construct list of segments and vertices which require (re)computation in terms of translation vectors and length change
            ////For firt iteration is is all of the,
            //List<int> cornerComputes = new List<int>(Enumerable.Range(0, vertices.Count));
            //List<int> segmentsComputes = new List<int>(Enumerable.Range(0, segments.Count));

            //List<int> cornerIntersectionComputes;
            //if (options.HandleGeneralCreatedSelfIntersections)
            //    cornerIntersectionComputes = new List<int>(Enumerable.Range(0, vertices.Count));
            //else
            //    cornerIntersectionComputes = new List<int>();

            bool firstIteration = true;

            //Loop and continue offseting until all offsets done.
            //Done to handle local apparent self intersections due to segments getting a length < 0 (being flipped) for concave offsets
            int bailout = 10000;
            int counter = 0;
            while (offset > 0 && counter < bailout)
            {
                counter++;  
                //Compute corner offset vectors. First whileloop iteration computes for all corners
                //Subsequent iteration only updates corners that have seen a change
                bool recheckTranlsations = false;
                do
                {
                    recheckTranlsations = false;
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        OffsetVertex v = vertices[i];
                        if (!v.ComputeTranlation)
                            continue;

                        v.ComputeTranlation = false;
                        if (!isClosed && i == 0)    //Start point vector for open curves will simply be the orthogonal vector of the first segment
                            v.Translation = segments[i].Orthogonal;
                        else if (!isClosed && i == vertices.Count - 1)  //Similar to start
                            v.Translation = segments[i - 1].Orthogonal;
                        else
                        {
                            int prev = i == 0 ? segments.Count - 1 : i - 1;

                            var seg1 = segments[prev];
                            var seg2 = segments[i];

                            Vector dir = (seg1.Orthogonal + seg2.Orthogonal) / 2; //Offset direction computed as sum of the previous and next orthogonal vectors

                            //Below factor gives the correct magnitude for corner scaling based on equal angle triangles.
                            //ortho projected onto the direction gives a right angled triangle with the same angles as the triangle
                            //formed by offset vetor with correct length and dir.
                            double length = dir.SquareLength();

                            if (options.HandleAdjacentParallelSegments && length < sinTol)    //Equivalent to check that angle between two adjecent segements is less than angle tolerance
                            {
                                if (!HandleParalellAdjecentPolylinearSegments(segments, vertices, normal, isClosed, firstIteration, distTol, i, prev, angleTol))
                                {
                                    Base.Compute.RecordWarning("Polyline reduced to nothing. Empty polyline returned.");
                                    return new Polyline();
                                }
                                recheckTranlsations = true; //Set flag to re-iteratate to check translation vectors again
                            }
                            else
                            {
                                Vector trans = dir * (1 / length);
                                double vertLengthChange = seg1.Tangent.DotProduct(trans);
                                v.Translation = trans;
                                v.AdjacentSegmentLengthChange = vertLengthChange;
                                seg1.ComputeLengthChange = options.HandleCreatedLocalSelfIntersections; //Set adjecent segments to be recomputed in terms of length change
                                seg2.ComputeLengthChange = options.HandleCreatedLocalSelfIntersections;
                                v.ComputeIntersection = options.HandleGeneralCreatedSelfIntersections;
                            }
                        }

                    }
                } while (recheckTranlsations);
                

                //Find maximum offset possible
                double maxOffset = offset;

                if (options.HandleCreatedLocalSelfIntersections || options.HandleGeneralCreatedSelfIntersections)
                {
                    //Compute how much the length of each segment is affected by a unit offset (length 1)
                    //First whileloop iteration runs though all segemnts, later loop iterations only recomputes for segments that have seen a change
                    for (int i = 0; i < segments.Count; i++)
                    {
                        if (!segments[i].ComputeLengthChange)
                            continue;

                        double change;  //Change length change given a unit offset (length 1)
                        if (!isClosed && i == 0)
                            change = vertices[i + 1].AdjacentSegmentLengthChange; //For open curves, the first vertex has no impact
                        else if (!isClosed && i == segments.Count - 1)
                            change = vertices[i].AdjacentSegmentLengthChange;     //For open curves the last vertex has no impact
                        else
                        {
                            change = vertices[i].AdjacentSegmentLengthChange + vertices[(i + 1) % vertices.Count].AdjacentSegmentLengthChange;  //For closed curves, as well as non-start points, both vertices impact
                        }
                        segments[i].SegmentLengthChange = change;   //Set length change 
                        segments[i].ComputeLengthChange = false;    //Set computation flag
                    }

                    //Compute the maximum offset allowed for this main iteration.
                    //The maximum offset is set to be the smaller of the remaining offset value and the minimum offset value required to make any segment get a length of 0
                    for (int i = 0; i < segments.Count; i++)
                    {
                        double offsetScale = segments[i].Length / segments[i].SegmentLengthChange; //offset required for element length to become 0 length
                        if (offsetScale < 0)    //Positive value means segments are getting longer, hence no limit
                            maxOffset = Math.Min(maxOffset, -offsetScale);  //Set max offset to maxmimum of offset value and 
                    }
                }

                if (options.HandleGeneralCreatedSelfIntersections)
                    maxOffset = Math.Min(maxOffset, ComputeMaxOffsetUntilIntersection(segments, vertices, offset, isClosed));

                //Offset the points
                for (int i = 0; i < vertices.Count; i++)
                {
                    var v = vertices[i];
                    v.Position = v.Position + v.Translation * maxOffset;
                }

                //Reduce the remaining offset by amount used up in this iteration
                offset -= maxOffset;

                //If remaining offset, make adjustments (remove 0 length segments etc) for next iteration
                if (offset > 0)
                {
                    List<int> toBeRemoved = new List<int>();
                    if (options.HandleCreatedLocalSelfIntersections)
                    {
                        for (int i = 0; i < segments.Count; i++)
                        {
                            var seg = segments[i];
                            double l = seg.Length + seg.SegmentLengthChange * maxOffset;   //new length of segment
                            if (l < distTol)    //Segment vansihes -> to be removed
                            {
                                //If smaller than tolerance, add to be removed from segment and vertex lists
                                toBeRemoved.Add(i);

                                //Set vertices and segments that are to be recomputed in terms of translation vectors
                                vertices[(i + 1) % vertices.Count].ComputeTranlation = true;
                            }
                            else
                                segments[i].Length = l;
                        }

                        foreach (int i in toBeRemoved.OrderByDescending(x => x))
                        {
                            vertices.RemoveAt(i);
                            segments.RemoveAt(i);
                        }

                    }
                    if (options.HandleGeneralCreatedSelfIntersections)
                    {
                        if (toBeRemoved.Count == 0)
                        {
                            CheckIntersectionAndCullSegments(segments, vertices, normal, isClosed, maxOffset, distTol);
                        }
                        else
                        {
                            foreach (OffsetVertex vertex in vertices)
                            {
                                if (vertex.AnySegmentInRangeForIntersection)
                                    vertex.ComputeIntersection = true;

                            }
                        }
                    }
                }

                firstIteration = false;
            }

            cPts = vertices.Select(x => x.Position).ToList();

            if (isClosed && cPts.Count > 0)
                cPts.Add(cPts[0]);

            if (cPts.Count < 2 || (isClosed && cPts.Count < 4))
            {
                Base.Compute.RecordWarning("Offset leads to curve vanishing.");
            }

            return new Polyline() { ControlPoints = cPts };
        }

        /***************************************************/

        [PreviousVersion("7.2", "BH.Engine.Geometry.Modify.Offset(BH.oM.Geometry.PolyCurve, System.Double, BH.oM.Geometry.Vector, System.Boolean, System.Double)")]
        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves")]
        [Output("curve", "Resulting offset")]
        public static PolyCurve Offset(this PolyCurve curve, double offset, Vector normal = null, OffsetOptions options = null, double tolerance = Tolerance.Distance)
        {

            if (curve == null || curve.Length() < tolerance)
                return null;

            options = options ?? new OffsetOptions();
            bool tangentExtensions = options.TangentExtensions;

            //if there are only Line segmensts switching to polyline method which is more reliable 
            if (curve.Curves.All(x => x is Line))
            {
                Polyline polyline = ((Polyline)curve).Offset(offset, normal, options, tolerance);
                if (polyline == null)
                    return null;

                return new PolyCurve { Curves = polyline.SubParts().Cast<ICurve>().ToList() };
            }

            List<ICurve> subParts = curve.SubParts();
            //Check if contains any circles, if so, handle them explicitly, and offset any potential leftovers by backcalling this method
            if (subParts.Any(x => x is Circle))
            {
                IEnumerable<Circle> circles = subParts.Where(x => x is Circle).Cast<Circle>().Select(x => x.Offset(offset, normal, options, tolerance));
                PolyCurve nonCirclePolyCurve = new PolyCurve { Curves = curve.Curves.Where(x => !(x is Circle)).ToList() };
                if (nonCirclePolyCurve.Curves.Count != 0)
                    nonCirclePolyCurve = nonCirclePolyCurve.Offset(offset, normal, options, tolerance);

                nonCirclePolyCurve.Curves.AddRange(circles);
                return nonCirclePolyCurve;
            }

            if (!curve.IsPlanar(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Offset works only on planar curves");
                return null;
            }

            if (curve.IsSelfIntersecting(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Offset works only on non-self intersecting curves");
                return null;
            }

            if (offset == 0)
                return curve;

            bool isClosed = curve.IsClosed(tolerance);
            if (normal == null)
            {
                if (!isClosed)
                {
                    BH.Engine.Base.Compute.RecordError("Normal is missing. Normal vector is not needed only for closed curves");
                    return null;
                }
                else
                    normal = curve.Normal();
            }

            if (offset > 0.05 * curve.Length())
                return (curve.Offset(offset / 2, normal, options, tolerance))?.Offset(offset / 2, normal, options, tolerance);

            PolyCurve result = new PolyCurve();

            Vector normalNormalised = normal.Normalise();

            //First - offseting each individual element
            List<ICurve> offsetCurves = new List<ICurve>();
            foreach (ICurve crv in subParts)
                if (crv.IOffset(offset, normal, options, tolerance) != null)
                    offsetCurves.Add(crv.IOffset(offset, normal, options, tolerance));

            int counter = 0;
            //removing curves that are on a wrong side of the main curve
            for (int i = 0; i < offsetCurves.Count; i++)
            {
                Point sp = offsetCurves[i].IStartPoint();
                Point ep = offsetCurves[i].IEndPoint();
                Point mp = offsetCurves[i].IPointAtParameter(0.5);
                Point spOnCurve = curve.ClosestPoint(sp);
                Point epOnCurve = curve.ClosestPoint(ep);
                Point mpOnCurve = curve.ClosestPoint(mp);
                Vector sTan = curve.TangentAtPoint(spOnCurve, tolerance);
                Vector eTan = curve.TangentAtPoint(epOnCurve, tolerance);
                Vector mTan = curve.TangentAtPoint(mpOnCurve, tolerance);
                Vector sCheck = sp - spOnCurve;
                Vector eCheck = ep - epOnCurve;
                Vector mCheck = mp - mpOnCurve;
                Vector sCP = sTan.CrossProduct(sCheck).Normalise();
                Vector eCP = eTan.CrossProduct(eCheck).Normalise();
                Vector mCP = mTan.CrossProduct(mCheck).Normalise();
                if (offset > 0)
                {
                    if (sCP.IsEqual(normalNormalised, tolerance) && eCP.IsEqual(normalNormalised, tolerance) && mCP.IsEqual(normalNormalised, tolerance))
                    {
                        offsetCurves.RemoveAt(i);
                        i--;
                        counter++;
                    }
                }
                else
                {
                    if (!sCP.IsEqual(normalNormalised, tolerance) && !eCP.IsEqual(normalNormalised, tolerance) && !mCP.IsEqual(normalNormalised, tolerance))
                    {
                        offsetCurves.RemoveAt(i);
                        i--;
                        counter++;
                    }
                }
            }

            //Again if there are only Line segments switching to polyline method as it is more reliable 
            if (offsetCurves.All(x => x is Line))
            {
                Polyline polyline = new Polyline { ControlPoints = curve.DiscontinuityPoints() };
                result.Curves.AddRange(polyline.Offset(offset, normal, options, tolerance).SubParts());
                return result;
            }

            bool connectingError = false;
            //Filleting offset curves to create continuous curve
            for (int i = 0; i < offsetCurves.Count; i++)
            {
                int j;
                if (i == offsetCurves.Count - 1)
                {
                    if (isClosed)
                    {
                        j = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                    j = i + 1;

                PolyCurve temp = offsetCurves[i].Fillet(offsetCurves[j], tangentExtensions, true, false, tolerance);
                if (temp == null) //trying to fillet with next curve 
                {
                    offsetCurves.RemoveAt(j);

                    if (j == 0)
                        i--;

                    if (j == offsetCurves.Count)
                        j = 0;
                    temp = offsetCurves[i].Fillet(offsetCurves[j], tangentExtensions, true, false, tolerance);
                }

                if (!(temp == null)) //inserting filetted curves
                {
                    if (j != 0)
                    {
                        offsetCurves.RemoveRange(i, 2);
                        offsetCurves.InsertRange(i, temp.Curves);
                    }
                    else
                    {
                        offsetCurves.RemoveAt(i);
                        offsetCurves.RemoveAt(0);
                        offsetCurves.InsertRange(i - 1, temp.Curves);
                    }
                    i = i + temp.Curves.Count - 2;
                }
                else
                    connectingError = true;
            }

            //removing curves that are to close to the main curve
            for (int i = 0; i < offsetCurves.Count; i++)
            {
                if ((offsetCurves[i].IPointAtParameter(0.5).Distance(curve) + tolerance < Math.Abs(offset) &&
                     (offsetCurves[i].IStartPoint().Distance(curve) + tolerance < Math.Abs(offset) ||
                      offsetCurves[i].IEndPoint().Distance(curve) + tolerance < Math.Abs(offset))))
                {
                    PolyCurve temp = offsetCurves[((i - 1) + offsetCurves.Count) % offsetCurves.Count].Fillet(offsetCurves[(i + 1) % offsetCurves.Count], tangentExtensions, true, false, tolerance);
                    if (temp != null)
                    {
                        if (i == 0)
                        {
                            offsetCurves.RemoveRange(0, 2);
                            offsetCurves.RemoveAt(offsetCurves.Count - 1);
                            offsetCurves.InsertRange(0, temp.Curves);
                            i = temp.Curves.Count - 1;
                        }
                        else if (i == offsetCurves.Count - 1)
                        {
                            offsetCurves.RemoveRange(i - 1, 2);
                            offsetCurves.RemoveAt(0);
                            offsetCurves.InsertRange(offsetCurves.Count - 1, temp.Curves);
                            i = offsetCurves.Count - 1;
                        }
                        else
                        {
                            offsetCurves.RemoveRange(i - 1, 3);
                            offsetCurves.InsertRange(i - 1, temp.Curves);
                            i = i - 3 + temp.Curves.Count;
                        }
                    }

                    if (offsetCurves.Count < 1)
                    {
                        Base.Compute.ClearCurrentEvents();
                        Base.Compute.RecordError("Method failed to produce correct offset. Returning null.");
                        return null;
                    }
                    counter++;
                }
            }

            Base.Compute.ClearCurrentEvents();

            if (connectingError)
                Base.Compute.RecordWarning("Couldn't connect offset subCurves properly.");

            if (offsetCurves.Count == 0)
            {
                Base.Compute.RecordError("Method failed to produce correct offset. Returning null.");
                return null;
            }

            List<PolyCurve> resultList = Compute.IJoin(offsetCurves, tolerance);

            if (resultList.Count == 1)
                result = resultList[0];
            else
            {
                result.Curves = offsetCurves;
                Base.Compute.RecordWarning("Offset may be wrong. Please inspect the results.");
            }

            if (counter > 0)
                Base.Compute.RecordWarning("Reduced " + counter + " line(s). Please inspect the results.");

            if (result.IsSelfIntersecting(tolerance) || result.CurveIntersections(curve, tolerance).Count != 0)
                Base.Compute.RecordWarning("Intersections occured. Please inspect the results.");

            if (isClosed && !result.IsClosed(tolerance))
                Base.Compute.RecordError("Final curve is not closed. Please inspect the results.");

            return result;
        }


        /***************************************************/
        /***  Public methods - Interfaces                ***/
        /***************************************************/

        [PreviousVersion("7.2", "BH.Engine.Geometry.Modify.IOffset(BH.oM.Geometry.ICurve, System.Double, BH.oM.Geometry.Vector, System.Boolean, System.Double)")]
        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a closed curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc")]
        [Output("curve", "Resulting offset")]
        public static ICurve IOffset(this ICurve curve, double offset, Vector normal = null, OffsetOptions options = null, double tolerance = Tolerance.Distance)
        {
            return Offset(curve as dynamic, offset, normal, options, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static ICurve Offset(this ICurve curve, double offset, Vector normal = null, OffsetOptions options = null, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"Offset is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }


        /***************************************************/
        /***  Private Methods                            ***/
        /***************************************************/

        [Description("Support method for Polyline offsets for the case of two adjecent polyline segments creating a acute angle within provided tolerance. This case is handled either by vertex and segment removal, or introduction of additional vertices.")]
        private static double ComputeMaxOffsetUntilIntersection(List<OffsetSegment> segments, List<OffsetVertex> vertices, double offset, bool isClosed)
        {
            double overallMaxOffset = offset;
            for (int i = 0; i < segments.Count; i++)
            {
                if (!isClosed)  //Will never happen for end vertices of open curves
                {
                    if (i == 0 || i == vertices.Count - 1)
                        continue;
                }
                OffsetVertex vertex = vertices[i];
                if (!vertex.ComputeIntersection)    //Intersection check does not need to be recomputed
                    continue;

                vertex.ComputeIntersection = false;
                vertex.AnySegmentInRangeForIntersection = false;    //Set flag to false to start

                Vector trans = vertex.Translation;
                double maxOffset = offset;
                int segmentIntersected = -1;

                for (int j = 0; j < segments.Count; j++)
                {
                    //Skip segments adjecent
                    if (i == j)
                        continue;

                    if (i == 0)
                    {
                        if (j == segments.Count - 1)
                            continue;
                    }
                    else if (j == i - 1)
                        continue;

                    OffsetSegment segment = segments[j];
                    Vector segOrtho = segment.Orthogonal;
                    var startVertex = vertices[j];
                    Point segStart = startVertex.Position;

                    double dot1 = segOrtho.DotProduct(segStart - vertex.Position);
                    double dot2 = segOrtho.DotProduct(trans);
                    double reqOffset = dot1 / (dot2 - 1); //Required offset for the vertex to end up on the segment-plane after it has been offset

                    if (reqOffset > 0 && reqOffset < maxOffset)  //Offset in the correct direction and samller than previous max.
                    {
                        vertex.AnySegmentInRangeForIntersection = true;
                        //Check if the point ends up on the finite segment
                        Point vOff = vertex.Position + reqOffset * trans;  //Point translated with offset
                        Point stOff = startVertex.Position + reqOffset * startVertex.Translation;
                        var enVertex = vertices[(j + 1) % vertices.Count];
                        Point enOff = enVertex.Position + reqOffset * enVertex.Translation;

                        double sqLen = stOff.SquareDistance(enOff);

                        if (vOff.SquareDistance(stOff) < sqLen && vOff.SquareDistance(enOff) < sqLen)   //Point ends on line
                        {
                            maxOffset = reqOffset;
                            segmentIntersected = j;
                        }
                    }
                }

                if (segmentIntersected != -1)
                {
                    vertices[i].OffsetUntilIntersection = maxOffset;
                    vertices[i].SegmentIntersected = segmentIntersected;
                }
                else if (vertex.SegmentIntersected != -1)
                {
                    vertices[i].OffsetUntilIntersection = double.MaxValue;
                    vertices[i].SegmentIntersected = -1;
                }
            }

            foreach (var vertex in vertices)
            {
                if (vertex.SegmentIntersected != -1)
                    overallMaxOffset = Math.Min(vertex.OffsetUntilIntersection, overallMaxOffset);
            }
            return overallMaxOffset;
        }

        /***************************************************/

        private static void CheckIntersectionAndCullSegments(List<OffsetSegment> segments, List<OffsetVertex> vertices, Vector normal, bool isClosed, double maxOffset, double distTol)
        {
            if (isClosed)
            {
                List<Tuple<int, int>> vertSegInter = new List<Tuple<int, int>>();
                List<Tuple<List<int>, bool>> vertexLoops = new List<Tuple<List<int>, bool>>();
                for (int i = 0; i < vertices.Count; i++)
                {
                    var v = vertices[i];
                    if (v.SegmentIntersected != -1)
                    {
                        double remOffset = vertices[i].OffsetUntilIntersection - maxOffset;
                        if (remOffset < distTol)    //Intersection between vertex and another segment
                        {
                            vertSegInter.Add(new Tuple<int, int>(i, v.SegmentIntersected));
                            v.ComputeTranlation = true;
                            v.ComputeIntersection = true;
                            segments[v.SegmentIntersected].RecomputeLength = true;
                        }
                        else
                            vertices[i].OffsetUntilIntersection = remOffset;
                    }
                }
                if (vertSegInter.Count > 0)
                {
                    List<int> toRemove = new List<int>();
                    bool decreseSegRemovalIndex = false;

                    if (vertSegInter.Count > 1)
                    {
                        List<Tuple<int, int>> transfers = GenerateTransfers(vertSegInter, vertices.Count);

                        var loops = new List<List<int>>();

                        for (int i = 0; i < vertSegInter.Count; i++)
                        {
                            loops.Add(GenerateLoop(vertSegInter[i].Item1, vertSegInter[i].Item2, vertices.Count, transfers));
                            loops.Add(GenerateLoop(vertSegInter[i].Item2 + 1, vertSegInter[i].Item1, vertices.Count, transfers));
                        }

                        List<int> largestLoop = new List<int>();
                        double maxArea = double.MinValue;

                        for (int i = 0; i < loops.Count; i++)
                        {
                            double area = LoopArea(loops[i], vertices, normal);
                            if (area > maxArea)
                            {
                                maxArea = area;
                                largestLoop = loops[i];
                            }
                        }
                        bool shiftList = false;
                        List<int> segmentLoop = largestLoop.ToList();
                        Tuple<int, int> finalInter = vertSegInter.First(x => x.Item1 == segmentLoop[segmentLoop.Count - 1]);
                        shiftList = finalInter.Item2 < finalInter.Item1;
                        segmentLoop[segmentLoop.Count - 1] = finalInter.Item2;

                        vertices[largestLoop.First()].ComputeTranlation = true;
                        vertices[largestLoop.Last()].ComputeTranlation = true;

                        List<int> vertToRemove = Enumerable.Range(0, vertices.Count).Except(largestLoop).OrderByDescending(x => x).ToList();
                        List<int> segToRemove = Enumerable.Range(0, segments.Count).Except(segmentLoop).OrderByDescending(x => x).ToList();



                        foreach (int i in vertToRemove)
                            vertices.RemoveAt(i);
                        
                        foreach (int i in segToRemove)
                            segments.RemoveAt(i);

                        if (shiftList)
                        {
                            var temp = segments[0];
                            segments.RemoveAt(0);
                            segments.Add(temp);
                        }

                    }
                    else
                    {
                        int vIndex = vertSegInter[0].Item1;
                        int sIndex = vertSegInter[0].Item2;

                        List<int> loop1 = GenerateLoop(vIndex, sIndex, vertices.Count);
                        List<int> loop2 = GenerateLoop((sIndex + 1) % vertices.Count, vIndex, vertices.Count);

                        double area1 = LoopArea(loop1, vertices, normal);
                        double area2 = LoopArea(loop2, vertices, normal);

                        List<int> vertLoop;

                        if (area1 > area2)
                        {
                            vertLoop = loop1;
                            decreseSegRemovalIndex = false;
                        }
                        else
                        {
                            vertLoop = loop2;
                            decreseSegRemovalIndex = true;
                        }

                        segments[sIndex].Length = vertices[vertLoop.First()].Position.Distance(vertices[vertLoop.Last()].Position);
                        toRemove = Enumerable.Range(0, vertices.Count).Except(vertLoop).OrderByDescending(x => x).ToList();


                        foreach (int rem in toRemove)
                        {
                            vertices.RemoveAt(rem);
                            int segRem = decreseSegRemovalIndex ? rem - 1 : rem;
                            if (segRem < 0)
                            {
                                segments.RemoveAt(segments.Count - 1);
                                var temp = segments[0];
                                segments.RemoveAt(0);
                                segments.Add(temp);
                            }
                            else
                                segments.RemoveAt(segRem);

                        }
                    }


                    for (int i = 0; i < segments.Count; i++)
                    {
                        if (segments[i].RecomputeLength)
                        {
                            OffsetVertex prev = vertices[i];
                            OffsetVertex next = vertices[(i + 1) % segments.Count];
                            segments[i].Length = prev.Position.Distance(next.Position);
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i < vertices.Count - 1; i++)
                {
                    var v = vertices[i];
                    if (v.SegmentIntersected != -1)
                    {
                        double remOffset = vertices[i].OffsetUntilIntersection - maxOffset;
                        if (remOffset < distTol)    //Intersection between vertex and another segment
                        {
                            int endIndex = v.SegmentIntersected; //Index of segment intersected
                            int startRem, endRem;
                            if (endIndex > i)
                            {
                                startRem = i;
                                endRem = endIndex;
                            }
                            else
                            {
                                startRem = endIndex;
                                endRem = i;
                                i -= (endRem - startRem);
                            }

                            int partsToRemove = endRem - startRem;
                            while (partsToRemove > 0)
                            {
                                segments.RemoveAt(startRem);
                                vertices.RemoveAt(startRem + 1);
                                partsToRemove--;
                            }

                            segments[startRem + 1].Length = v.Position.Distance(vertices[startRem + 1].Position);

                            vertices[startRem].ComputeIntersection = true;
                            vertices[startRem].ComputeTranlation = true;
                            segments[startRem].ComputeLengthChange = true;
                        }
                        else
                            vertices[i].OffsetUntilIntersection = remOffset;
                    }
                }
            }

            for (int j = 0; j < vertices.Count; j++)
            {
                if (vertices[j].SegmentIntersected != -1)
                    vertices[j].ComputeIntersection = true;
            }
        }

        /***************************************************/

        private static List<int> GenerateLoop(int start, int end, int overallCount)
        {
            List<int> indecies = new List<int>();
            int i = start;
            while (i != (end + 1) % overallCount)
            {
                indecies.Add(i);
                i = (i + 1) % overallCount;
            }
            return indecies;
        }

        /***************************************************/

        private static List<int> GenerateLoop(int start, int end, int overallCount, List<Tuple<int,int>> transfers)
        {
            List<int> indecies = new List<int>();
            if (start > end)
                end += overallCount;

            int i = start;
            bool canTransfer = false;

            while (i <= end && indecies.Count < overallCount)
            {

                indecies.Add(i);
                bool transfered = false;

                if (canTransfer)
                {
                    for (int j = 0; j < transfers.Count; j++)
                    {
                        if (transfers[j].Item1 == i % overallCount)
                        {
                            int next = transfers[j].Item2;
                            while (next < i)
                                next += overallCount;
                            i = next;
                            transfered = true;
                        }
                    }
                    canTransfer = !transfered;
                }
                else
                    canTransfer = true;

                if (!transfered)
                    i++;

            }

            return indecies.Select(x => x % overallCount).ToList();
        }

        /***************************************************/

        private static List<int> SegmentLoop(List<int> vertLoop, List<Tuple<int, int>> vertSegInter, out int segToRecompute)
        {
            List<int> seg = new List<int>();
            segToRecompute = -1;
            for (int i = 0; i < vertLoop.Count; i++)
            {
                if (i > 0)
                {
                    int interInd = vertSegInter.FindIndex(x => x.Item1 == vertLoop[i]);
                    if (interInd != -1)
                    {
                        seg.Add(vertSegInter[interInd].Item2);
                        segToRecompute = interInd;
                    }
                    else
                        seg.Add(vertLoop[i]);
                }
                else
                    seg.Add(vertLoop[i]);
            }
            return seg;
        }

        /***************************************************/

        private static List<Tuple<int, int>> GenerateTransfers(List<Tuple<int, int>> vertSegInter, int count)
        {
            List<Tuple<int, int>> transfers = new List<Tuple<int, int>>();

            foreach (var gr in vertSegInter.GroupBy(x => x.Item2))
            {
                if (gr.Count() == 1)
                {
                    var item = gr.First();
                    int v = item.Item1;
                    int s = item.Item2;
                    transfers.Add(new Tuple<int, int>(v, (s + 1) % count));
                    transfers.Add(new Tuple<int, int>(s, v));
                }
                else
                {
                    var ordered = gr.OrderBy(x => (x.Item1 > x.Item2 ? x.Item1 - x.Item2 + count : x.Item1 - x.Item2)).ToList();
                    transfers.Add(new Tuple<int, int>(ordered[0].Item2, ordered[0].Item1));
                    for (int i = 0; i < ordered.Count - 1; i++)
                    {
                        transfers.Add(new Tuple<int, int>(ordered[i].Item1, ordered[i + 1].Item1));
                    }
                    transfers.Add(new Tuple<int, int>(ordered[ordered.Count - 1].Item1, (ordered[ordered.Count - 1].Item2 + 1) % count));
                }
            }

            return transfers;
        }

        /***************************************************/

        private static List<int> LargestAreaLoop(List<List<int>> loops, List<OffsetVertex> vertices, Vector normal)
        {
            double maxArea = double.MinValue;
            List<int> maxLoop = null;
            foreach (var loop in loops)
            {
                double area = LoopArea(loop, vertices, normal);
                if (area > maxArea)
                {
                    maxArea = area; 
                    maxLoop = loop;
                }
            }
            return maxLoop;
        }

        /***************************************************/

        private static double LoopArea(List<int> loop, List<OffsetVertex> vertices, Vector normal)
        {
            if (loop.Count < 3)
                return 0;

            List<Point> pts = new List<Point>();
            for (int i = 0; i < loop.Count; i++)
            {
                pts.Add(vertices[loop[i]].Position);
            }

            Point avg = pts.Average();
            pts.Add(vertices[loop[0]].Position);
            double x = 0, y = 0, z = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                int j = (i + 1) % pts.Count;
                Vector prod = Query.CrossProduct(pts[i] - avg, pts[j] - avg);
                x += prod.X;
                y += prod.Y;
                z += prod.Z;
            }

            return Math.Abs((new Vector { X = x, Y = y, Z = z } * normal) * 0.5);
        }

        /***************************************************/

        [Description("Support method for Polyline offsets for the case of two adjecent polyline segments creating a acute angle within provided tolerance. This case is handled either by vertex and segment removal, or introduction of additional vertices.")]
        private static bool HandleParalellAdjecentPolylinearSegments(List<OffsetSegment> segments, List<OffsetVertex> vertices, Vector normal, bool isClosed, bool firstIteration, double distTol, int i, int prev, double angleTol)
        {
            bool outwards = false;
            if (firstIteration) //Only need to check if "Inwards" or "Outwards" for first iteration, as subsequent iterations all can be assumed to be inwards (vanishing)
                outwards = CheckAcuteCornerOutwards(segments, isClosed,i, prev, angleTol, distTol);
            
            if (outwards)
            {
                //For the case of outwards, an additional vertex and segment is inserted, creating a right angled corner from the acute corner angle
                vertices.Insert(i, new OffsetVertex() { Position = vertices[i].Position, ComputeTranlation = true });
                Vector tanNew = (segments[i].Orthogonal - segments[prev].Orthogonal).Normalise();
                segments.Insert(i, new OffsetSegment() { Length = 0, Tangent = tanNew, Orthogonal = tanNew.CrossProduct(normal).Normalise(), SegmentLengthChange = 0, ComputeLengthChange = true });


                //More segments need to be recomputed due to the insertion and change
                segments[prev].ComputeLengthChange = true;
                segments[i].ComputeLengthChange = true;
                segments[(i + 1) % segments.Count].ComputeLengthChange = true;

                //Same for vertecies requiring re-computation
                vertices[(i + 1) % vertices.Count].ComputeTranlation = true;

            }
            else
            {
                if (vertices[prev].Position.Distance(vertices[(i + 1) % vertices.Count].Position) < distTol)
                {
                    //If the corner removed leads to the previous and next points being adjecent, remove two verticesm as well as the two segments leading up to the removed vertex

                    vertices[(i + 1) % vertices.Count].ComputeTranlation = true;    //Kept vertex needs recomputation of translation vector

                    vertices.RemoveAt(i);
                    segments.RemoveAt(i);
                    if (i < prev)
                        prev--;

                    vertices.RemoveAt(prev);
                    segments.RemoveAt(prev);

                    if (vertices.Count <= 1)
                        return false;   //Vertex list small enough that nothing is left. Method to return empty polyline.

                    if (i >= 2)
                    {
                        segments[i - 2].ComputeLengthChange = true;
                        segments[(i - 1) % segments.Count].ComputeLengthChange = true;
                    }
                    else
                    {
                        segments[0].ComputeLengthChange = true;
                        if (isClosed)
                            segments[segments.Count - 1].ComputeLengthChange = true;
                    }
                }
                else
                {
                    //If not, remove one of the segments and one of the vertices
                    vertices.RemoveAt(i);
                    segments.RemoveAt(i);

                    if (vertices.Count <= 1)
                        return false;   //Vertex list small enough that nothing is left. Method to return empty polyline.

                    if (i < prev)
                        prev--;

                    //And recompute the new direction for the other
                    Vector tan = vertices[i % vertices.Count].Position - vertices[prev].Position;
                    double lengthNew = tan.Length();
                    tan = tan * (1 / lengthNew);
                    segments[prev] = new OffsetSegment() { Length = lengthNew, Tangent = tan, Orthogonal = tan.CrossProduct(normal).Normalise(), SegmentLengthChange = 0, ComputeLengthChange = true };

                    vertices[prev].ComputeTranlation = true;
                    vertices[i % vertices.Count].ComputeTranlation = true;
                    segments[i % segments.Count].ComputeLengthChange = true;

                }
            }

            return true;
        }

        /***************************************************/

        [Description("Method that finds if a particular corner should be lead to an outwards of inwards offset.")]
        private static bool CheckAcuteCornerOutwards(List<OffsetSegment> segments, bool isClosed, int i, int prev, double angleTol, double distTol)
        {

            Vector t1 = segments[prev].Tangent;

            //Checks whether the offset is "outwards" or "inwards" for the current corner
            int i0 = i - 2;

            if (isClosed && i0 < 0)
                i0 = segments.Count + i0;

            Vector t0 = null;   //Tangent vector of the segment coming into the corner before the current vertex being evaluated. 
            List<int> beforeSegs = new List<int>() { prev };

            if (i0 >= 0)
            {
                t0 = segments[i0].Tangent;

                while (t0.IsParallel(t1, angleTol) != 0)    //Ensure not parallel with segments going into the corner. If so, take the one before
                {
                    beforeSegs.Add(i0);
                    i0--;
                    if (i0 < 0)
                    {
                        if (isClosed)
                            i0 = segments.Count - 1;    //Set index to last for closed curves when looping around the list
                        else
                        {
                            t0 = null;  //No suitable vector exists
                            break;
                        }
                    }
                    if (i0 == i)    //Looped all the way around, all segments are parallel
                    {
                        return false;
                    }
                    t0 = segments[i0].Tangent;
                }
            }


            int i3 = i + 1;
            if (isClosed && i3 > segments.Count - 1)
                i3 = i3 % segments.Count;

            Vector t3 = null;   //Vector
            List<int> afterSegs = new List<int>() { i };
            if (i3 <= segments.Count - 1)
            {
                t3 = segments[i3].Tangent;

                while (t3.IsParallel(t1, angleTol) != 0)
                {
                    afterSegs.Add(i3);
                    i3++;
                    if (i3 > segments.Count - 1)
                    {
                        if (isClosed)
                            i3 = 0;
                        else
                        {
                            t3 = null;
                            break;
                        }
                    }
                    if (i3 == prev)    //Looped all the way around, all segments are parallel
                    {
                        return false;
                    }

                    t3 = segments[i3].Tangent;

                }
            }

            if (t0 != null && t3 != null)
            {
                double l1 = beforeSegs.Sum(x => segments[x].Length);// segments[prev].Item1;
                double l2 = afterSegs.Sum(x => segments[x].Length);// segments[i].Item1;
                double lengthDif = l1 - l2;

                //Dot products used for sign, if dotproduct is positive, the vectors are pointing in roungly the same direction
                double t0n1 = t0.DotProduct(segments[prev].Orthogonal);
                double t3n1 = t3.DotProduct(segments[prev].Orthogonal);

                if (Math.Abs(lengthDif) < distTol)
                {
                    if (t0n1 * t3n1 < 0)
                    {
                        //Same side
                        if (t0n1 < 0)
                            return -t0.DotProduct(t1) > t3.DotProduct(t1);
                        else
                            return -t0.DotProduct(t1) < t3.DotProduct(t1);
                    }
                    else
                        return t0n1 < 0;
                }
                else if (lengthDif < 0)
                    return t0n1 < 0;
                else
                    return t3n1 < 0;

            }
            else if (t3 != null)
            {
                //For case of no incoming vector (t0) the corner is outwards if the tangent of the next vector is pointing in the oposite direction to the orthogonal vector coing into the corner
                return t3.DotProduct(segments[prev].Orthogonal) < 0; 
            }
            else if (t0 != null)
            {
                //For case of no outgoing vector (t3) the corner is outwards if the tangent of the incoming vector is pointing in the oposite direction to the orthogonal vector coing into the corner
                return t0.DotProduct(segments[prev].Orthogonal) < 0;
            }

            return false;
        }

        /***************************************************/

        private static void SortAndRemoveDuplicatesAndNegatives(this List<int> list) 
        {
            list.Sort();

            while (list.Count > 0)
            {
                if (list[0] < 0)
                    list.RemoveAt(0);
                else
                    break;
            }

            int index = list.Count - 1;
            while (index > 0)
            {
                if (list[index] == list[index - 1])
                {
                    list.RemoveAt(index);
                }

                index--;
            }
        }

        /***************************************************/

        private static List<ICurve> ExtendToPoint(this ICurve curve, Point startPoint, Point endPoint, bool tangentExtension, double tolerance)
        {
            //TODO:
            //Decide if useful enough to make public. If so, rewrite and test.

            double start = startPoint.Distance(curve.IStartPoint());
            double end = endPoint.Distance(curve.IEndPoint());

            if (startPoint.IIsOnCurve(curve, tolerance))
                start = -start;

            if (endPoint.IIsOnCurve(curve, tolerance))
                end = -end;

            List<ICurve> result = new List<ICurve>();

            if (!tangentExtension)
            {
                if (curve is Arc)
                    result.Add((curve as Arc).Extend(2 * Math.Asin(start / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, 2 * Math.Asin(end / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, false, tolerance));
                else
                    result.Add((curve as Line).Extend(start, end, false, tolerance));
            }
            else
            {
                if (curve is Arc)
                {
                    if (start >= 0 && end >= 0)
                        result.Add((curve as Arc).Extend(start, end, true, tolerance));
                    else if (start >= 0 && end < 0)
                        result.Add((curve as Arc).Extend(start, 2 * Math.Asin(end / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, true, tolerance));
                    else if (start < 0 && end >= 0)
                        result.Add((curve as Arc).Extend(2 * Math.Asin(start / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, end, true, tolerance));
                    else
                        result.Add((curve as Arc).Extend(2 * Math.Asin(start / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, 2 * Math.Asin(end / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, true, tolerance));
                }
                else
                    result.Add((curve as Line).Extend(start, end, true, tolerance));
            }

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] is PolyCurve)
                {
                    PolyCurve temp = (PolyCurve)result[i];
                    result.RemoveAt(i);
                    result.InsertRange(i, temp.Curves);
                }
            }

            return result;
        }

        /***************************************************/

        private static PolyCurve Fillet(this ICurve curve1, ICurve curve2, bool tangentExtensions, bool keepCurve1StartPoint, bool keepCurve2StartPoint, double tolerance = Tolerance.Distance)
        {
            //TODO:
            //Write a proper fillet method, test and make it public            

            if (!((curve1 is Line || curve1 is Arc) && (curve2 is Line || curve2 is Arc))) //for now works only with combinations of lines and arcs
            {
                Base.Compute.RecordError("Private method fillet is implemented only for PolyCurves consisting of Lines or Arcs.");
                return null;
            }

            List<PolyCurve> joinCurves = Compute.IJoin(new List<ICurve> { curve1, curve2 }, tolerance).ToList();

            if (joinCurves.Count == 1)
                return joinCurves[0];

            List<ICurve> resultCurves = new List<ICurve>();

            bool C1SP = keepCurve1StartPoint;
            bool C2SP = keepCurve2StartPoint;

            List<Point> intersections = curve1.ICurveIntersections(curve2, tolerance);
            if (intersections.Count > 2)
                Base.Compute.RecordError("Invalid number of intersections between curves. Two lines/arcs can have no more than two intersections.");
            else if (intersections.Count == 2 || intersections.Count == 1)
            {
                Point intersection = intersections[0];
                if (intersections.Count == 2)
                {
                    double maxdist = double.MaxValue;
                    if (C1SP)
                        foreach (Point p in intersections)
                        {
                            double dist = p.SquareDistance(curve1.IStartPoint());
                            if (dist < maxdist)
                            {
                                intersection = p;
                                maxdist = dist;
                            }
                        }
                    else
                        foreach (Point p in intersections)
                        {
                            double dist = p.SquareDistance(curve1.IEndPoint());
                            if (dist < maxdist)
                            {
                                intersection = p;
                                maxdist = dist;
                            }
                        }
                }
                else
                    intersection = intersections[0];

                if (C1SP && C2SP)
                {
                    resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, tolerance));
                    resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, tolerance));
                    resultCurves[1] = resultCurves[1].IFlip();
                }
                else if (C1SP && !C2SP)
                {
                    resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, tolerance));
                    resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, tolerance));
                }
                else if (!C1SP && C2SP)
                {
                    resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, tolerance));
                    resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, tolerance));
                    resultCurves[0] = resultCurves[0].IFlip();
                    resultCurves[1] = resultCurves[1].IFlip();
                }
                else
                {
                    resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, tolerance));
                    resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, tolerance));
                    resultCurves[0] = resultCurves[0].IFlip();
                }
            }
            else
            {
                if (curve1 is Line && curve2 is Line)
                {
                    Point intersection = (curve1 as Line).LineIntersection(curve2 as Line, true, tolerance);
                    if (C1SP && C2SP)
                    {
                        if ((curve1.IStartPoint().Distance((curve2 as Line), true) < curve1.IEndPoint().Distance((curve2 as Line), true) &&
                            !intersection.IIsOnCurve(curve1)) ||
                            (curve2.IStartPoint().Distance((curve1 as Line), true) < curve2.IEndPoint().Distance((curve1 as Line), true) &&
                             !intersection.IIsOnCurve(curve2)))
                        {
                            Base.Compute.RecordWarning("Couldn't provide correct fillet for given input");
                            return null;
                        }
                        resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, tolerance));
                        resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, tolerance));
                        resultCurves[1] = resultCurves[1].IFlip();
                    }
                    else if (C1SP && !C2SP)
                    {
                        if ((curve1.IStartPoint().Distance((curve2 as Line), true) < curve1.IEndPoint().Distance((curve2 as Line), true) &&
                            !intersection.IIsOnCurve(curve1)) ||
                            (curve2.IStartPoint().Distance((curve1 as Line), true) > curve2.IEndPoint().Distance((curve1 as Line), true) &&
                             !intersection.IIsOnCurve(curve2)))
                        {
                            Base.Compute.RecordWarning("Couldn't provide correct fillet for given input");
                            return null;
                        }
                        resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, tolerance));
                        resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, tolerance));
                    }
                    else if (!C1SP && C2SP)
                    {
                        if ((curve1.IStartPoint().Distance((curve2 as Line), true) > curve1.IEndPoint().Distance((curve2 as Line), true) &&
                            !intersection.IIsOnCurve(curve1)) ||
                            (curve2.IStartPoint().Distance((curve1 as Line), true) < curve2.IEndPoint().Distance((curve1 as Line), true) &&
                             !intersection.IIsOnCurve(curve2)))
                        {
                            Base.Compute.RecordWarning("Couldn't provide correct fillet for given input");
                            return null;
                        }
                        resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, tolerance));
                        resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, tolerance));
                        resultCurves[0] = resultCurves[0].IFlip();
                        resultCurves[1] = resultCurves[1].IFlip();
                    }
                    else
                    {
                        if ((curve1.IStartPoint().Distance((curve2 as Line), true) > curve1.IEndPoint().Distance((curve2 as Line), true) &&
                            !intersection.IIsOnCurve(curve1)) ||
                            (curve2.IStartPoint().Distance((curve1 as Line), true) > curve2.IEndPoint().Distance((curve1 as Line), true) &&
                             !intersection.IIsOnCurve(curve2)))
                        {
                            Base.Compute.RecordWarning("Couldn't provide correct fillet for given input");
                            return null;
                        }
                        resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, tolerance));
                        resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, tolerance));
                        resultCurves[0] = resultCurves[0].IFlip();
                    }
                }
                else
                {
                    Point intersection;
                    if (!tangentExtensions)
                    {
                        PolyCurve pCurve1 = new PolyCurve();
                        PolyCurve pCurve2 = new PolyCurve();
                        if (curve1 is Arc)
                            pCurve1.Curves.Add(new Circle { Centre = (curve1 as Arc).Centre(), Normal = (curve1 as Arc).Normal(), Radius = (curve1 as Arc).Radius });
                        else
                            pCurve1.Curves.Add(new Line { Start = (curve1 as Line).Start, End = (curve1 as Line).End, Infinite = true });

                        if (curve2 is Arc)
                            pCurve2.Curves.Add(new Circle { Centre = (curve2 as Arc).Centre(), Normal = (curve2 as Arc).Normal(), Radius = (curve2 as Arc).Radius });
                        else
                            pCurve2.Curves.Add(new Line { Start = (curve2 as Line).Start, End = (curve2 as Line).End, Infinite = true });

                        List<Point> curveIntersections = pCurve1.CurveIntersections(pCurve2, tolerance);
                        if (curveIntersections.Count == 0)
                        {
                            Base.Compute.RecordError("Curves' extensions do not intersect");
                            return null;
                        }
                        if (C1SP && C2SP)
                        {
                            intersection = curveIntersections.ClosestPoint(curve1.IEndPoint());

                            resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, tolerance));
                            resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, tolerance));
                            resultCurves[1] = resultCurves[1].IFlip();
                        }
                        else if (C1SP && !C2SP)
                        {
                            intersection = curveIntersections.ClosestPoint(curve1.IEndPoint());
                            resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, tolerance));
                            resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, tolerance));
                        }
                        else if (!C1SP && C2SP)
                        {
                            intersection = curveIntersections.ClosestPoint(curve1.IStartPoint());
                            resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, tolerance));
                            resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, tolerance));
                            resultCurves[0] = resultCurves[0].IFlip();
                            resultCurves[1] = resultCurves[1].IFlip();
                        }
                        else
                        {
                            intersection = curveIntersections.ClosestPoint(curve1.IStartPoint());
                            resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, tolerance));
                            resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, tolerance));
                            resultCurves[0] = resultCurves[0].IFlip();
                        }
                    }
                    else
                    {
                        if (C1SP && C2SP)
                        {
                            Line tanLine1 = Create.Line(curve1.IEndPoint(), curve1.IEndDir() / tolerance);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { curve1, tanLine1 } };

                            Line tanLine2 = Create.Line(curve2.IEndPoint(), curve2.IEndDir() / tolerance);
                            tanLine2.Infinite = false;
                            PolyCurve pCurve2 = new PolyCurve { Curves = { curve2, tanLine2 } };

                            List<Point> curveIntersecions = pCurve1.CurveIntersections(pCurve2, tolerance);
                            if (curveIntersecions.Count > 0)
                                intersection = curveIntersecions.ClosestPoint(curve1.IEndPoint());
                            else
                            {
                                Base.Compute.RecordWarning("Couldn't create fillet");
                                return null;
                            }

                            PolyCurve subResult1 = new PolyCurve
                            {
                                Curves = curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, tolerance).ToList()
                            };

                            PolyCurve subResult2 = new PolyCurve
                            {
                                Curves = curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, tolerance).ToList()
                            };

                            subResult2 = subResult2.Flip();

                            resultCurves.AddRange(subResult1.Curves);
                            resultCurves.AddRange(subResult2.Curves);
                        }
                        else if (C1SP && !C2SP)
                        {
                            Line tanLine1 = Create.Line(curve1.IEndPoint(), curve1.IEndDir() / tolerance);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { curve1, tanLine1 } };

                            Line tanLine2 = Create.Line(curve2.IStartPoint(), curve2.IStartDir().Reverse() / tolerance);
                            tanLine2.Infinite = false;
                            PolyCurve pCurve2 = new PolyCurve { Curves = { tanLine2, curve2 } };

                            List<Point> curveIntersecions = pCurve1.ICurveIntersections(pCurve2, tolerance);

                            if (curveIntersecions.Count > 0)
                                intersection = curveIntersecions.ClosestPoint(curve1.IEndPoint());
                            else
                            {
                                Base.Compute.RecordWarning("Couldn't create fillet");
                                return null;
                            }

                            resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, tolerance));
                            resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, tolerance));
                        }
                        else if (!C1SP && C2SP)
                        {
                            Line tanLine1 = Create.Line(curve1.IStartPoint(), curve1.IStartDir().Reverse() / tolerance);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { tanLine1, curve1 } };

                            Line tanLine2 = Create.Line(curve2.IEndPoint(), curve2.IEndDir() / tolerance);
                            tanLine2.Infinite = false;
                            PolyCurve pCurve2 = new PolyCurve { Curves = { curve2, tanLine2 } };

                            List<Point> curveIntersecions = pCurve1.ICurveIntersections(pCurve2, tolerance);
                            if (curveIntersecions.Count > 0)
                                intersection = curveIntersecions.ClosestPoint(curve1.IStartPoint());
                            else
                            {
                                Base.Compute.RecordWarning("Couldn't create fillet");
                                return null;
                            }
                            PolyCurve subResult1 = new PolyCurve
                            {
                                Curves = curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, tolerance).ToList()
                            };
                            PolyCurve subResult2 = new PolyCurve
                            {
                                Curves = curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, tolerance).ToList()
                            };

                            subResult1 = subResult1.Flip();
                            subResult2 = subResult2.Flip();

                            resultCurves.AddRange(subResult1.Curves);
                            resultCurves.AddRange(subResult2.Curves);
                        }
                        else
                        {
                            Line tanLine1 = Create.Line(curve1.IStartPoint(), curve1.IStartDir().Reverse() / tolerance);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { tanLine1, curve1 } };

                            Line tanLine2 = Create.Line(curve2.IStartPoint(), curve2.IStartDir().Reverse() / tolerance);
                            tanLine2.Infinite = false;
                            PolyCurve pCurve2 = new PolyCurve { Curves = { tanLine2, curve2 } };

                            List<Point> curveIntersecions = pCurve1.ICurveIntersections(pCurve2, tolerance);
                            if (curveIntersecions.Count > 0)
                                intersection = curveIntersecions.ClosestPoint(curve1.IStartPoint());
                            else
                            {
                                Base.Compute.RecordWarning("Couldn't create fillet");
                                return null;
                            }
                            PolyCurve subResult1 = new PolyCurve
                            {
                                Curves = curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, tolerance).ToList()
                            };
                            PolyCurve subResult2 = new PolyCurve
                            {
                                Curves = curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, tolerance).ToList()
                            };

                            subResult1 = subResult1.Flip();

                            resultCurves.AddRange(subResult1.Curves);
                            resultCurves.AddRange(subResult2.Curves);
                        }
                    }
                }
            }

            for (int i = resultCurves.Count - 1; i >= 0; i--)
                if (resultCurves[i] is PolyCurve)
                {
                    PolyCurve temp = (PolyCurve)resultCurves[i];
                    resultCurves.RemoveAt(i);
                    resultCurves.InsertRange(i, temp.Curves);
                }

            PolyCurve result = new PolyCurve { Curves = resultCurves };
            return result;
        }

        /***************************************************/
    }
}



