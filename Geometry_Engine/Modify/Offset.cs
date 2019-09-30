/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Offset(this Arc curve, double offset, Vector normal)
        {
            if (!curve.IsPlanar())
            {
                BH.Engine.Reflection.Compute.RecordError("Offset works only on planar curves");
                return null;
            }

            if (offset == 0)
                return curve.Clone();

            double radius = curve.Radius;
            Arc result = curve.Clone();

            if (normal.DotProduct(curve.Normal()) > 0)
                radius += offset;
            else
                radius -= offset;

            if (radius > 0)
            {
                result.Radius = radius;
                return result;
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordError("Offset value is greater than arc radius");
                return null;
            }
        }

        /***************************************************/

        public static Circle Offset(this Circle curve, double offset, Vector normal)
        {
            if (offset == 0)
                return curve.Clone();

            double radius = curve.Radius;
            Circle result = curve.Clone();

            if (normal.DotProduct(curve.Normal()) > 0)
                radius += offset;
            else
                radius -= offset;

            if (radius > 0)
            {
                result.Radius = radius;
                return result;
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordError("Offset value is greater than circle radius");
                return null;
            }

        }

        /***************************************************/

        public static Polyline Offset(this Polyline curve, double offset, Vector normal = null, double tolerance = Tolerance.Distance)
        {
            if (!curve.IsPlanar())
            {
                BH.Engine.Reflection.Compute.RecordError("Offset works only on planar curves");
                return null;
            }
            Boolean isClosed = curve.IsClosed(tolerance);
            if (normal == null)
                if (!isClosed)
                    BH.Engine.Reflection.Compute.RecordError("Normal is missing. Normal vector is not needed only for closed curves");
                else
                    normal = curve.Normal();

            if (offset == 0)
                return curve.Clone();

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
                //return new Polyline { ControlPoints = tmp };
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

                for (int i = 0; i < cPts.Count; i++)  // adjusting move distance
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
            List<Line> Lines = result.SubParts();
            if (isClosed)
                tmp.RemoveAt(tmp.Count - 1);
            int counter = 0;
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].PointAtParameter(0.5).Distance(curve) + tolerance < Math.Abs(offset) &&
                   (Lines[i].Start.Distance(curve) + tolerance < Math.Abs(offset) ||
                    Lines[i].End.Distance(curve) + tolerance < Math.Abs(offset)))
                {
                    Line line1 = new Line { Start = tmp[i], End = tmp[(i + tmp.Count - 1) % tmp.Count], Infinite = true };
                    Line line2 = new Line { Start = tmp[(i + 1) % tmp.Count], End = tmp[(i + 2) % tmp.Count], Infinite = true };
                    if(!(line1.LineIntersection(line2) == null))
                        tmp[i] = line1.LineIntersection(line2);
                    tmp.RemoveAt((i + 1) % tmp.Count);
                    if(tmp.Count == 0)
                    {
                        Reflection.Compute.RecordError("Method failed to produce corretct offset. Null has been returned.");
                        return null;
                    }
                    tmp.Add(tmp[0]);
                    Lines = result.SubParts();
                    tmp.RemoveAt(tmp.Count - 1);
                    i--;
                    counter++;
                }
            }
            if (isClosed)
                tmp.Add(tmp[0]);
            if (tmp.Count < 3 || (isClosed && tmp.Count < 4))
            {
                Reflection.Compute.RecordError("Method failed to produce corretct offset. Null has been returned.");
                return null;
            }
            if (counter > 0)
                Reflection.Compute.RecordWarning("Reduced " + counter as string + " lines. Offset may be wrong. Please inspect the results.");
            if (result.IsSelfIntersecting(tolerance) || result.LineIntersections(curve).Count != 0)
                Reflection.Compute.RecordWarning("Intersections occured. Offset may be wrong. Please inspect the results.");
            return result;
        }
    

        /***************************************************/

        public static PolyCurve Offset(this PolyCurve curve, double offset, Vector normal = null, bool extend = false, double tolerance = Tolerance.Distance)
        {
            if (!curve.IsPlanar(tolerance))
            {
                BH.Engine.Reflection.Compute.RecordError("Offset works only on planar curves");
                return null;
            }

            if (curve.IsSelfIntersecting())
            {
                BH.Engine.Reflection.Compute.RecordError("Offset works only on non-self intersecting curves");
                return null;
            }

            if (offset == 0)
                return curve.Clone();

            List<ICurve> resultList = new List<ICurve>();
            List<ICurve> ICrvs = new List<ICurve>(curve.Curves);
            PolyCurve result = new PolyCurve();

            if (ICrvs[0] is Circle)
            {
                result.Curves.Add(Offset((Circle)ICrvs[0], offset, normal));
                return result;
            }

            //First - offseting each individual element

            List<ICurve> crvs = new List<ICurve>();
            foreach (ICurve crv in curve.Curves)
            {
                if (crv is Arc)
                {
                    Arc arc = new Arc();
                    arc = ((Arc)crv).Clone();
                    crvs.Add(arc.Offset(offset, normal));
                }
                else
                {
                    Line ln = new Line();
                    ln = ((Line)crv).Clone();
                    Vector mv = new Vector();
                    mv = ln.TangentAtPoint(ln.Start).CrossProduct(normal);
                    double mvScale = 0;
                    mvScale = offset / mv.Length();
                    mv *= mvScale;
                    ln.Start = ln.Start + mv;
                    ln.End += mv;
                    crvs.Add(ln);
                }
            }

            // Looking for an index of a longest curve - most offsets maintain this longest curve all the time
            int lngstIndex = 0;
            for (int i = 1; i < crvs.Count; i++)
            {
                if (crvs[i].ILength() > crvs[lngstIndex].ILength())
                {
                    lngstIndex = i;
                }
            }

            // Searching for intersections - if 2 curves intersect, trim them to the point of intersection
            Line tmpLn = null;
            bool omitWarning = false;
            // intersecting lines
            for (int i = 0; i < crvs.Count - 1; i++)
            {
                if (crvs[i] is Line && crvs[(i + 1) % crvs.Count] is Line)
                {
                    tmpLn = Create.Line(crvs[i].IStartPoint(), -crvs[i].IStartDir() / crvs[i].IStartDir().Length() * Math.Abs(offset * offset * 5));
                    tmpLn.Infinite = false;
                    if (!((Line)crvs[i]).LineIntersection((Line)crvs[(i + 1) % crvs.Count], true).IsOnCurve(tmpLn))
                    {
                        crvs[i]=crvs[i].IExtend(crvs[i].IStartPoint(), ((Line)crvs[i]).LineIntersection((Line)crvs[(i + 1) % crvs.Count], true));
                        crvs[i + 1] = crvs[i + 1].IExtend(crvs[i].IEndPoint(), crvs[i + 1].IEndPoint());
                    }
                    else
                    {
                        crvs[i] = crvs[i].IExtend(((Line)crvs[i]).LineIntersection((Line)crvs[(i + 1) % crvs.Count], true), crvs[i].IEndPoint());
                        crvs[i + 1] = crvs[i + 1].IExtend(crvs[i + 1].IStartPoint(), ((Line)crvs[i]).LineIntersection((Line)crvs[(i + 1) % crvs.Count], true));
                        crvs.Reverse(i, 2);
                        omitWarning = true;
                    }
                }
            }

            List<Point> Pts = new List<Point>();
            List<Point[]> intersections = new List<Point[]>();

            for (int i = 0; i < crvs.Count; i++)
            {
                intersections.Add(new Point[2]);
                intersections[i][0] = crvs[i].IStartPoint();
                intersections[i][1] = crvs[i].IEndPoint();
            }
            //intersecting arcs with arcs and lines

            for (int i = lngstIndex; i <lngstIndex+ crvs.Count; i++)
            {
                for (int j = i + 1; j < i + Math.Min(crvs.Count / 2+2, crvs.Count-1); j++)
                {

                    if ((Pts = crvs[i % crvs.Count].ICurveIntersections(crvs[j % crvs.Count])).Count > 0)
                    {
                        if (Pts.Count == 2)
                        {
                            if (Pts[0].Distance(crvs[i % crvs.Count].IStartPoint()) < Pts[1].Distance(crvs[i % crvs.Count].IStartPoint()))
                            {
                                intersections[i%crvs.Count][0] = Pts[0];
                                intersections[i % crvs.Count][1] = Pts[1];
                                intersections[j % crvs.Count][0] = Pts[1];
                                intersections[j % crvs.Count][1] = Pts[0];
                            }
                            else
                            {
                                intersections[i % crvs.Count][0] = Pts[1];
                                intersections[i % crvs.Count][1] = Pts[0];
                                intersections[j % crvs.Count][0] = Pts[0];
                                intersections[j % crvs.Count][1] = Pts[1];
                            }

                            if (curve.IsClosed())
                            {
                                //this will happen only in making a curve smaller - if 2 curves intersect creating a closed Pcurve, we can return it
                                resultList.Add(crvs[i % crvs.Count].ITrim(intersections[i % crvs.Count][0], intersections[i % crvs.Count][1]));
                                resultList.Add(crvs[j % crvs.Count].ITrim(intersections[j % crvs.Count][0], intersections[j % crvs.Count][1]));
                                return new PolyCurve { Curves = resultList };
                            }
                            break;
                        }
                        else
                        {
                            if (i % crvs.Count == lngstIndex)
                            {
                                if (intersections[i % crvs.Count][0].Distance(Pts[0]) > intersections[i % crvs.Count][1].Distance(Pts[0])&&intersections[i % crvs.Count][0].Distance(crvs[i % crvs.Count].IStartPoint())==0)
                                    intersections[i % crvs.Count][1] = Pts[0];
                                else
                                    intersections[i % crvs.Count][0] = Pts[0];

                                if (intersections[j % crvs.Count][1].Distance(Pts[0]) > 0)
                                    intersections[j % crvs.Count][0] = Pts[0];
                            }
                            else
                            {
                                if (intersections[i % crvs.Count][0].Distance(Pts[0]) > 0)
                                    intersections[i % crvs.Count][1] = Pts[0];

                                if (intersections[j % crvs.Count][1].Distance(Pts[0]) > 0)
                                    intersections[j % crvs.Count][0] = Pts[0];
                            }
                        }

                        if (j % crvs.Count == lngstIndex)
                            break;
                    }
                }

            }

            for (int i = 0; i < crvs.Count; i++)
            {
                if (intersections[i][0].Distance(crvs[i].IStartPoint()) < intersections[i][1].Distance(crvs[i].IStartPoint()))
                    crvs[i] = crvs[i].ITrim(intersections[i][0], intersections[i][1]);
            }

            result.Curves = crvs;
            //if this operation was enough to create a curve matching the last one - only appropriate on closed curves - return the result
            if (result.IsClosed()&&curve.IsClosed())
                return result;

            resultList.Add(crvs[lngstIndex]);
            Point lastPoint = resultList[resultList.Count - 1].IEndPoint();
            bool changed = false;
            List<ICurve> tmp = new List<ICurve>();
            List<Point> tmpPts = new List<Point>();

            //if the original curve is open, it is better to start offseting from the beginning, rather than from longest curve
            if(!curve.IsClosed())
            {
                resultList.Clear();
                lngstIndex = 0;
                resultList.Add(crvs[0]);
            }

            int lastIndexUsed = lngstIndex;
            bool flip = false;

            //this loop finds us the proper offset - trims/extends the curves to create one joint polycurve
            for (int i = lngstIndex + 1; i < lngstIndex + crvs.Count; i++)
            {
                changed = false;
                flip = false;
                Line ln1 = Create.Line(resultList[resultList.Count - 1].IEndPoint(), resultList[resultList.Count - 1].IEndDir() / resultList[resultList.Count - 1].IEndDir().Length() * Math.Max(Math.Abs(offset * offset * 3), 5));
                ln1.Infinite = false;
                Line ln2 = Create.Line(crvs[i % crvs.Count].IStartPoint(), -crvs[i % crvs.Count].IStartDir() / crvs[i % crvs.Count].IStartDir().Length() * Math.Max(Math.Abs(offset * offset * 3), 5));
                ln2.Infinite = false;

                if (crvs[i % crvs.Count] is Line && resultList[resultList.Count - 1] is Arc)
                {
                    if (((Arc)resultList[resultList.Count - 1]).Centre().Distance((Line)crvs[i % crvs.Count]) < ((Arc)resultList[resultList.Count - 1]).Radius)
                    {
                        flip = true;
                        ln2.Infinite = true;
                    }
                }

                Pts.Clear();

                if ((tmpPts = crvs[i % crvs.Count].ICurveIntersections(resultList[resultList.Count - 1])).Count > 0)
                {
                    foreach (Point point in tmpPts)
                        Pts.Add(point);
                    changed = true;
                }
                else
                {
                    if ((tmpPts = ln1.ICurveIntersections(crvs[i % crvs.Count])).Count > 0)
                    {
                        foreach (Point point in tmpPts)
                            Pts.Add(point);
                        changed = true;
                    }

                    if ((tmpPts = ln1.ICurveIntersections(ln2)).Count > 0)
                    {
                        foreach (Point point in tmpPts)
                            Pts.Add(point);
                        changed = true;
                    }

                    if ((tmpPts = ln2.ICurveIntersections(resultList[resultList.Count - 1])).Count > 0)
                    {
                        foreach (Point point in tmpPts)
                            Pts.Add(point);
                        changed = true;
                    }
                }

                if (changed)
                {
                    if (Pts.Count >= 2)
                    {
                        for (int a = 0; a < Pts.Count; a++)
                        {
                            if ((Pts[a].IDistance(crvs[i % crvs.Count]) + Pts[a].IDistance(resultList[resultList.Count - 1]) <= Pts[0].IDistance(crvs[i % crvs.Count]) + Pts[0].IDistance(resultList[resultList.Count - 1])))
                                Pts[0] = Pts[a];
                        }
                    }

                    int oldResNmbr = resultList.Count - 1;
                    tmp = resultList[resultList.Count - 1].TrimExtend(resultList[resultList.Count - 1].IStartPoint(), Pts[0], extend);
                    resultList.RemoveAt(resultList.Count - 1);
                    foreach (ICurve x in tmp)
                    {
                        resultList.Add(x);
                    }
                    tmp = crvs[i % crvs.Count].TrimExtend(Pts[0], crvs[i % crvs.Count].IEndPoint(), extend);

                    foreach (ICurve x in tmp)
                    {
                        resultList.Add(x);
                    }

                    if (flip)
                    {
                        if (resultList[resultList.Count - 1].IStartPoint().Distance(resultList[oldResNmbr].IEndPoint()) > resultList[resultList.Count - 1].IEndPoint().Distance(resultList[oldResNmbr].IEndPoint()))
                            resultList[resultList.Count - 1] = resultList[resultList.Count - 1].IFlip();
                    }

                    lastPoint = resultList[resultList.Count - 1].IEndPoint();
                    lastIndexUsed = i;
                }
                else
                    omitWarning = true;
            }

            result.Curves = resultList;
            //here, we try to close the curve and get rid of "unnecessary" curves - creating self-intersections
            if (!result.IsClosed() && curve.IsClosed())
            {
                if ((lastIndexUsed + 1) % crvs.Count == lngstIndex)
                {
                    Line ln1 = null;
                    Line ln2 = null;
                    ln1 = Create.Line(resultList[resultList.Count - 1].IEndPoint(), resultList[resultList.Count - 1].IEndDir() / resultList[resultList.Count - 1].IEndDir().Length() * Math.Abs(offset * offset * 5));
                    ln2 = Create.Line(resultList[0].IStartPoint(), -resultList[0].IStartDir() / resultList[0].IStartDir().Length() * Math.Abs(offset * offset * 5));
                    Pts.Clear();

                    if ((tmpPts = ln1.ICurveIntersections(resultList[0])).Count > 0)
                    {
                        foreach (Point point in tmpPts)
                            Pts.Add(point);
                    }

                    if ((tmpPts = ln1.ICurveIntersections(ln2)).Count > 0)
                    {
                        foreach (Point point in tmpPts)
                            Pts.Add(point);
                    }

                    if ((tmpPts = ln2.ICurveIntersections(resultList[resultList.Count - 1])).Count > 0)
                    {
                        foreach (Point point in tmpPts)
                            Pts.Add(point);
                    }

                    if (Pts.Count >= 2)
                    {
                        for (int a = 0; a < Pts.Count; a++)
                        {
                            if ((Pts[a].IDistance(resultList[0]) <= Pts[0].IDistance(resultList[0]) || Pts[a].IDistance(resultList[resultList.Count - 1]) <= Pts[0].IDistance(resultList[resultList.Count - 1])))
                                Pts[0] = Pts[a];
                        }
                    }

                    tmp = resultList[resultList.Count - 1].TrimExtend(resultList[resultList.Count - 1].IStartPoint(), Pts[0], extend);
                    resultList.RemoveAt(resultList.Count - 1);
                    foreach (ICurve x in tmp)
                    {
                        resultList.Add(x);
                    }

                    tmp = resultList[0].TrimExtend(Pts[0], resultList[0].IEndPoint(), extend);
                    if (tmp.Count == 1)
                        resultList[0] = tmp[0];
                    else
                    {
                        resultList.Add(tmp[0]);
                        resultList[0] = tmp[1];
                    }
                }

                else
                {
                    Line ln2 = Create.Line(resultList[0].IStartPoint(), -resultList[0].IStartDir() / resultList[0].IStartDir().Length() * Math.Abs(offset * offset * 3));
                    ln2.Infinite = false;
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        if ((Pts = ln2.ICurveIntersections(resultList[i])).Count > 0)
                        {
                            resultList[i] = resultList[i].ITrim(resultList[i].IStartPoint(), Pts[0]);
                            for (int j = i + 1; j < resultList.Count; j++)
                                resultList.RemoveAt(j);
                            resultList.Add(new Line { Start = Pts[0], End = resultList[0].IStartPoint() });
                            omitWarning = true;
                            break;
                        }
                    }
                }

            }


            if (!result.IsClosed(tolerance) && curve.IsClosed(tolerance))
            {
                BH.Engine.Reflection.Compute.RecordError("Offset is incorrect.");
                return null;
            }

            if (omitWarning||result.CurveIntersections(curve).Count > 0|| result.IsSelfIntersecting())
            {
                BH.Engine.Reflection.Compute.RecordWarning("Offset may be wrong. Please inspect the results.");
            }

            return result;
        }
    }
}