/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Distance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/

        public static double SquareDistance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        public static double Distance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/

        public static double SquareDistance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        public static double Distance(this Point point, Plane plane)
        {
            Vector normal = plane.Normal.Normalise();
            return Math.Abs(normal.DotProduct(point - plane.Origin));
        }


        /***************************************************/
        /****       Public Methods - Point/Curve        ****/
        /***************************************************/

        public static double Distance(this Point point, Line line, bool infiniteSegment = false)
        {
            return point.Distance(line.ClosestPoint(point, infiniteSegment));
        }

        /***************************************************/

        public static double SquareDistance(this Point point, Line line, bool infiniteSegment = false)
        {
            return point.SquareDistance(line.ClosestPoint(point, infiniteSegment));
        }

        /***************************************************/

        public static double Distance(this Point point, Arc arc)
        {
            return point.Distance(arc.ClosestPoint(point));
        }

        /***************************************************/

        public static double Distance(this Point point, Circle circle)
        {
            return point.Distance(circle.ClosestPoint(point));
        }

        /***************************************************/

        [NotImplemented]
        public static double Distance(this Point point, Ellipse ellipse)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double Distance(this Point point, Polyline curve)
        {
            return point.Distance(curve.ClosestPoint(point));
        }

        /***************************************************/

        public static double Distance(this Point point, PolyCurve curve)
        {
            return point.Distance(curve.ClosestPoint(point));
        }

        /***************************************************/

        [NotImplemented]
        public static double Distance(this Point point, NurbsCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
        /****       Public Methods - Curve/Curve        ****/
        /***************************************************/

        public static double Distance(this Line curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return 0f;
            double distance1 = Math.Sqrt(Math.Min(curve2.End.SquareDistance(curve1), curve2.Start.SquareDistance(curve1)));
            double distance2 = Math.Sqrt(Math.Min(curve1.End.SquareDistance(curve2), curve1.Start.SquareDistance(curve2)));
            double min = Math.Min(distance1, distance2);
            if (curve1.IsCoplanar(curve2))
            {
                return min;
            }

            double[] t = curve1.SkewLineProximity(curve2);
            double t1 = Math.Max(Math.Min(t[0], 1), 0);
            double t2 = Math.Max(Math.Min(t[1], 1), 0);
            Vector e1 = curve1.End - curve1.Start;
            Vector e2 = curve2.End - curve2.Start;
            return Math.Min((curve1.Start + e1 * t1).Distance(curve2.Start + e2 * t2),min);
        }

        /***************************************************/

        public static double Distance(this Line curve1, Arc curve2,double tolerance=Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return 0f;
            double distance1 = Math.Min(curve2.EndPoint().Distance(curve1), curve2.StartPoint().Distance(curve1));
            double distance2 = Math.Min(curve1.End.Distance(curve2), curve1.Start.Distance(curve2));
            double smallestEnd = Math.Min(distance1, distance2);
            if (curve1.IIsCoplanar(curve2))
                return smallestEnd;
            Point start, end, binSearch;
            if(distance1<distance2)
            {
                Arc temp = curve2;
                start = curve2.StartPoint();
                end = curve2.EndPoint();
                while((start-end).Length()>tolerance*tolerance)
                {
                    binSearch = temp.PointAtParameter(0.5);
                    if (end.Distance(curve1) > start.Distance(curve1))
                        end = binSearch;
                    else
                        start = binSearch;
                    temp = temp.Trim(start, end);
                }
                distance1 = Math.Min(start.Distance(curve1), end.Distance(curve1));
            }
            else
            {
                start = curve1.Start;
                end = curve1.End;
                while ((start - end).Length() > tolerance * tolerance)
                {
                    binSearch = start + ((end - start) / 2);
                    if (start.Distance(curve2) > end.Distance(curve2))
                        start = binSearch;
                    else
                        end = binSearch;
                }
                distance1 = Math.Min(start.Distance(curve2), end.Distance(curve2));
            }
            return Math.Min(distance1, smallestEnd);
        }

        /***************************************************/

        public static double Distance(this Line curve1, Circle  curve2, double tolerance=Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return 0f;
            List<Point> crclpts = new List<Point>();
            for (double i = 0; i < 1; i += 0.25)
                crclpts.Add(curve2.PointAtParameter(i));
            crclpts.Add(curve1.Start);
            crclpts.Add(curve1.End);
            if (crclpts.IsCoplanar())
            {
                Line temp = Create.Line(curve2.Centre, curve1.ClosestPoint(curve2.Centre));
                if (temp.Length() < curve2.Radius)
                    return Math.Min(curve1.End.Distance(curve2), curve1.Start.Distance(curve2));
                return curve2.Centre.Distance(curve1) - curve2.Radius;
            }
            crclpts.Remove(curve1.Start);
            crclpts.Remove(curve1.End);
            Point ptToRmv=crclpts[0];
            for(int i=1;i<4;i++)
            {
                if (ptToRmv.Distance(curve1) < crclpts[i].Distance(curve1))
                    ptToRmv = crclpts[i];
            }
            crclpts.Remove(ptToRmv);
            Arc closestArc = Create.Arc(crclpts[0], crclpts[1], crclpts[2]);
            return closestArc.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Line curve1, PolyCurve curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Line curve1, Polyline curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Arc curve1, Line curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Arc curve1, Arc curve2,double tolerance=Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return 0f;
            double distance1 = Math.Min(curve2.EndPoint().Distance(curve1), curve2.StartPoint().Distance(curve1));
            double distance2 = Math.Min(curve1.EndPoint().Distance(curve2), curve1.StartPoint().Distance(curve2));
            double min = Math.Min(distance1, distance2);
            Arc tmp, tmp2;
            tmp = curve1;
            tmp2 = curve2;
            if (distance2 < distance1)
            {
                tmp = curve2;
                tmp2 = curve1;
            }
            Point start = tmp2.StartPoint();
            Point end=tmp2.EndPoint();
            Point binSearch = new Point();
            while ((start - end).Length() > tolerance * tolerance)
            {
                binSearch = tmp2.PointAtParameter(0.5);
                if (start.Distance(tmp) > end.Distance(tmp))
                    start = binSearch;
                else
                    end = binSearch;
                tmp2 = tmp2.Trim(start, end);
            }
            min = Math.Min(start.Distance(tmp), min);
            return Math.Min(min, end.Distance(tmp));
        }

        /***************************************************/

        public static double Distance(this Arc curve1, Circle curve2, double tolerance=Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return 0f;
            double distance = Math.Min(curve1.EndPoint().Distance(curve2), curve1.StartPoint().Distance(curve2));
            if (curve1.IIsCoplanar(curve2))
            {
                return Math.Min(Math.Abs(curve2.Centre.Distance(curve1) - curve2.Radius), distance);
            }
            List<Point> crclpts=new List<Point>();
            for(double i=0.1;i<1;i+=0.25)
            {
                crclpts.Add(curve2.PointAtParameter(i));
            }
            Arc tmp = Create.Arc(crclpts[0], crclpts[1], crclpts[2]);
            Arc tmp2 = Create.Arc(crclpts[2], crclpts[3], crclpts[0]);
            distance = Math.Min(tmp2.Distance(curve1), distance);
            return Math.Min(tmp.Distance(curve1),distance);
        }

        /***************************************************/

        public static double Distance(this Arc curve1, PolyCurve curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Arc curve1, Polyline curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Circle curve1, Line curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Circle curve1, Arc curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Circle curve1, Circle curve2, double tolerance=Tolerance.Distance)
        {
            if (curve1.CurveIntersections(curve2).Count > 0)
                return 0f;
            if (curve1.IIsCoplanar(curve2))
            {
                if (curve1.Centre == curve2.Centre)
                    return Math.Abs(curve2.Radius - curve1.Radius);
                if (curve1.Centre.Distance(curve2.Centre) < Math.Max(curve2.Radius, curve1.Radius))
                {
                    if (curve2.Radius < curve1.Radius)
                        return curve2.Centre.Distance(curve1) - curve2.Radius;
                    else
                        return curve1.Centre.Distance(curve2) - curve1.Radius;
                }
                else
                    return curve1.Centre.Distance(curve2.Centre) - (curve2.Radius + curve1.Radius);
            }
            List<Point> crclpts=new List<Point>();
            for(double i=0;i<1;i+=0.25)
            {
                crclpts.Add(curve1.PointAtParameter(i));
            }
            Arc tmp = Create.Arc(crclpts[0], crclpts[1], crclpts[2]);
            Arc tmp2 = Create.Arc(crclpts[2], crclpts[3], crclpts[0]);
            return Math.Min(tmp.Distance(curve2), tmp2.Distance(curve2));
        }

        /***************************************************/

        public static double Distance(this Circle curve1, PolyCurve curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Circle curve1, Polyline curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this PolyCurve curve1, Line curve2, double tolerance=Tolerance.Distance)
        {
            double distance = curve2.IDistance(curve1.Curves[0]);
            for (int i = 1; i < curve1.Curves.Count; i++)
                distance = Math.Min(distance, curve2.IDistance(curve1.Curves[i]));
            return distance;
        }

        /***************************************************/

        public static double Distance(this PolyCurve curve1, Arc curve2, double tolerance=Tolerance.Distance)
        {
            double distance = curve2.IDistance(curve1.Curves[0]);
            for (int i = 1; i < curve1.Curves.Count; i++)
                distance = Math.Min(distance, curve2.IDistance(curve1.Curves[i]));
            return distance;
        }

        /***************************************************/

        public static double Distance(this PolyCurve curve1, Circle curve2, double tolerance=Tolerance.Distance)
        {
            double distance = curve2.IDistance(curve1.Curves[0]);
            for (int i = 1; i < curve1.Curves.Count; i++)
                distance = Math.Min(distance, curve2.IDistance(curve1.Curves[i]));
            return distance;
        }

        /***************************************************/
        public static double Distance(this PolyCurve curve1, Polyline curve2, double tolerance=Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve2.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve2.ControlPoints[i], curve2.ControlPoints[i + 1]));
            if (curve2.IsClosed())
                temp.Add(Create.Line(curve2.ControlPoints[curve2.ControlPoints.Count - 1], curve2.ControlPoints[0]));
            double distance = temp[0].IDistance(curve1);
            for (int i = 0; i < temp.Count; i++)
                distance = Math.Min(distance, temp[i].IDistance(curve1));
            return distance;
        }

        /***************************************************/

        public static double Distance(this PolyCurve curve1, PolyCurve curve2, double tolerance=Tolerance.Distance)
        {
            double distance = curve1.Curves[0].IDistance(curve2.Curves[0]);
            for (int i = 0; i < curve1.Curves.Count; i++)
            {
                for (int j = 0; j < curve2.Curves.Count; j++)
                    distance = Math.Min(curve1.Curves[i].IDistance(curve2.Curves[j]), distance);
            }
            return distance;
        }

        /***************************************************/

        public static double Distance(this Polyline curve1, Line curve2, double tolerance=Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve1.ControlPoints[i], curve1.ControlPoints[i + 1]));
            double distance = curve2.Distance(temp[0]);
            for (int i = 1; i < temp.Count; i++)
                distance = Math.Min(curve2.Distance(temp[i]), distance);
            return distance;
        }

        /***************************************************/

        public static double Distance(this Polyline curve1, Arc curve2, double tolerance=Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve1.ControlPoints[i], curve1.ControlPoints[i + 1]));
            double distance = curve2.Distance(temp[0]);
            for (int i = 1; i < temp.Count; i++)
                distance = Math.Min(curve2.Distance(temp[i]), distance);
            return distance;
        }

        /***************************************************/

        public static double Distance(this Polyline curve1, Circle curve2, double tolerance=Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve1.ControlPoints[i], curve1.ControlPoints[i + 1]));
            double distance = curve2.Distance(temp[0]);
            for (int i = 1; i < temp.Count; i++)
                distance = Math.Min(curve2.Distance(temp[i]), distance);
            return distance;
        }

        /***************************************************/

        public static double Distance(this Polyline curve1, PolyCurve curve2, double tolerance=Tolerance.Distance)
        {
            return curve2.Distance(curve1);
        }

        /***************************************************/

        public static double Distance(this Polyline curve1, Polyline curve2, double tolerance=Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
                temp.Add(Create.Line(curve1.ControlPoints[i], curve1.ControlPoints[i + 1]));
            List<Line> temp2 = new List<Line>();
            for (int i = 0; i < curve2.ControlPoints.Count - 1; i++)
                temp2.Add(Create.Line(curve2.ControlPoints[i], curve2.ControlPoints[i + 1]));
            double distance = temp[0].Distance(temp2[0]);
            for (int i = 0; i < temp.Count; i++)
            {
                for (int j = 0; j < temp2.Count; j++)
                    distance = Math.Min(distance, temp[i].Distance(temp2[j]));
            }
            return distance;
        }

        /***************************************************/

        [NotImplemented]
        public static double Distance(this ICurve curve1, Ellipse curve2, double tolerance=Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static double Distance(this ICurve curve1, NurbsCurve curve2, double tolerance=Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IDistance(this Point point, ICurve curve)
        {
            return Distance(point, curve as dynamic);
        }

        public static double IDistance(this ICurve curve1, ICurve curve2, double tolerance=Tolerance.Distance)
        {
            return Distance(curve1 as dynamic, curve2 as dynamic);
        }
        /***************************************************/
    }
}
