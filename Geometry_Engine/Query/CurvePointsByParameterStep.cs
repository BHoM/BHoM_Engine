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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes a set number of points on the curve by dividing the curve domain into equal steps. Note that the points wont be equal distance divided for the general case.")]
        [Input("curve", "The NurbsCurve to divide.")]
        [Input("steps", "The number of divisions.")]
        [Output("pts", "Points along the curve.")]
        public static Output<List<Point>, List<double>> CurvePointsByParameterStep(this NurbsCurve curve, int steps)
        {

            Point[] pts = new Point[steps];
            double[] ts = new double[steps];
            bool periodic = curve.IsPeriodic();
            int degree = curve.Degree();

            List<double> knots = curve.Knots;
            int n = knots.Count - degree;   //Index of first knot correpsonding to last controlpoint
            double min = knots[degree - 1];
            double max = knots[n];

            //Compute stepsize
            double stepSize = (max - min) / (double)((double)steps - 1.0);

            //Get start point of the curve
            if (periodic)
                pts[0] = ComputeCurvePoint(curve.ControlPoints, curve.Weights, BasisFunctions(knots, degree - 1, degree, min), degree - 1, degree);
            else
                pts[0] = curve.ControlPoints[0];

            ts[0] = min;
            int iter = 1;   //Current point index

            double t = min + stepSize;

            //Iterate along the knot vector and pick up all points corresponding to the current knot span before moving to next
            for (int k = degree - 1; k < n; k++)
            {
                while (knots[k] == knots[k + 1] && knots[k] < 1)
                {
                    k++;
                }
                while (t < knots[k + 1])    //While t is the knotspan corresponding to k
                {
                    double[] basisArray = BasisFunctions(knots, k, degree, t);
                    pts[iter] = ComputeCurvePoint(curve.ControlPoints, curve.Weights, basisArray, k, degree);
                    ts[iter] = t;
                    iter++;
                    t += stepSize;
                }
            }

            if (periodic)
                pts[steps - 1] = ComputeCurvePoint(curve.ControlPoints, curve.Weights, BasisFunctions(knots, n - 1, degree, max), n - 1, degree);
            else
                pts[steps - 1] = curve.ControlPoints.Last();

            ts[steps - 1] = max;
            return new Output<List<Point>, List<double>> { Item1 = pts.ToList(), Item2 = ts.ToList() };
        }

        /***************************************************/
        /**** private Methods                           ****/
        /***************************************************/

        private static Point ComputeCurvePoint(List<Point> pts, List<double> weights, double[] basis, int span, int degree)
        {
            Point pt = new Point();
            double sum = 0;
            int ptIndexAddtion = span - degree + 1;
            for (int i = 0; i < basis.Length; i++)
            {
                int ptIndex = ptIndexAddtion + i;
                double basisWeight = basis[i] * weights[ptIndex];
                pt += pts[ptIndex] * basisWeight;
                sum += basisWeight;
            }
            return pt / sum;
        }

        /***************************************************/
    }
}
