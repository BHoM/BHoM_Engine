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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using BH.oM.Base;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static Plane FitPlane(this List<Point> points, double tolerance = Tolerance.Distance)
        {
            int n = points.Count;
            if (n < 3 || points.IsCollinear(tolerance))
                return null;

            Point C = points.Average();

            double[,] A = new double[n, 3];
            for (int i = 0; i < n; i++)
            {
                A[i, 0] = points[i].X - C.X;
                A[i, 1] = points[i].Y - C.Y;
                A[i, 2] = points[i].Z - C.Z;
            }

            Output<double[,], double[], double[,]> svd = A.SingularValueDecomposition();
            double[,] Vh = svd.Item3;
            Vector normal = new Vector { X = Vh[2, 0], Y = Vh[2, 1], Z = Vh[2, 2] };
            return new Plane { Origin = C, Normal = normal };
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





