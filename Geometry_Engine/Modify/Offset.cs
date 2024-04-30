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

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Creates an offset of a curve")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc")]
        [Output("curve", "Resulting offset")]
        public static Line Offset(this Line curve, double offset, Vector normal, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            return curve.Translate(normal.CrossProduct(curve.Start - curve.End).Normalise() * offset);
        }

        /***************************************************/

        [Description("Creates an offset of a curve")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc")]
        [Output("curve", "Resulting offset")]
        public static Arc Offset(this Arc curve, double offset, Vector normal, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
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

        [Description("Creates an offset of a curve")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc")]
        [Output("curve", "Resulting offset")]
        public static Circle Offset(this Circle curve, double offset, Vector normal = null, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
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

        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves")]
        [Output("curve", "Resulting offset")]
        public static Polyline Offset(this Polyline curve, double offset, Vector normal = null, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            if (curve == null || curve.Length() < tolerance)
                return null;

            if (!curve.IsPlanar(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Offset works only on planar curves");
                return null;
            }

            bool isClosed = curve.IsClosed(tolerance);

            if (normal == null)
                if (!isClosed)
                    BH.Engine.Base.Compute.RecordError("Normal is missing. Normal vector is not needed only for closed curves");
                else
                    normal = curve.Normal();
            else
                normal = normal.Normalise();

            if (offset == 0)
                return curve;

            if (offset > 0.05 * curve.Length())
                return (curve.Offset(offset / 2, normal, tangentExtensions, tolerance))?.Offset(offset / 2, normal, tangentExtensions, tolerance);

            List<Point> cPts = new List<Point>(curve.ControlPoints);
            List<Point> tmp = new List<Point>(curve.ControlPoints);

            if (!isClosed)
            {
                for (int i = 0; i < cPts.Count - 1; i++) //moving every vertex perpendicularly to each of it's edgses. At this point only direction of move is good.
                {
                    Vector trans = (cPts[i + 1] - cPts[i]).CrossProduct(normal);
                    trans = trans.Normalise() * offset;
                    tmp[i] += trans;
                    tmp[i + 1] += trans;
                }

                for (int i = 1; i < cPts.Count - 1; i++) //adjusting move distance
                {
                    Vector trans = (tmp[i] - cPts[i]);
                    trans = trans.Normalise() * Math.Abs(offset) / Math.Sin(Math.Abs((cPts[i] - cPts[i - 1]).Angle(cPts[i] - cPts[i + 1])) / 2);
                    tmp[i] = cPts[i] + trans;
                }

            }
            else
            {
                cPts.RemoveAt(cPts.Count - 1);
                tmp.RemoveAt(tmp.Count - 1);
                for (int i = 0; i < cPts.Count; i++) //moving every vertex perpendicularly to each of it's edgses. At this point only direction of move is good.
                {
                    Vector trans = (cPts[(i + 1) % cPts.Count] - cPts[i]).CrossProduct(normal);
                    trans = trans.Normalise() * offset;
                    tmp[i] += trans;
                    tmp[(i + 1) % cPts.Count] += trans;
                }

                for (int i = 0; i < cPts.Count; i++)  //adjusting move distance
                {
                    if (!(Math.Abs(Math.Sin((cPts[i] - cPts[(i + cPts.Count - 1) % cPts.Count]).Angle(cPts[i] - cPts[(i + 1) % cPts.Count]))) < Tolerance.Angle))
                    {
                        Vector trans = (tmp[i] - cPts[i]);
                        trans = trans.Normalise() * Math.Abs(offset) / Math.Sin(Math.Abs((cPts[i] - cPts[(i + cPts.Count - 1) % cPts.Count]).Angle(cPts[i] - cPts[(i + 1) % cPts.Count])) / 2);
                        tmp[i] = cPts[i] + trans;
                    }
                }
                tmp.Add(tmp[0]);
            }

            Polyline result = new Polyline { ControlPoints = tmp };
            List<Line> lines = result.SubParts();

            if (isClosed)
                tmp.RemoveAt(tmp.Count - 1);

            //removing erroneous curves
            int counter = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].PointAtParameter(0.5).Distance(curve) + tolerance < Math.Abs(offset) &&
                   (lines[i].Start.Distance(curve) + tolerance < Math.Abs(offset) ||
                    lines[i].End.Distance(curve) + tolerance < Math.Abs(offset)))
                {
                    Line line1 = new Line { Start = tmp[i], End = tmp[(i + tmp.Count - 1) % tmp.Count], Infinite = true };
                    Line line2 = new Line { Start = tmp[(i + 1) % tmp.Count], End = tmp[(i + 2) % tmp.Count], Infinite = true };

                    if (!(line1.LineIntersection(line2, false, tolerance) == null))
                        tmp[i] = line1.LineIntersection(line2, false, tolerance);

                    tmp.RemoveAt((i + 1) % tmp.Count);

                    if (tmp.Count == 0)
                    {
                        Base.Compute.RecordError("Method failed to produce correct offset. Returning null.");
                        return null;
                    }
                    tmp.Add(tmp[0]);
                    lines = result.SubParts();
                    tmp.RemoveAt(tmp.Count - 1);
                    i--;
                    counter++;
                }
            }

            if (isClosed)
                tmp.Add(tmp[0]);

            if (tmp.Count < 3 || (isClosed && tmp.Count < 4))
            {
                Base.Compute.RecordError("Method failed to produce correct offset. Returning null.");
                return null;
            }

            if (counter > 0)
                Base.Compute.RecordWarning("Reduced " + counter + " line(s). Offset may be wrong. Please inspect the results.");

            if (result.IsSelfIntersecting(tolerance) || result.LineIntersections(curve, tolerance).Count != 0)
                Base.Compute.RecordWarning("Intersections occured. Offset may be wrong. Please inspect the results.");

            return result;
        }

        /***************************************************/

        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("options", "Options for the offset operation. Default values is used if nothing is provided.")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves")]
        [Output("curve", "Resulting offset")]
        public static Polyline Offset2(this Polyline curve, double offset, Vector normal = null, OffsetOptions options = null, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
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

            //Construct list of tangent vectors and orthogonal vectors
            //List<double> lengths = new List<double>();
            //List<Vector> tans = new List<Vector>();
            //List<Vector> orthos = new List<Vector>();
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

                    segments.Add(new Tuple<double, Vector, Vector, double>(length, tan, normal.CrossProduct(tan).Normalise(), 0));
                    //lengths.Add(length);
                    //tans.Add(tan);
                    //orthos.Add(normal.CrossProduct(tan).Normalise());
                }
            }

            if (isClosed)
                cPts.RemoveAt(cPts.Count - 1);

            //Construct list of translation vectors for each vertex, each scaled to represent a offset by 1
            //List<Vector> trans = new Vector[cPts.Count].ToList();
            //List<double> lengthChange = new double[tans.Count].ToList();

            double sinTol = Math.Pow(Math.Sin(angleTol / 2), 2);

            List<int> cornerComputes = new List<int>(Enumerable.Range(0, cPts.Count));
            List<int> segmentsComputes = new List<int>(Enumerable.Range(0, segments.Count));

            bool firstIteration = true;

            //Loop and continue offseting until all offsets done.
            while (offset > 0)
            {

                //Compute corner offset vectors. First whileloop iteration computes for all corners
                //Subsequent iteration only updates corners that have seen a change
                while (cornerComputes.Count > 0)
                {
                    int i = cornerComputes[0];
                    cornerComputes.RemoveAt(0);

                    if (!isClosed && i == 0)
                        trans[i] = orthos[i];
                    else if (!isClosed && i == cPts.Count - 1)
                        trans[i] = orthos[i - 1];
                    else
                    {
                        int prev = i == 0 ? orthos.Count - 1 : i - 1;

                        Vector v1 = orthos[prev];
                        Vector v2 = orthos[i];
                        Vector dir = (v1 + v2) / 2;

                        //Below factor gives the correct magnitude for corner scaling based on equal angle triangles.
                        //ortho projected onto the direction gives a right angled triangle with the same angles as the triangle
                        //formed by offset vetor with correct length and dir.
                        double length = dir.SquareLength();

                        if (options.HandleAdjacentParallelSegments && length < sinTol)    //equivalent to check that angle between two adjecent segements is less than angle tolerance
                        {
                            bool outwards = false;
                            if (firstIteration) //Only relevant to check for outwards for first iteration. Subsequent iterations can never end up in this case
                            {
                                int i0 = i - 2;
                                if (isClosed && i0 < 0)
                                    i0 = orthos.Count + i0;

                                int i3 = i + 1;
                                if (isClosed && i3 > orthos.Count - 1)
                                    i3 = i3 % orthos.Count;

                                if (i0 >= 0 && i3 <= orthos.Count - 1)
                                {
                                    Vector t0 = tans[i0];
                                    Vector t1 = tans[prev];
                                    Vector t3 = tans[i3];

                                    double l1 = lengths[prev];
                                    double l2 = lengths[i];
                                    double lengthDif = l1 - l2;


                                    double t0n1 = t0.DotProduct(v1);
                                    double t3n1 = t3.DotProduct(v1);

                                    if (Math.Abs(lengthDif) < distTol)
                                    {
                                        if (t0n1 * t3n1 < 0)
                                        {
                                            //Same side
                                            if (t0n1 < 0)
                                                outwards = -t0.DotProduct(t1) > t3.DotProduct(t1);
                                            else
                                                outwards = -t0.DotProduct(t1) < t3.DotProduct(t1);
                                        }
                                        else
                                            outwards = t0n1 < 0;
                                    }
                                    else if (lengthDif < 0)
                                        outwards = t0n1 < 0;
                                    else
                                        outwards = t3n1 < 0;

                                }
                            }

                            if (outwards)
                            {
                                cPts.Insert(i, cPts[i]);
                                trans.Insert(i, null);

                                lengths.Insert(i + 1, 0);
                                lengthChange.Insert(i, 0);
                                orthos.Insert(i, tans[prev]);
                                tans.Insert(i, -v1);

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

                                segmentsComputes.Add(prev);
                                segmentsComputes.Add(i);
                                segmentsComputes.Add((i + 1) % tans.Count);
                                segmentsComputes = segmentsComputes.Distinct().ToList();

                                cornerComputes.Insert(0, i);
                                cornerComputes.Insert(0, i + 1);
                            }
                            else
                            {
                                cPts.RemoveAt(i);
                                trans.RemoveAt(i);
                                int removed = 1;

                                if (cPts[prev].Distance(cPts[i]) < distTol)
                                {
                                    cPts.RemoveAt(i);
                                    trans.RemoveAt(i);

                                    lengths.RemoveAt(i);
                                    lengthChange.RemoveAt(i);
                                    tans.RemoveAt(i);
                                    orthos.RemoveAt(i);

                                    lengths.RemoveAt(prev);
                                    lengthChange.RemoveAt(prev);
                                    tans.RemoveAt(prev);
                                    orthos.RemoveAt(prev);

                                    removed++;
                                }
                                else
                                {
                                    lengths.RemoveAt(i);
                                    lengthChange.RemoveAt(i);
                                    tans.RemoveAt(i);
                                    orthos.RemoveAt(i);
                                    Vector tan = cPts[i] - cPts[prev];
                                    double lengthNew = tan.Length();
                                    tans[prev] = tan * (1 / lengthNew);
                                    lengths[prev] = lengthNew;
                                    orthos[prev] = normal.CrossProduct(tan).Normalise();
                                }

                                for (int j = 0; j < cornerComputes.Count; j++)
                                {
                                    if (cornerComputes[j] > i)
                                        cornerComputes[j] -= removed;
                                }

                                cornerComputes.Insert(0, i);
                                cornerComputes.Insert(0, prev);
                                cornerComputes = cornerComputes.Distinct().ToList();

                                for (int j = 0; j < segmentsComputes.Count; j++)
                                {
                                    if (segmentsComputes[j] > i - 1)
                                        segmentsComputes[j] -= removed;
                                }
                                segmentsComputes = segmentsComputes.Distinct().ToList();
                            }

                        }
                        else
                        {
                            trans[i] = dir * (1 / length);
                        }
                    }
                }

                //Find maximum offset possible
                double maxOffset = offset;

                if (options.HandleCreatedSelfIntersections)
                {
                    //Compute how much the length of each segment is affected by a unit offset (length 1)
                    //First whileloop iteration runs though all segemnts, later loop iterations only recomputes for segments that have seen a change
                    while (segmentsComputes.Count > 0)
                    {
                        int i = segmentsComputes[0];
                        segmentsComputes.RemoveAt(0);
                        double change;
                        if (!isClosed && i == 0)
                            change = tans[i].DotProduct(trans[i + 1]);
                        else if (!isClosed && i == tans.Count - 1)
                            change = -tans[i].DotProduct(trans[i]);
                        else
                        {
                            change = -tans[i].DotProduct(trans[i]) + tans[i].DotProduct(trans[(i + 1) % (trans.Count - 1)]);
                        }

                        lengthChange[i] = change;
                    }


                    for (int i = 0; i < lengthChange.Count; i++)
                    {
                        double offsetScale = -lengths[i] / lengthChange[i];
                        if (offsetScale > 0)
                            maxOffset = Math.Min(maxOffset, offsetScale);
                    }
                }

                //Offset the points
                for (int i = 0; i < cPts.Count; i++)
                {
                    cPts[i] += trans[i] * maxOffset;
                }

                offset -= maxOffset;

                //If remaining offset, make adjustments (remove 0 length segments etc) for next iteration
                if (offset > 0)
                {
                    firstIteration = false;

                    bool recomputeLastSegment = false;

                    for (int i = 0; i < lengths.Count; i++)
                    {
                        double l = lengths[i] + lengthChange[i] * maxOffset;   //new length of segment
                        if (l < distTol)
                        {
                            //If smaller than tolerance, remove from segment lists
                            lengths.RemoveAt(i);
                            lengthChange.RemoveAt(i);
                            tans.RemoveAt(i);
                            orthos.RemoveAt(i);
                            if (i == 0)
                            {
                                if (isClosed)
                                    recomputeLastSegment = true;
                            }
                            else
                                segmentsComputes.Add(i - 1);

                            segmentsComputes.Add(i % lengths.Count);

                            //And from vertex lists
                            cPts.RemoveAt(i);
                            trans.RemoveAt(i);
                            cornerComputes.Add(i % cPts.Count);
                            i--;
                        }
                        else
                            lengths[i] = l;
                    }

                    if (recomputeLastSegment)
                        segmentsComputes.Add(lengths.Count - 1);
                }
            }


            if (isClosed)
                cPts.Add(cPts[0]);

            if (cPts.Count < 3 || (isClosed && cPts.Count < 4))
            {
                Base.Compute.RecordError("Method failed to produce correct offset. Returning null.");
                return null;
            }

            return new Polyline() { ControlPoints = cPts };
        }

        /***************************************************/

        /***************************************************/

        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves")]
        [Output("curve", "Resulting offset")]
        public static PolyCurve Offset(this PolyCurve curve, double offset, Vector normal = null, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {

            if (curve == null || curve.Length() < tolerance)
                return null;

            //if there are only Line segmensts switching to polyline method which is more reliable 
            if (curve.Curves.All(x => x is Line))
            {
                Polyline polyline = ((Polyline)curve).Offset(offset, normal, tangentExtensions, tolerance);
                if (polyline == null)
                    return null;

                return new PolyCurve { Curves = polyline.SubParts().Cast<ICurve>().ToList() };
            }

            List<ICurve> subParts = curve.SubParts();
            //Check if contains any circles, if so, handle them explicitly, and offset any potential leftovers by backcalling this method
            if (subParts.Any(x => x is Circle))
            {
                IEnumerable<Circle> circles = subParts.Where(x => x is Circle).Cast<Circle>().Select(x => x.Offset(offset, normal, tangentExtensions, tolerance));
                PolyCurve nonCirclePolyCurve = new PolyCurve { Curves = curve.Curves.Where(x => !(x is Circle)).ToList() };
                if (nonCirclePolyCurve.Curves.Count != 0)
                    nonCirclePolyCurve = nonCirclePolyCurve.Offset(offset, normal, tangentExtensions, tolerance);

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
                return (curve.Offset(offset / 2, normal, tangentExtensions, tolerance))?.Offset(offset / 2, normal, tangentExtensions, tolerance);

            PolyCurve result = new PolyCurve();

            Vector normalNormalised = normal.Normalise();

            //First - offseting each individual element
            List<ICurve> offsetCurves = new List<ICurve>();
            foreach (ICurve crv in subParts)
                if (crv.IOffset(offset, normal, false, tolerance) != null)
                    offsetCurves.Add(crv.IOffset(offset, normal, false, tolerance));

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
                result.Curves.AddRange(polyline.Offset(offset, normal, tangentExtensions, tolerance).SubParts());
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

        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("curve", "Curve to offset")]
        [Input("offset", "Offset distance. Positive value offsets outside of a closed curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc")]
        [Output("curve", "Resulting offset")]
        public static ICurve IOffset(this ICurve curve, double offset, Vector normal = null, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            return Offset(curve as dynamic, offset, normal, tangentExtensions, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static ICurve Offset(this ICurve curve, double offset, Vector normal = null, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"Offset is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }


        /***************************************************/
        /***  Private Methods                            ***/
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



