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

        [Description("Creates an offset of a curve.")]
        [Input("curve", "Curve to offset.")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward.")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves.")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc.")]
        [Input("distTol", "Distance tolerance used for checking segment lengths equal to zero, point coincidence etc.", typeof(Length))]
        [Input("angleTol", "Angle tolerance used in the method.", typeof(Angle))]
        [Output("curve", "Resulting offset.")]
        public static Line Offset(this Line curve, double offset, Vector normal, bool tangentExtensions = false, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            return curve.Translate(normal.CrossProduct(curve.Start - curve.End).Normalise() * offset);
        }

        /***************************************************/

        [Description("Creates an offset of a curve.")]
        [Input("curve", "Curve to offset.")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward.")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves.")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc.")]
        [Input("distTol", "Distance tolerance used for checking segment lengths equal to zero, point coincidence etc.", typeof(Length))]
        [Input("angleTol", "Angle tolerance used in the method.", typeof(Angle))]
        [Output("curve", "Resulting offset.")]
        public static Arc Offset(this Arc curve, double offset, Vector normal, bool tangentExtensions = false, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
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

        [Description("Creates an offset of a curve.")]
        [Input("curve", "Curve to offset.")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward.")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves.")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc.")]
        [Input("distTol", "Distance tolerance used for checking segment lengths equal to zero, point coincidence etc.", typeof(Length))]
        [Input("angleTol", "Angle tolerance used in the method.", typeof(Angle))]
        [Output("curve", "Resulting offset.")]
        public static Circle Offset(this Circle curve, double offset, Vector normal = null, bool tangentExtensions = false, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
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

        [Description("Creates an offset of a curve. Works only on planar curves.")]
        [Input("curve", "Curve to offset.")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward.")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves.")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc.")]
        [Input("distTol", "Distance tolerance used for checking segment lengths equal to zero, point coincidence etc.", typeof(Length))]
        [Input("angleTol", "Angle tolerance used for checking if segments are parallel.")]
        [Output("curve", "Resulting offset.")]
        public static Polyline Offset(this Polyline curve, double offset, Vector normal = null, bool tangentExtensions = false, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            if (curve == null || curve.Length() < distTol)
                return null;

            if (offset == 0)
                return curve;

            OffsetOptions options = new OffsetOptions();
            List<Polyline> pLines = Compute.MultiOffset(curve, new List<double> { offset }, normal, options, true, distTol, angleTol);

            if (pLines.Count == 0)
            {
                BH.Engine.Base.Compute.RecordWarning("Polyline offset leads to curve vanishing into nothing. Empty polyline returned.");
                return new Polyline { ControlPoints = new List<Point>() };
            }
            return pLines[0];
        }

        /***************************************************/

        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("curve", "Curve to offset.")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward.")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves.")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc.")]
        [Input("distTol", "Distance tolerance used for checking segment lengths equal to zero, point coincidence etc.", typeof(Length))]
        [Input("angleTol", "Angle tolerance used in the method.", typeof(Angle))]
        [Output("curve", "Resulting offset.")]
        public static PolyCurve Offset(this PolyCurve curve, double offset, Vector normal = null, bool tangentExtensions = false, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {

            if (curve == null || curve.Length() < distTol)
                return null;


            //if there are only Line segmensts switching to polyline method which is more reliable 
            if (curve.Curves.All(x => x is Line))
            {
                Polyline polyline = ((Polyline)curve).Offset(offset, normal, tangentExtensions, distTol, angleTol);
                if (polyline == null)
                    return null;

                return new PolyCurve { Curves = polyline.SubParts().Cast<ICurve>().ToList() };
            }

            List<ICurve> subParts = curve.SubParts();
            //Check if contains any circles, if so, handle them explicitly, and offset any potential leftovers by backcalling this method
            if (subParts.Any(x => x is Circle))
            {
                IEnumerable<Circle> circles = subParts.Where(x => x is Circle).Cast<Circle>().Select(x => x.Offset(offset, normal, tangentExtensions, distTol, angleTol));
                PolyCurve nonCirclePolyCurve = new PolyCurve { Curves = curve.Curves.Where(x => !(x is Circle)).ToList() };
                if (nonCirclePolyCurve.Curves.Count != 0)
                    nonCirclePolyCurve = nonCirclePolyCurve.Offset(offset, normal, tangentExtensions, distTol, angleTol);

                nonCirclePolyCurve.Curves.AddRange(circles);
                return nonCirclePolyCurve;
            }

            if (!curve.IsPlanar(distTol))
            {
                BH.Engine.Base.Compute.RecordError("Offset works only on planar curves");
                return null;
            }

            if (curve.IsSelfIntersecting(distTol))
            {
                BH.Engine.Base.Compute.RecordError("Offset works only on non-self intersecting curves");
                return null;
            }

            if (offset == 0)
                return curve;

            bool isClosed = curve.IsClosed(distTol);
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
                return (curve.Offset(offset / 2, normal, tangentExtensions, distTol, angleTol))?.Offset(offset / 2, normal, tangentExtensions, distTol, angleTol);

            PolyCurve result = new PolyCurve();

            Vector normalNormalised = normal.Normalise();

            //First - offseting each individual element
            List<ICurve> offsetCurves = new List<ICurve>();
            foreach (ICurve crv in subParts)
            {
                ICurve partOffset = crv.IOffset(offset, normal, tangentExtensions, distTol, angleTol);
                if (partOffset != null)
                    offsetCurves.Add(partOffset);
            }

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
                Vector sTan = curve.TangentAtPoint(spOnCurve, distTol);
                Vector eTan = curve.TangentAtPoint(epOnCurve, distTol);
                Vector mTan = curve.TangentAtPoint(mpOnCurve, distTol);
                Vector sCheck = sp - spOnCurve;
                Vector eCheck = ep - epOnCurve;
                Vector mCheck = mp - mpOnCurve;
                Vector sCP = sTan.CrossProduct(sCheck).Normalise();
                Vector eCP = eTan.CrossProduct(eCheck).Normalise();
                Vector mCP = mTan.CrossProduct(mCheck).Normalise();
                if (offset > 0)
                {
                    if (sCP.IsEqual(normalNormalised, distTol) && eCP.IsEqual(normalNormalised, distTol) && mCP.IsEqual(normalNormalised, distTol))
                    {
                        offsetCurves.RemoveAt(i);
                        i--;
                        counter++;
                    }
                }
                else
                {
                    if (!sCP.IsEqual(normalNormalised, distTol) && !eCP.IsEqual(normalNormalised, distTol) && !mCP.IsEqual(normalNormalised, distTol))
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
                result.Curves.AddRange(polyline.Offset(offset, normal, tangentExtensions, distTol, angleTol).SubParts());
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

                PolyCurve temp = offsetCurves[i].Fillet(offsetCurves[j], tangentExtensions, true, false, distTol);
                if (temp == null) //trying to fillet with next curve 
                {
                    offsetCurves.RemoveAt(j);

                    if (j == 0)
                        i--;

                    if (j == offsetCurves.Count)
                        j = 0;
                    temp = offsetCurves[i].Fillet(offsetCurves[j], tangentExtensions, true, false, distTol);
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
                if ((offsetCurves[i].IPointAtParameter(0.5).Distance(curve) + distTol < Math.Abs(offset) &&
                     (offsetCurves[i].IStartPoint().Distance(curve) + distTol < Math.Abs(offset) ||
                      offsetCurves[i].IEndPoint().Distance(curve) + distTol < Math.Abs(offset))))
                {
                    PolyCurve temp = offsetCurves[((i - 1) + offsetCurves.Count) % offsetCurves.Count].Fillet(offsetCurves[(i + 1) % offsetCurves.Count], tangentExtensions, true, false, distTol);
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

            List<PolyCurve> resultList = Compute.IJoin(offsetCurves, distTol);

            if (resultList.Count == 1)
                result = resultList[0];
            else
            {
                result.Curves = offsetCurves;
                Base.Compute.RecordWarning("Offset may be wrong. Please inspect the results.");
            }

            if (counter > 0)
                Base.Compute.RecordWarning("Reduced " + counter + " line(s). Please inspect the results.");

            if (result.IsSelfIntersecting(distTol) || result.CurveIntersections(curve, distTol).Count != 0)
                Base.Compute.RecordWarning("Intersections occured. Please inspect the results.");

            if (isClosed && !result.IsClosed(distTol))
                Base.Compute.RecordError("Final curve is not closed. Please inspect the results.");

            return result;
        }


        /***************************************************/
        /***  Public methods - Interfaces                ***/
        /***************************************************/

        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("curve", "Curve to offset.")]
        [Input("offset", "Offset distance. Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward.")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves.")]
        [Input("tangentExtensions", "If true, arc segments of a PolyCurve will be extend by a tangent line, if false - by arc.")]
        [Input("distTol", "Distance tolerance used for checking segment lengths equal to zero, point coincidence etc.", typeof(Length))]
        [Input("angleTol", "Angle tolerance used in the method.", typeof(Angle))]
        [Output("curve", "Resulting offset.")]
        public static ICurve IOffset(this ICurve curve, double offset, Vector normal = null, bool tangentExtensions = false, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            return Offset(curve as dynamic, offset, normal, tangentExtensions, distTol, angleTol);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static ICurve Offset(this ICurve curve, double offset, Vector normal = null, bool tangentExtensions = false, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            Base.Compute.RecordError($"Offset is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }


        /***************************************************/
        /***  Private Methods                            ***/
        /***************************************************/

        private static List<ICurve> ExtendToPoint(this ICurve curve, Point startPoint, Point endPoint, bool tangentExtension, double distTol)
        {
            //TODO:
            //Decide if useful enough to make public. If so, rewrite and test.

            double start = startPoint.Distance(curve.IStartPoint());
            double end = endPoint.Distance(curve.IEndPoint());

            if (startPoint.IIsOnCurve(curve, distTol))
                start = -start;

            if (endPoint.IIsOnCurve(curve, distTol))
                end = -end;

            List<ICurve> result = new List<ICurve>();

            if (!tangentExtension)
            {
                if (curve is Arc)
                    result.Add((curve as Arc).Extend(2 * Math.Asin(start / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, 2 * Math.Asin(end / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, false, distTol));
                else
                    result.Add((curve as Line).Extend(start, end, false, distTol));
            }
            else
            {
                if (curve is Arc)
                {
                    if (start >= 0 && end >= 0)
                        result.Add((curve as Arc).Extend(start, end, true, distTol));
                    else if (start >= 0 && end < 0)
                        result.Add((curve as Arc).Extend(start, 2 * Math.Asin(end / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, true, distTol));
                    else if (start < 0 && end >= 0)
                        result.Add((curve as Arc).Extend(2 * Math.Asin(start / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, end, true, distTol));
                    else
                        result.Add((curve as Arc).Extend(2 * Math.Asin(start / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, 2 * Math.Asin(end / (2 * (curve as Arc).Radius)) * (curve as Arc).Radius, true, distTol));
                }
                else
                    result.Add((curve as Line).Extend(start, end, true, distTol));
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

        private static PolyCurve Fillet(this ICurve curve1, ICurve curve2, bool tangentExtensions, bool keepCurve1StartPoint, bool keepCurve2StartPoint, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            //TODO:
            //Write a proper fillet method, test and make it public            

            if (!((curve1 is Line || curve1 is Arc) && (curve2 is Line || curve2 is Arc))) //for now works only with combinations of lines and arcs
            {
                Base.Compute.RecordError("Private method fillet is implemented only for PolyCurves consisting of Lines or Arcs.");
                return null;
            }

            List<PolyCurve> joinCurves = Compute.IJoin(new List<ICurve> { curve1, curve2 }, distTol).ToList();

            if (joinCurves.Count == 1)
                return joinCurves[0];

            List<ICurve> resultCurves = new List<ICurve>();

            bool C1SP = keepCurve1StartPoint;
            bool C2SP = keepCurve2StartPoint;

            List<Point> intersections = curve1.ICurveIntersections(curve2, distTol);
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
                    resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, distTol));
                    resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, distTol));
                    resultCurves[1] = resultCurves[1].IFlip();
                }
                else if (C1SP && !C2SP)
                {
                    resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, distTol));
                    resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, distTol));
                }
                else if (!C1SP && C2SP)
                {
                    resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, distTol));
                    resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, distTol));
                    resultCurves[0] = resultCurves[0].IFlip();
                    resultCurves[1] = resultCurves[1].IFlip();
                }
                else
                {
                    resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, distTol));
                    resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, distTol));
                    resultCurves[0] = resultCurves[0].IFlip();
                }
            }
            else
            {
                if (curve1 is Line && curve2 is Line)
                {
                    Point intersection = (curve1 as Line).LineIntersection(curve2 as Line, true, distTol);
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
                        resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, distTol));
                        resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, distTol));
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
                        resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, distTol));
                        resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, distTol));
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
                        resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, distTol));
                        resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, distTol));
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
                        resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, distTol));
                        resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, distTol));
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

                        List<Point> curveIntersections = pCurve1.CurveIntersections(pCurve2, distTol);
                        if (curveIntersections.Count == 0)
                        {
                            Base.Compute.RecordError("Curves' extensions do not intersect");
                            return null;
                        }
                        if (C1SP && C2SP)
                        {
                            intersection = curveIntersections.ClosestPoint(curve1.IEndPoint());

                            resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, distTol));
                            resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, distTol));
                            resultCurves[1] = resultCurves[1].IFlip();
                        }
                        else if (C1SP && !C2SP)
                        {
                            intersection = curveIntersections.ClosestPoint(curve1.IEndPoint());
                            resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, distTol));
                            resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, distTol));
                        }
                        else if (!C1SP && C2SP)
                        {
                            intersection = curveIntersections.ClosestPoint(curve1.IStartPoint());
                            resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, distTol));
                            resultCurves.AddRange(curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, distTol));
                            resultCurves[0] = resultCurves[0].IFlip();
                            resultCurves[1] = resultCurves[1].IFlip();
                        }
                        else
                        {
                            intersection = curveIntersections.ClosestPoint(curve1.IStartPoint());
                            resultCurves.AddRange(curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, distTol));
                            resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, distTol));
                            resultCurves[0] = resultCurves[0].IFlip();
                        }
                    }
                    else
                    {
                        if (C1SP && C2SP)
                        {
                            Line tanLine1 = Create.Line(curve1.IEndPoint(), curve1.IEndDir() / distTol);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { curve1, tanLine1 } };

                            Line tanLine2 = Create.Line(curve2.IEndPoint(), curve2.IEndDir() / distTol);
                            tanLine2.Infinite = false;
                            PolyCurve pCurve2 = new PolyCurve { Curves = { curve2, tanLine2 } };

                            List<Point> curveIntersecions = pCurve1.CurveIntersections(pCurve2, distTol);
                            if (curveIntersecions.Count > 0)
                                intersection = curveIntersecions.ClosestPoint(curve1.IEndPoint());
                            else
                            {
                                Base.Compute.RecordWarning("Couldn't create fillet");
                                return null;
                            }

                            PolyCurve subResult1 = new PolyCurve
                            {
                                Curves = curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, distTol).ToList()
                            };

                            PolyCurve subResult2 = new PolyCurve
                            {
                                Curves = curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, distTol).ToList()
                            };

                            subResult2 = subResult2.Flip();

                            resultCurves.AddRange(subResult1.Curves);
                            resultCurves.AddRange(subResult2.Curves);
                        }
                        else if (C1SP && !C2SP)
                        {
                            Line tanLine1 = Create.Line(curve1.IEndPoint(), curve1.IEndDir() / distTol);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { curve1, tanLine1 } };

                            Line tanLine2 = Create.Line(curve2.IStartPoint(), curve2.IStartDir().Reverse() / distTol);
                            tanLine2.Infinite = false;
                            PolyCurve pCurve2 = new PolyCurve { Curves = { tanLine2, curve2 } };

                            List<Point> curveIntersecions = pCurve1.ICurveIntersections(pCurve2, distTol);

                            if (curveIntersecions.Count > 0)
                                intersection = curveIntersecions.ClosestPoint(curve1.IEndPoint());
                            else
                            {
                                Base.Compute.RecordWarning("Couldn't create fillet");
                                return null;
                            }

                            resultCurves.AddRange(curve1.ExtendToPoint(curve1.IStartPoint(), intersection, tangentExtensions, distTol));
                            resultCurves.AddRange(curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, distTol));
                        }
                        else if (!C1SP && C2SP)
                        {
                            Line tanLine1 = Create.Line(curve1.IStartPoint(), curve1.IStartDir().Reverse() / distTol);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { tanLine1, curve1 } };

                            Line tanLine2 = Create.Line(curve2.IEndPoint(), curve2.IEndDir() / distTol);
                            tanLine2.Infinite = false;
                            PolyCurve pCurve2 = new PolyCurve { Curves = { curve2, tanLine2 } };

                            List<Point> curveIntersecions = pCurve1.ICurveIntersections(pCurve2, distTol);
                            if (curveIntersecions.Count > 0)
                                intersection = curveIntersecions.ClosestPoint(curve1.IStartPoint());
                            else
                            {
                                Base.Compute.RecordWarning("Couldn't create fillet");
                                return null;
                            }
                            PolyCurve subResult1 = new PolyCurve
                            {
                                Curves = curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, distTol).ToList()
                            };
                            PolyCurve subResult2 = new PolyCurve
                            {
                                Curves = curve2.ExtendToPoint(curve2.IStartPoint(), intersection, tangentExtensions, distTol).ToList()
                            };

                            subResult1 = subResult1.Flip();
                            subResult2 = subResult2.Flip();

                            resultCurves.AddRange(subResult1.Curves);
                            resultCurves.AddRange(subResult2.Curves);
                        }
                        else
                        {
                            Line tanLine1 = Create.Line(curve1.IStartPoint(), curve1.IStartDir().Reverse() / distTol);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { tanLine1, curve1 } };

                            Line tanLine2 = Create.Line(curve2.IStartPoint(), curve2.IStartDir().Reverse() / distTol);
                            tanLine2.Infinite = false;
                            PolyCurve pCurve2 = new PolyCurve { Curves = { tanLine2, curve2 } };

                            List<Point> curveIntersecions = pCurve1.ICurveIntersections(pCurve2, distTol);
                            if (curveIntersecions.Count > 0)
                                intersection = curveIntersecions.ClosestPoint(curve1.IStartPoint());
                            else
                            {
                                Base.Compute.RecordWarning("Couldn't create fillet");
                                return null;
                            }
                            PolyCurve subResult1 = new PolyCurve
                            {
                                Curves = curve1.ExtendToPoint(intersection, curve1.IEndPoint(), tangentExtensions, distTol).ToList()
                            };
                            PolyCurve subResult2 = new PolyCurve
                            {
                                Curves = curve2.ExtendToPoint(intersection, curve2.IEndPoint(), tangentExtensions, distTol).ToList()
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




