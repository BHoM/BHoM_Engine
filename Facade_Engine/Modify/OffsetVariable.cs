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
using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Base.Debugging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Facade
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an offset of a curve. Works only on planar curves")]
        [Input("polyCurve", "Curve to offset")]
        [Input("offsets", "Offset distances (Length must match count of curves in the PolyCurve). Positive value offsets outside of a curve. If normal is given then offsets to the right with normal pointing up and direction of a curve pointing forward")]
        [Input("normal", "Normal of a plane for offset operation, not needed for closed curves")]
        [Output("curve", "Resulting offset")]
        public static PolyCurve OffsetVariable(this PolyCurve curve, List<double> offsets, Vector normal = null, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            //TODO:
            //Migrate to Geometry Engine and align with Offset method
            if (curve == null || curve.Length() < tolerance)
                return null;

            List<ICurve> subParts = curve.SubParts();
            if (subParts.Count != offsets.Count)
            {
                BH.Engine.Base.Compute.RecordError("PolyCurve segment count does not match amount of offset values provided.");
                return null;
            }

            //Check if contains any circles, if so, handle them explicitly, and offset any potential leftovers by backcalling this method
            if (subParts.Any(x => x is Circle))
            {
                curve.Offset(offsets[0]);
                return null;
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

            PolyCurve result = new PolyCurve();
            Vector normalNormalised = normal.Normalise();

            //First - offseting each individual element
            List<ICurve> offsetCurves = new List<ICurve>();
            for (int i = 0; i < subParts.Count; i++)
                if (subParts[i].IOffset(offsets[i], normal, false, tolerance) != null)
                    offsetCurves.Add(subParts[i].IOffset(offsets[i], normal, false, tolerance));

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

            Base.Compute.ClearCurrentEvents();

            if (connectingError)
                Base.Compute.RecordWarning("Couldn't connect offset subCurves properly.");

            if (offsetCurves.Count == 0)
            {
                Base.Compute.RecordError("Method failed to produce correct offset. Returning null.");
                return null;
            }

            List<PolyCurve> resultList = Geometry.Compute.IJoin(offsetCurves, tolerance);

            if (resultList.Count == 1)
                result = resultList[0];
            else
            {
                result.Curves = offsetCurves;
                Base.Compute.RecordWarning("Offset may be wrong. Please inspect the results.");
            }

            if (result.IsSelfIntersecting(tolerance) || result.CurveIntersections(curve, tolerance).Count != 0)
                Base.Compute.RecordWarning("Intersections occured. Please inspect the results.");

            if (isClosed && !result.IsClosed(tolerance))
                Base.Compute.RecordError("Final curve is not closed. Please inspect the results.");

            return result;
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

            List<PolyCurve> joinCurves = Geometry.Compute.IJoin(new List<ICurve> { curve1, curve2 }, tolerance).ToList();

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
                            Line tanLine1 = Geometry.Create.Line(curve1.IEndPoint(), curve1.IEndDir() / tolerance);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { curve1, tanLine1 } };

                            Line tanLine2 = Geometry.Create.Line(curve2.IEndPoint(), curve2.IEndDir() / tolerance);
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
                            Line tanLine1 = Geometry.Create.Line(curve1.IEndPoint(), curve1.IEndDir() / tolerance);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { curve1, tanLine1 } };

                            Line tanLine2 = Geometry.Create.Line(curve2.IStartPoint(), curve2.IStartDir().Reverse() / tolerance);
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
                            Line tanLine1 = Geometry.Create.Line(curve1.IStartPoint(), curve1.IStartDir().Reverse() / tolerance);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { tanLine1, curve1 } };

                            Line tanLine2 = Geometry.Create.Line(curve2.IEndPoint(), curve2.IEndDir() / tolerance);
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
                            Line tanLine1 = Geometry.Create.Line(curve1.IStartPoint(), curve1.IStartDir().Reverse() / tolerance);
                            tanLine1.Infinite = false;
                            PolyCurve pCurve1 = new PolyCurve { Curves = { tanLine1, curve1 } };

                            Line tanLine2 = Geometry.Create.Line(curve2.IStartPoint(), curve2.IStartDir().Reverse() / tolerance);
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






