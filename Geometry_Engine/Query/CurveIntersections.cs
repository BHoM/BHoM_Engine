﻿/*
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

        public static List<Point> CurveIntersections(this Arc curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            Circle circle1 = new Circle { Centre = curve1.CoordinateSystem.Origin, Normal = curve1.CoordinateSystem.Z, Radius = curve1.Radius };
            Circle circle2 = new Circle { Centre = curve2.CoordinateSystem.Origin, Normal = curve2.CoordinateSystem.Z, Radius = curve2.Radius };

            List<Point> iPts = circle1.CurveIntersections(circle2, tolerance);

            Point midPoint1 = curve1.PointAtParameter(0.5);
            Point midPoint2 = curve2.PointAtParameter(0.5);
            double dist1 = midPoint1.Distance(curve1.StartPoint());
            double dist2 = midPoint2.Distance(curve2.StartPoint());

            for (int i = iPts.Count - 1; i >= 0; i--)
            {
                if (midPoint1.Distance(iPts[i]) - dist1 > tolerance || midPoint2.Distance(iPts[i]) - dist2 > tolerance)
                    iPts.RemoveAt(i);
            }

            return iPts;
        }

        /***************************************************/

        public static List<Point> CurveIntersections(this Arc curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            Circle circle1 = new Circle { Centre = curve1.CoordinateSystem.Origin, Normal = curve1.CoordinateSystem.Z, Radius = curve1.Radius };

            List<Point> iPts = circle1.CurveIntersections(curve2, tolerance);

            Point midPoint1 = curve1.PointAtParameter(0.5);
            double dist = midPoint1.Distance(curve1.StartPoint());

            for (int i = iPts.Count - 1; i >= 0; i--)
            {
                if (midPoint1.Distance(iPts[i]) - dist > tolerance)
                    iPts.RemoveAt(i);
            }

            return iPts;
        }

        /***************************************************/

        public static List<Point> CurveIntersections(this Circle curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurveIntersections(curve1, tolerance);
        }

        /***************************************************/

        public static List<Point> CurveIntersections(this Circle curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> result = new List<Point>();

            Plane p1 = new Plane { Origin = curve1.Centre, Normal = curve1.Normal };
            Plane p2 = new Plane { Origin = curve2.Centre, Normal = curve2.Normal };

            if (p1.IsCoplanar(p2, tolerance))
            {
                Point c1 = curve1.Centre;
                Point c2 = curve2.Centre;
                double r1 = curve1.Radius;
                double r2 = curve2.Radius;
                Vector n1 = curve1.Normal;
                Vector n2 = curve2.Normal;

                double sqrDist = c1.SquareDistance(c2);
                double dist = Math.Sqrt(sqrDist);

                if (dist <= tolerance)
                    return result;

                double sumRadii = r1 + r2;
                double difRadii = Math.Abs(r1 - r2);
                Vector dir = (c2 - c1).Normalise();

                if (Math.Abs(dist - sumRadii) <= tolerance)
                    result.Add(c1 + dir * r1);
                else if (Math.Abs(dist - difRadii) <= tolerance)
                    result.Add(c1 - dir * r1);
                else if (dist - sumRadii < tolerance && dist - difRadii >= tolerance)
                {
                    double a = (r1 * r1 - r2 * r2 + sqrDist) / (2 * dist);
                    Point midPt = (c1 + dir * a);
                    Vector perp = dir.Rotate(Math.PI * 0.5, n1);
                    double shift = Math.Sqrt(r1 * r1 - midPt.SquareDistance(c1));
                    result.Add(midPt + perp * shift);
                    result.Add(midPt - perp * shift);
                }
            }
            else
            {
                double sqTolerance = tolerance * tolerance;
                List<Point> intPts1 = curve1.PlaneIntersections(p2, tolerance);
                List<Point> intPts2 = curve2.PlaneIntersections(p1, tolerance);

                foreach(Point pt1 in intPts1)
                {
                    foreach(Point pt2 in intPts2)
                    {
                        if (pt1.SquareDistance(pt2) <= sqTolerance)
                            result.Add((pt1 + pt2) * 0.5);
                    }
                }

                result = result.CullDuplicates(tolerance);
            }

            return result;
        }

        /***************************************************/

        public static List<Point> CurveIntersections(this Line curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.LineIntersections(curve1, false, tolerance);
        }

        /***************************************************/

        public static List<Point> CurveIntersections(this Arc curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return curve1.LineIntersections(curve2, false, tolerance);
        }

        /***************************************************/

        public static List<Point> CurveIntersections(this Circle curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return curve1.LineIntersections(curve2, false, tolerance);
        }

        /***************************************************/

        public static List<Point> CurveIntersections(this Line curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.LineIntersections(curve1, false, tolerance);
        }

        /***************************************************/

        public static List<Point> CurveIntersections(this Line curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return curve1.LineIntersections(curve2, false, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> ICurveIntersections(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> result = new List<Point>();
            foreach (ICurve c1 in curve1.ISubParts())
            {
                foreach (ICurve c2 in curve2.ISubParts())
                {
                    result.AddRange(CurveIntersections(c1 as dynamic, c2 as dynamic, tolerance));
                }
            }
            return result;
        }

        /***************************************************/
    }
}