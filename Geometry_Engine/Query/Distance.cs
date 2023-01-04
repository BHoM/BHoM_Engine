/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
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
        /****       Public Methods - Curve/Curve        ****/
        /***************************************************/

        public static double Distance(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            BH.oM.Base.Output<Point, Point> results = curve1.ICurveProximity(curve2, tolerance);
            return results.Item1.Distance(results.Item2);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IDistance(this Point point, ICurve curve)
        {
            return Distance(point, curve as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static double Distance(this Point point, ICurve curve)
        {
            Base.Compute.RecordError($"Distance is not implemented for ICurves of type: {curve.GetType().Name}.");
            return double.NaN;
        }

        /***************************************************/
    }
}




