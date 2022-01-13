/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, Arc arc, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (arc.Angle() <= angleTolerance)
                return points.Select(p => p.DeepClone()).ToList();

            List<Tuple<Point, double>> cData = points.Select(p => new Tuple<Point, double>(p.DeepClone(), arc.ParameterAtPoint(arc.ClosestPoint(p)))).ToList();

            cData.Sort(delegate (Tuple<Point, double> d1, Tuple<Point, double> d2)
            {
                return d1.Item2.CompareTo(d2.Item2);
            });

            return cData.Select(d => d.Item1).ToList();
        }

        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, Circle circle, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (circle.Radius <= distanceTolerance)
                return points.Select(p => p.DeepClone()).ToList();

            List<Tuple<Point, double>> cData = points.Select(p => new Tuple<Point, double>(p.DeepClone(), circle.ParameterAtPoint(circle.ClosestPoint(p)))).ToList();

            cData.Sort(delegate (Tuple<Point, double> d1, Tuple<Point, double> d2)
            {
                return d1.Item2.CompareTo(d2.Item2);
            });

            return cData.Select(d => d.Item1).ToList();
        }

        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, Line line, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (line.Length() <= distanceTolerance)
                return points.Select(p => p.DeepClone()).ToList();

            Vector lDir = line.Direction();
            List<Tuple<Point, Point>> cData = points.Select(p => new Tuple<Point, Point>(p.DeepClone(), (p.Project(line)))).ToList();

            if ((Math.Abs(lDir.X)) > angleTolerance)
            {
                cData.Sort(delegate (Tuple<Point, Point> d1, Tuple<Point, Point> d2)
                {
                    return d1.Item2.X.CompareTo(d2.Item2.X);
                });

                if (lDir.X < 0)
                    cData.Reverse();
            }
            else if ((Math.Abs(lDir.Y)) > angleTolerance)
            {
                cData.Sort(delegate (Tuple<Point, Point> d1, Tuple<Point, Point> d2)
                {
                    return d1.Item2.Y.CompareTo(d2.Item2.Y);
                });

                if (lDir.Y < 0)
                    cData.Reverse();
            }
            else
            {
                cData.Sort(delegate (Tuple<Point, Point> d1, Tuple<Point, Point> d2)
                {
                    return d1.Item2.Z.CompareTo(d2.Item2.Z);
                });

                if (lDir.Z < 0)
                    cData.Reverse();
            }

            return cData.Select(d => d.Item1).ToList();
        }

        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, PolyCurve curve, double tolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {

            List<Tuple<Point, double>> cData = points.Select(p => new Tuple<Point, double>(p.DeepClone(), curve.ParameterAtPoint(curve.ClosestPoint(p)))).ToList();

            cData.Sort(delegate (Tuple<Point, double> d1, Tuple<Point, double> d2)
            {
                return d1.Item2.CompareTo(d2.Item2);
            });

            return cData.Select(d => d.Item1).ToList();
        }

        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, Polyline curve, double tolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            List<Point> result = new List<Point>();
            List<Point> tmpResult = new List<Point>();

            foreach (Point pt in points)
            {
                if (pt.IsOnCurve(curve, tolerance))
                    tmpResult.Add(pt);
            }

            if (!tmpResult.Any())
                return result;
            else if (tmpResult.Count == 1)
                return tmpResult;


            List<Point> subPoints = new List<Point>();

            foreach (ICurve subPart in curve.ISubParts())
            {
                foreach (Point pt in tmpResult)
                {
                    if (pt.IIsOnCurve(subPart, tolerance))
                        subPoints.Add(pt);
                }
                subPoints = subPoints.SortAlongCurve(subPart as Line);

                if (result.Any() && subPoints.Any())
                {
                    if (result[result.Count - 1].IsEqual(subPoints[0]))
                        result.RemoveAt(result.Count - 1);
                }
                result.AddRange(subPoints);
                subPoints.Clear();
            }

            return result;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> ISortAlongCurve(this List<Point> points, ICurve curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            return SortAlongCurve(points, curve as dynamic, distanceTolerance, angleTolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<Point> SortAlongCurve(this List<Point> points, ICurve curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            Base.Compute.RecordError($"SortAlongCurve is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}

