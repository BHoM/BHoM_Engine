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

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static Plane FitPlane(this List<Point> points, double tolerance = Tolerance.Distance)
        {
            if (points.Count < 3 || points.IsCollinear(tolerance))
                return null;

            Plane result = null;
            Point origin = points.Average();
            Vector normal = new Vector();

            if (points.IsCoplanar(tolerance))
            {
                for (int i = 0; i < points.Count - 1; i++)
                    normal += (points[i] - origin).CrossProduct(points[i + 1] - origin);
                return new Plane { Origin = origin, Normal = normal.Normalise() };
            }

            double[,] MTM = new double[3, 3];
            double[,] normalizedPoints = new double[points.Count, 3];

            for (int i = 0; i < points.Count; i++)
            {
                normalizedPoints[i, 0] = points[i].X - origin.X;
                normalizedPoints[i, 1] = points[i].Y - origin.Y;
                normalizedPoints[i, 2] = points[i].Z - origin.Z;
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double value = 0;
                    for (int k = 0; k < points.Count; k++)
                    {
                        value += normalizedPoints[k, i] * normalizedPoints[k, j];
                    }
                    MTM[i, j] = value;
                }
            }

            Vector[] eigenvectors = MTM.Eigenvectors(tolerance);
            if (eigenvectors == null)
                return null;

            double leastSquares = double.PositiveInfinity;
            foreach (Vector eigenvector in eigenvectors)
            {
                double squares = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    squares += Math.Pow(eigenvector.X * normalizedPoints[i, 0] + eigenvector.Y * normalizedPoints[i, 1] + eigenvector.Z * normalizedPoints[i, 2], 2);
                }

                if (squares <= leastSquares)
                {
                    leastSquares = squares;
                    result = new Plane { Origin = origin, Normal = eigenvector.Normalise() };
                }
            }

            return result;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static Plane FitPlane(this Arc curve, double tolerance = Tolerance.Distance)
        {
            return (Plane)curve.CoordinateSystem;
        }

        /***************************************************/

        public static Plane FitPlane(this Circle curve, double tolerance = Tolerance.Distance)
        {
            return new Plane { Origin = curve.Centre, Normal = curve.Normal };
        }

        /***************************************************/

        public static Plane FitPlane(this Ellipse curve, double tolerance = Tolerance.Distance)
        {
            return new Plane { Origin = curve.Centre, Normal = curve.Axis1.CrossProduct(curve.Axis2).Normalise() };
        }

        /***************************************************/

        public static Plane FitPlane(this Line curve, double tolerance = Tolerance.Distance)
        {
            return null;
        }

        /***************************************************/
        
        public static Plane FitPlane(this NurbsCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.FitPlane();
        }

        /***************************************************/

        public static Plane FitPlane(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return FitPlane(curve.ControlPoints(), tolerance);
        }

        /***************************************************/

        public static Plane FitPlane(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            return FitPlane(curve.ControlPoints, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Plane FitPlane(this PlanarSurface surface, double tolerance = Tolerance.Distance)
        {
            return IFitPlane(surface.ExternalBoundary, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Plane IFitPlane(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return FitPlane(curve as dynamic, tolerance);
        }

        /***************************************************/
    }
}




