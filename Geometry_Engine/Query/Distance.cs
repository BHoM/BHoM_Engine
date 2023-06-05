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
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes and returns the Euclidean distance between two points.")]
        [Input("a", "First Point for distance computation.")]
        [Input("b", "Second Point for distance computation.")]
        [Output("dist", "The Euclidean distance between the two points.", typeof(Length))]
        public static double Distance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/

        [Description("Computes and returns the Euclidean square distance between two points, that is the sum of the squares of the differences of each component.")]
        [Input("a", "First Point for square distance computation.")]
        [Input("b", "Second Point for  square distance computation.")]
        [Output("sqDist", "The square distance between the two points.")]
        public static double SquareDistance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        [Description("Computes and returns the Euclidean distance between two vectors treated as position vectors.")]
        [Input("a", "First Vector for distance computation.")]
        [Input("b", "Second Vector for distance computation.")]
        [Output("dist", "The Euclidean distance between the two Vectors.", typeof(Length))]
        public static double Distance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/


        [Description("Computes and returns the square distance between two vectors treated as position vectors.")]
        [Input("a", "First Vector for square distance computation.")]
        [Input("b", "Second Vector for  square distance computation.")]
        [Output("sqDist", "The square distance between the two Vectors.")]
        public static double SquareDistance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        [Description("Computes and returns the Euclidean distance between the Point and the closest point on the Plane.")]
        [Input("point", "Point for distance computation.")]
        [Input("plane", "Plane for distance computation.")]
        [Output("dist", "The Euclidean distance between the Point and the Plane.", typeof(Length))]
        public static double Distance(this Point point, Plane plane)
        {
            Vector normal = plane.Normal.Normalise();
            return Math.Abs(normal.DotProduct(point - plane.Origin));
        }


        /***************************************************/
        /****       Public Methods - Point/Curve        ****/
        /***************************************************/

        [Description("Computes and returns the Euclidean distance between the Point and the closest point on the Line.")]
        [Input("point", "Point for distance computation.")]
        [Input("line", "Line for distance computation.")]
        [Input("infiniteSegment", "If true, distance will be computed to the closest point on the infinite line. If false, the distance will be compated to the closest point on the finite line segment.")]
        [Output("dist", "The Euclidean distance between the Point and the Line.", typeof(Length))]
        public static double Distance(this Point point, Line line, bool infiniteSegment = false)
        {
            return point.Distance(line.ClosestPoint(point, infiniteSegment));
        }

        /***************************************************/

        [Description("Computes and returns the square distance between the Point and the closest point on the Line.")]
        [Input("point", "Point for square distance computation.")]
        [Input("line", "Line for square distance computation.")]
        [Input("infiniteSegment", "If true, distance will be computed to the closest point on the infinite line. If false, the distance will be compated to the closest point on the finite line segment.")]
        [Output("dist", "The Euclidean distance between the Point and the Line.", typeof(Length))]
        public static double SquareDistance(this Point point, Line line, bool infiniteSegment = false)
        {
            return point.SquareDistance(line.ClosestPoint(point, infiniteSegment));
        }

        /***************************************************/

        [Description("Computes and returns the Euclidean distance between the Point and the closest point on the Arc.")]
        [Input("point", "Point for distance computation.")]
        [Input("arc", "Arc for distance computation.")]
        [Output("dist", "The Euclidean distance between the Point and the Arc.", typeof(Length))]
        public static double Distance(this Point point, Arc arc)
        {
            return point.Distance(arc.ClosestPoint(point));
        }

        /***************************************************/

        [Description("Computes and returns the Euclidean distance between the Point and the closest point on the Circle.")]
        [Input("point", "Point for distance computation.")]
        [Input("circle", "Circle for distance computation.")]
        [Output("dist", "The Euclidean distance between the Point and the Circle.", typeof(Length))]
        public static double Distance(this Point point, Circle circle)
        {
            return point.Distance(circle.ClosestPoint(point));
        }

        /***************************************************/

        [Description("Computes and returns the Euclidean distance between the Point and the closest point on the Ellipse.")]
        [Input("point", "Point for distance computation.")]
        [Input("ellipse", "Ellipse for distance computation.")]
        [Output("dist", "The Euclidean distance between the Point and the Ellipse.", typeof(Length))]
        public static double Distance(this Point point, Ellipse ellipse)
        {
            if (ellipse.IsNull() || point.IsNull())
                return double.NaN;

            return point.Distance(ellipse.ClosestPoint(point));
        }

        /***************************************************/

        [Description("Computes and returns the Euclidean distance between the Point and the closest point on the Polyline.")]
        [Input("point", "Point for distance computation.")]
        [Input("curve", "Polyline for distance computation.")]
        [Output("dist", "The Euclidean distance between the Point and the Polyline.", typeof(Length))]
        public static double Distance(this Point point, Polyline curve)
        {
            return point.Distance(curve.ClosestPoint(point));
        }

        /***************************************************/

        [Description("Computes and returns the Euclidean distance between the Point and the closest point on the PolyCurve.")]
        [Input("point", "Point for distance computation.")]
        [Input("curve", "PolyCurve for distance computation.")]
        [Output("dist", "The Euclidean distance between the Point and the PolyCurve.", typeof(Length))]
        public static double Distance(this Point point, PolyCurve curve)
        {
            return point.Distance(curve.ClosestPoint(point));
        }

        /***************************************************/

        [Description("Computes the distance between 2 points along the direction of an input vector.")]
        [Input("point1", "A point to computer the distance from.")]
        [Input("point2", "A point to computer the distance to.")]
        [Input("vector", "A vector along which the point distance will be computed.")]
        [Output("distance", "The distance between 2 points along the direction of an input vector.")]
        public static double DistanceAlongVector(this Point point1, Point point2, Vector vector)
        {
            Vector pointVector = point2 - point1;
            return Math.Abs(pointVector.DotProduct(vector.Normalise()));
        }

        /***************************************************/
        /****       Public Methods - Curve/Curve        ****/
        /***************************************************/

        [Description("Computes and returns the minimum distance between the two curves.")]
        [Input("curve1", "First curve for distance computation.")]
        [Input("curve2", "Second curve for distance computation.")]
        [Input("tolerance", "Distance tolerance used in the method.", typeof(Length))]
        [Output("dist", "The mimum distance between the two curves.", typeof(Length))]
        public static double Distance(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            BH.oM.Base.Output<Point, Point> results = curve1.ICurveProximity(curve2, tolerance);
            return results.Item1.Distance(results.Item2);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Computes and returns the Euclidean distance between the Point and the closest point on the curve.")]
        [Input("point", "Point for distance computation.")]
        [Input("curve", "curve for distance computation.")]
        [Output("dist", "The Euclidean distance between the Point and the curve.", typeof(Length))]
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




