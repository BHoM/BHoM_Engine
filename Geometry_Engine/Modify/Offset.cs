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
            List<Tuple<double, Vector, Vector, double>> segments = new List<Tuple<double, Vector, Vector, double>>();
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
                    segments.Add(new Tuple<double, Vector, Vector, double>(length, tan, tan.CrossProduct(normal).Normalise(), 0));
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
            List<Tuple<Point, Vector, double, double, int>> vertices = cPts.Select(x => new Tuple<Point, Vector, double, double, int>(x, null, 0, double.MaxValue, -1)).ToList();

            //Tolerance used for checking if two adjecent segments are anti-parallel and hence creating a acute corner
            double sinTol = Math.Pow(Math.Sin(angleTol / 2), 2);

            //Construct list of segments and vertices which require (re)computation in terms of translation vectors and length change
            //For firt iteration is is all of the,
            List<int> cornerComputes = new List<int>(Enumerable.Range(0, vertices.Count));
            List<int> segmentsComputes = new List<int>(Enumerable.Range(0, segments.Count));

            List<int> cornerIntersectionComputes;
            if (options.HandleGeneralCreatedSelfIntersections)
                cornerIntersectionComputes = new List<int>(Enumerable.Range(0, vertices.Count));
            else
                cornerIntersectionComputes = new List<int>();

            bool firstIteration = true;

            //Loop and continue offseting until all offsets done.
            //Done to handle local apparent self intersections due to segments getting a length < 0 (being flipped) for concave offsets
            while (offset > 0)
            {
                //Compute corner offset vectors. First whileloop iteration computes for all corners
                //Subsequent iteration only updates corners that have seen a change
                while (cornerComputes.Count > 0)
                {
                    //De-queue top corner to be computed
                    int i = cornerComputes[0];
                    cornerComputes.RemoveAt(0);
                    Tuple<Point, Vector, double, double, int> v = vertices[i];
                    if (!isClosed && i == 0)    //Start point vector for open curves will simply be the orthogonal vector of the first segment
                        vertices[i] = new Tuple<Point, Vector, double, double, int>(v.Item1, segments[i].Item3, 0, v.Item4, v.Item5);
                    else if (!isClosed && i == vertices.Count - 1)  //Similar to start
                        vertices[i] = new Tuple<Point, Vector, double, double, int>(v.Item1, segments[i - 1].Item3, 0, v.Item4, v.Item5);
                    else
                    {
                        int prev = i == 0 ? segments.Count - 1 : i - 1;

                        var seg1 = segments[prev];
                        var seg2 = segments[i];

                        Vector dir = (seg1.Item3 + seg2.Item3) / 2; //Offset direction computed as sum of the previous and next orthogonal vectors

                        //Below factor gives the correct magnitude for corner scaling based on equal angle triangles.
                        //ortho projected onto the direction gives a right angled triangle with the same angles as the triangle
                        //formed by offset vetor with correct length and dir.
                        double length = dir.SquareLength();

                        if (options.HandleAdjacentParallelSegments && length < sinTol)    //Equivalent to check that angle between two adjecent segements is less than angle tolerance
                        {
                            if (!HandleParalellAdjecentPolylinearSegments(segments, vertices, cornerComputes, segmentsComputes, normal, isClosed, firstIteration, distTol, i, prev, angleTol))
                            {
                                Base.Compute.RecordWarning("Polyline reduced to nothing. Empty polyline returned.");
                                return new Polyline();
                            }
                        }
                        else
                        {
                            Vector trans = dir * (1 / length);
                            double vertLengthChange = seg1.Item2.DotProduct(trans);
                            vertices[i] = new Tuple<Point, Vector, double, double, int>(v.Item1, trans, vertLengthChange, v.Item4, v.Item5);
                        }
                    }
                }

                //Find maximum offset possible
                double maxOffset = offset;

                if (options.HandleCreatedLocalSelfIntersections || options.HandleGeneralCreatedSelfIntersections)
                {
                    //Compute how much the length of each segment is affected by a unit offset (length 1)
                    //First whileloop iteration runs though all segemnts, later loop iterations only recomputes for segments that have seen a change
                    while (segmentsComputes.Count > 0)
                    {
                        int i = segmentsComputes[0];
                        segmentsComputes.RemoveAt(0);
                        double change;  //Change length change given a unit offset (length 1)
                        if (!isClosed && i == 0)
                            change = vertices[i + 1].Item3; //For open curves, the first vertex has no impact
                        else if (!isClosed && i == segments.Count - 1)
                            change = vertices[i].Item3;     //For open curves the last vertex has no impact
                        else
                        {
                            change = vertices[i].Item3 + vertices[(i + 1) % vertices.Count].Item3;  //For closed curves, as well as non-start points, both vertices impact
                        }

                        segments[i] = new Tuple<double, Vector, Vector, double>(segments[i].Item1, segments[i].Item2, segments[i].Item3, change);
                    }

                    //Compute the maximum offset allowed for this main iteration.
                    //The maximum offset is set to be the smaller of the remaining offset value and the minimum offset value required to make any segment get a length of 0
                    for (int i = 0; i < segments.Count; i++)
                    {
                        double offsetScale = segments[i].Item1 / segments[i].Item4; //offset required for element length to become 0 length
                        if (offsetScale < 0)    //Positive value means segments are getting longer, hence no limit
                            maxOffset = Math.Min(maxOffset, -offsetScale);  //Set max offset to maxmimum of offset value and 
                    }
                }

                if(options.HandleGeneralCreatedSelfIntersections)
                    maxOffset = ComputeMaxOffsetUntilIntersection(segments, vertices, cornerIntersectionComputes, offset, isClosed);

                //Offset the points
                for (int i = 0; i < vertices.Count; i++)
                {
                    var v = vertices[i];
                    vertices[i] = new Tuple<Point, Vector, double, double, int>(v.Item1 + v.Item2 * maxOffset, v.Item2, v.Item3, v.Item4, v.Item5);
                }

                //Reduce the remaining offset by amount used up in this iteration
                offset -= maxOffset;

                //If remaining offset, make adjustments (remove 0 length segments etc) for next iteration
                if (offset > 0)
                {
                    bool recomputeLastSegment = false;

                    if (options.HandleGeneralCreatedSelfIntersections)
                    {
                        CheckIntersectionAndCullSegments(ref segments, ref vertices, cornerComputes, segmentsComputes, cornerIntersectionComputes, normal, isClosed, maxOffset, distTol);
                    }

                    for (int i = 0; i < segments.Count; i++)
                    {
                        var seg = segments[i];
                        double l = seg.Item1 + seg.Item4 * maxOffset;   //new length of segment
                        if (l < distTol)
                        {
                            //If smaller than tolerance, remove from segment lists
                            segments.RemoveAt(i);

                            //Set segments that are to be recomputed in terms of length reduction
                            bool addSegCompute = false;
                            if (segmentsComputes.Count > 0 && segmentsComputes[segmentsComputes.Count - 1] == i)
                                segmentsComputes[segmentsComputes.Count - 1] = i % segments.Count;  //Case that happeneds if multiple adjecent segments are removed
                            else
                                addSegCompute = true;

                            if (i == 0)
                            {
                                if (isClosed)
                                    recomputeLastSegment = true;
                            }
                            else
                                segmentsComputes.Add(i - 1);    //Previous segment also to be recomputed

                            if (addSegCompute)
                                segmentsComputes.Add(i % segments.Count);

                            //And from vertex lists
                            vertices.RemoveAt(i);

                            //Set corners that need to be recomputed in terms of translation vector as well as length reduction impact on segments
                            if (cornerComputes.Count > 0 && cornerComputes[cornerComputes.Count - 1] == i)
                                cornerComputes[cornerComputes.Count - 1] = i % vertices.Count;  //Case that happends if multiple adjecent segments are removed
                            else
                                cornerComputes.Add(i % vertices.Count);

                            i--;
                        }
                        else
                            segments[i] = new Tuple<double, Vector, Vector, double>(l, seg.Item2, seg.Item3, seg.Item4);
                    }

                    if (recomputeLastSegment)
                        segmentsComputes.Add(segments.Count - 1);

                    segmentsComputes = segmentsComputes.Distinct().ToList();
                    cornerComputes = cornerComputes.Distinct().ToList();
                }

                firstIteration = false;
            }

            cPts = vertices.Select(x => x.Item1).ToList();

            if (isClosed)
                cPts.Add(cPts[0]);

            if (cPts.Count < 2 || (isClosed && cPts.Count < 4))
            {
                Base.Compute.RecordError("Method failed to produce correct offset. Returning null.");
                return null;
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
        private static double ComputeMaxOffsetUntilIntersection(List<Tuple<double, Vector, Vector, double>> segments, List<Tuple<Point, Vector, double, double, int>> vertices, List<int> cornerIntersectionComputes, double offset, bool isClosed)
        {
            while (cornerIntersectionComputes.Count > 0)
            {
                int i = cornerIntersectionComputes[0];
                cornerIntersectionComputes.RemoveAt(0);

                if (!isClosed)  //Will never happen for end vertices of open curves
                {
                    if (i == 0 || i == vertices.Count - 1)
                        continue;
                }
                Tuple<Point, Vector, double, double, int> vertex = vertices[i];
                Vector trans = vertex.Item2;
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

                    Tuple<double, Vector, Vector, double> segment = segments[j];
                    Vector segOrtho = segment.Item3;
                    var startVertex = vertices[j];
                    Point segStart = startVertex.Item1;

                    double dot1 = segOrtho.DotProduct(segStart - vertex.Item1);
                    double dot2 = segOrtho.DotProduct(trans);
                    double reqOffset = dot1 / (dot2 - 1); //Required offset for the vertex to end up on the segment-plane after it has been offset

                    if (reqOffset > 0 && reqOffset < maxOffset)  //Offset in the correct direction and samller than previous max.
                    {
                        //Check if the point ends up on the finite segment
                        Point vOff = vertex.Item1 + reqOffset * trans;  //Point translated with offset
                        Point stOff = startVertex.Item1 + reqOffset * startVertex.Item2;
                        var enVertex = vertices[(j + 1) % vertices.Count];
                        Point enOff = enVertex.Item1 + reqOffset * enVertex.Item2;

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
                    vertices[i] = new Tuple<Point, Vector, double, double, int>(vertex.Item1, vertex.Item2, vertex.Item3, maxOffset, segmentIntersected);
                }
                else if (vertex.Item5 != -1)
                {
                    vertices[i] = new Tuple<Point, Vector, double, double, int>(vertex.Item1, vertex.Item2, vertex.Item3, double.MaxValue, -1);
                }
            }
            double overallMaxOffset = offset;
            foreach (var vertex in vertices)
            {
                if (vertex.Item5 != -1)
                    overallMaxOffset = Math.Min(vertex.Item4, overallMaxOffset);
            }
            return overallMaxOffset;
        }

        /***************************************************/

        private static void CheckIntersectionAndCullSegments(ref List<Tuple<double, Vector, Vector, double>> segments, ref List<Tuple<Point, Vector, double, double, int>> vertices, List<int> cornerComputes, List<int> segmentsComputes, List<int> cornerIntersectionComputes, Vector normal, bool isClosed, double maxOffset, double distTol)
        {
            if (isClosed)
            {
                List<Tuple<int, int>> ranges = new List<Tuple<int, int>>();
                for (int i = 1; i < vertices.Count - 1; i++)
                {
                    var v = vertices[i];
                    if (v.Item5 != -1)
                    {
                        double remOffset = vertices[i].Item4 - maxOffset;
                        if (remOffset < distTol)    //Intersection between vertex and another segment
                        {
                            int endIndex = v.Item5; //Index of segment intersected

                            ranges.Add(new Tuple<int, int>(i, endIndex));
                            ranges.Add(new Tuple<int, int>(endIndex + 1, i));
                        }
                        else
                            vertices[i] = new Tuple<Point, Vector, double, double, int>(v.Item1, v.Item2, v.Item3, remOffset, v.Item5);
                    }
                }
                if (ranges.Count > 0)
                {
                    List<List<int>> vertexLoops = new List<List<int>>();
                    if (ranges.Count == 2)
                    {
                        foreach (var range in ranges)
                        {
                            int index = range.Item1;
                            List<int> loop = new List<int>();
                            while (index != range.Item2 + 1)
                            {
                                loop.Add(index);
                                index++;
                                if (index >= vertices.Count)
                                    index = 0;
                            }
                            vertexLoops.Add(loop);
                        }
                    }
                    else
                    { }

                    List<int> largestLoop = LargestAreaLoop(vertexLoops, vertices, normal);

                    List<Tuple<Point, Vector, double, double, int>> verticesToKeep = new List<Tuple<Point, Vector, double, double, int>>();
                    List<Tuple<double, Vector, Vector, double>> segmentsToKeep = new List<Tuple<double, Vector, Vector, double>>();
                    for (int i = 0; i < largestLoop.Count; i++)
                    {
                        verticesToKeep.Add(vertices[largestLoop[i]]);
                        segmentsToKeep.Add(segments[largestLoop[i]]);
                    }
                    segments = segmentsToKeep;
                    vertices = verticesToKeep;
                }
            }
            else
            {
                for (int i = 1; i < vertices.Count - 1; i++)
                {
                    var v = vertices[i];
                    if (v.Item5 != -1)
                    {
                        double remOffset = vertices[i].Item4 - maxOffset;
                        if (remOffset < distTol)    //Intersection between vertex and another segment
                        {
                            int endIndex = v.Item5; //Index of segment intersected
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
                            var interSeg = segments[startRem + 1];
                            segments[startRem + 1] = new Tuple<double, Vector, Vector, double>(v.Item1.Distance(vertices[startRem + 1].Item1), interSeg.Item2, interSeg.Item3, 0);

                            cornerComputes.Add(startRem);
                            cornerIntersectionComputes.Add(startRem);
                            segmentsComputes.Add(startRem);



                        }
                        else
                            vertices[i] = new Tuple<Point, Vector, double, double, int>(v.Item1, v.Item2, v.Item3, remOffset, v.Item5);
                    }
                }
            }

            for (int j = 0; j < vertices.Count; j++)
            {
                if (vertices[j].Item5 != -1)
                    cornerIntersectionComputes.Add(j);
            }
        }

        /***************************************************/

        private static List<int> LargestAreaLoop(List<List<int>> loops, List<Tuple<Point, Vector, double, double, int>> vertices, Vector normal)
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

        private static double LoopArea(List<int> loop, List<Tuple<Point, Vector, double, double, int>> vertices, Vector normal)
        {
            if (loop.Count < 3)
                return 0;

            List<Point> pts = new List<Point>();
            for (int i = 0; i < loop.Count; i++)
            {
                pts.Add(vertices[loop[i]].Item1);
            }

            Point avg = pts.Average();
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
        private static bool HandleParalellAdjecentPolylinearSegments(List<Tuple<double, Vector, Vector, double>> segments, List<Tuple<Point, Vector, double, double, int>> vertices, List<int> cornerComputes, List<int> segmentsComputes, Vector normal, bool isClosed, bool firstIteration, double distTol, int i, int prev, double angleTol)
        {
            bool outwards = false;
            if (firstIteration) //Only need to check if "Inwards" or "Outwards" for first iteration, as subsequent iterations all can be assumed to be inwards (vanishing)
                outwards = CheckAcuteCornerOutwards(segments, isClosed,i, prev, angleTol, distTol);
            
            if (outwards)
            {
                //For the case of outwards, an additional vertex and segment is inserted, creating a right angled corner from the acute corner angle
                vertices.Insert(i, new Tuple<Point, Vector, double, double, int>(vertices[i].Item1, null, 0, double.MaxValue, -1));
                Vector tanNew = (segments[i].Item3 - segments[prev].Item3).Normalise();
                segments.Insert(i, new Tuple<double, Vector, Vector, double>(0, tanNew, tanNew.CrossProduct(normal).Normalise(), 0));

                //Readjust all corner computes, incrementing indecies to acount for items being inserted in the list
                for (int j = 0; j < cornerComputes.Count; j++)
                {
                    if (cornerComputes[j] > i)
                        cornerComputes[j]++;
                }

                for (int j = 0; j < segmentsComputes.Count; j++)
                {
                    if (segmentsComputes[j] > i - 1)
                        segmentsComputes[j]++;
                }

                //More segments need to be recomputed due to the insertion and change
                segmentsComputes.Add(prev);
                segmentsComputes.Add(i);
                segmentsComputes.Add((i + 1) % segments.Count);

                //Same for vertecies requiring re-computation
                cornerComputes.Insert(0, i + 1);
                cornerComputes.Insert(0, i);

            }
            else
            {
                //Else remove the vertex
                vertices.RemoveAt(i);
                int removed = 1;

                if (vertices.Count <= 1)
                    return false;

                if (prev > i)
                    prev--;

                if (vertices[prev].Item1.Distance(vertices[i % vertices.Count].Item1) < distTol)
                {
                    //If the corner removed leads to the previous and next points being adjecent, remove one of them as well as the two segments leading up to the removed vertex
                    vertices.RemoveAt(prev);
                    segments.RemoveAt(i);
                    segments.RemoveAt(prev);
                    removed++;
                }
                else
                {
                    //If not, remove one of the segments
                    segments.RemoveAt(i);

                    //And recompute the new direction for the other
                    Vector tan = vertices[i % vertices.Count].Item1 - vertices[prev].Item1;
                    double lengthNew = tan.Length();
                    tan = tan * (1 / lengthNew);
                    segments[prev] = new Tuple<double, Vector, Vector, double>(lengthNew, tan, tan.CrossProduct(normal).Normalise(), 0);
                }

                if (vertices.Count <= 1)
                    return false;   //Vertex list small enough that nothing is left. Method to return empty polyline.


                //Readjust corner compute indecies due to vertex removal 
                for (int j = 0; j < cornerComputes.Count; j++)
                {
                    if (cornerComputes[j] > i)
                        cornerComputes[j] -= removed;
                }
                //Set corners that require recomputation due to vertex removal
                int recomputeIndex = i - removed;
                if (recomputeIndex > 0) //Standard case for internal vertex
                {
                    cornerComputes.Add(recomputeIndex - 1);
                    cornerComputes.Add(recomputeIndex);
                    cornerComputes.Add((recomputeIndex + 1) % vertices.Count);
                }
                else if (recomputeIndex == 0)   //Recomputation around the start vertex
                {
                    if (isClosed)
                        cornerComputes.Add(vertices.Count - 1);
                    cornerComputes.Add(recomputeIndex);
                    cornerComputes.Add(recomputeIndex + 1);
                }
                else
                {
                    if (i == 0)
                    {
                        cornerComputes.Add(0);
                        cornerComputes.Add(1);
                        if (isClosed)
                            cornerComputes.Add(vertices.Count - 1);
                    }
                    else    //i == 1
                    {
                        cornerComputes.Add(0);
                        cornerComputes.Add(1);
                        if (vertices.Count > 1)
                            cornerComputes.Add(2);
                    }
                }

                for (int j = 0; j < segmentsComputes.Count; j++)
                {
                    if (segmentsComputes[j] > i - removed)
                        segmentsComputes[j] -= removed;
                }

            }

            cornerComputes.SortAndRemoveDuplicatesAndNegatives();
            segmentsComputes.SortAndRemoveDuplicatesAndNegatives();
            return true;
        }

        /***************************************************/

        [Description("Method that finds if a particular corner should be lead to an outwards of inwards offset.")]
        private static bool CheckAcuteCornerOutwards(List<Tuple<double, Vector, Vector, double>> segments, bool isClosed, int i, int prev, double angleTol, double distTol)
        {

            Vector t1 = segments[prev].Item2;

            //Checks whether the offset is "outwards" or "inwards" for the current corner
            int i0 = i - 2;

            if (isClosed && i0 < 0)
                i0 = segments.Count + i0;

            Vector t0 = null;   //Tangent vector of the segment coming into the corner before the current vertex being evaluated. 
            List<int> beforeSegs = new List<int>() { prev };

            if (i0 >= 0)
            {
                t0 = segments[i0].Item2;

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
                    t0 = segments[i0].Item2;
                }
            }


            int i3 = i + 1;
            if (isClosed && i3 > segments.Count - 1)
                i3 = i3 % segments.Count;

            Vector t3 = null;   //Vector
            List<int> afterSegs = new List<int>() { i };
            if (i3 <= segments.Count - 1)
            {
                t3 = segments[i3].Item2;

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

                    t3 = segments[i3].Item2;

                }
            }

            if (t0 != null && t3 != null)
            {
                double l1 = beforeSegs.Sum(x => segments[x].Item1);// segments[prev].Item1;
                double l2 = afterSegs.Sum(x => segments[x].Item1);// segments[i].Item1;
                double lengthDif = l1 - l2;

                //Dot products used for sign, if dotproduct is positive, the vectors are pointing in roungly the same direction
                double t0n1 = t0.DotProduct(segments[prev].Item3);
                double t3n1 = t3.DotProduct(segments[prev].Item3);

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
                return t3.DotProduct(segments[prev].Item3) < 0; 
            }
            else if (t0 != null)
            {
                //For case of no outgoing vector (t3) the corner is outwards if the tangent of the incoming vector is pointing in the oposite direction to the orthogonal vector coing into the corner
                return t0.DotProduct(segments[prev].Item3) < 0;
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



