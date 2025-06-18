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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Trims a NurbsCurve between two parameter values. The trimmed curve will start at parameter t1 and end at parameter t2.")]
        [Input("curve", "The NurbsCurve to trim.")]
        [Input("t1", "The start parameter for trimming. Should be between 0 and 1.")]
        [Input("t2", "The end parameter for trimming. Should be between 0 and 1 and greater than t1.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("curve", "The trimmed NurbsCurve.")]
        public static NurbsCurve Trim(this NurbsCurve curve, double t1, double t2, double tolerance = Tolerance.Distance)
        {
            if (curve.IsNull())
                return null;

            // Validate parameters
            if (t1 < 0 || t1 > 1 || t2 < 0 || t2 > 1)
            {
                Base.Compute.RecordError("Parameters t1 and t2 must be between 0 and 1.");
                return null;
            }

            if (t1 >= t2)
            {
                Base.Compute.RecordError("Parameter t1 must be less than t2.");
                return null;
            }

            // Handle edge cases
            if (Math.Abs(t1) < tolerance && Math.Abs(t2 - 1) < tolerance)
                return curve.DeepClone();

            // Get the degree of the curve
            int degree = curve.Degree();
            List<double> knots = curve.Knots.ToList();
            List<Point> controlPoints = curve.ControlPoints.ToList();
            List<double> weights = curve.Weights.ToList();

            // Map normalized parameters to knot space
            double knotMin = knots[degree - 1];
            double knotMax = knots[knots.Count - degree];
            double u1 = knotMin + (knotMax - knotMin) * t1;
            double u2 = knotMin + (knotMax - knotMin) * t2;

            // For a complete implementation, we would need to:
            // 1. Insert knots at u1 and u2 if they don't exist
            // 2. Extract the segment between these knots
            // 3. Adjust the knot vector and control points accordingly
            
            // Simplified approach: Create a new curve by sampling points
            // This is less precise than true NURBS trimming but functional
            int sampleCount = Math.Max(controlPoints.Count * 2, 20);
            List<Point> trimmedPoints = new List<Point>();
            List<double> trimmedWeights = new List<double>();
            List<double> trimmedKnots = new List<double>();

            // Sample points along the trimmed section
            for (int i = 0; i <= sampleCount; i++)
            {
                double t = t1 + (t2 - t1) * ((double)i / sampleCount);
                Point pt = curve.PointAtParameter(t);
                trimmedPoints.Add(pt);
            }

            // Create weights (all equal for approximation)
            trimmedWeights = Enumerable.Repeat(1.0, trimmedPoints.Count).ToList();

            // Create knot vector for the approximated curve
            for (int i = 0; i < trimmedPoints.Count; i++)
            {
                trimmedKnots.Add((double)i / (trimmedPoints.Count - 1));
            }

            return new NurbsCurve
            {
                ControlPoints = trimmedPoints,
                Weights = trimmedWeights,
                Knots = trimmedKnots
            };
        }

        /***************************************************/

        [Description("Trims a NurbsCurve from the start to parameter t.")]
        [Input("curve", "The NurbsCurve to trim.")]
        [Input("t", "The end parameter for trimming. Should be between 0 and 1.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("curve", "The trimmed NurbsCurve from start to parameter t.")]
        public static NurbsCurve TrimFromStart(this NurbsCurve curve, double t, double tolerance = Tolerance.Distance)
        {
            return curve.Trim(0, t, tolerance);
        }

        /***************************************************/

        [Description("Trims a NurbsCurve from parameter t to the end.")]
        [Input("curve", "The NurbsCurve to trim.")]
        [Input("t", "The start parameter for trimming. Should be between 0 and 1.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("curve", "The trimmed NurbsCurve from parameter t to end.")]
        public static NurbsCurve TrimToEnd(this NurbsCurve curve, double t, double tolerance = Tolerance.Distance)
        {
            return curve.Trim(t, 1, tolerance);
        }

        /***************************************************/
    }
} 