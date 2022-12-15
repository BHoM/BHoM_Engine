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

using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the partial values of the B-spline basis function for t value as normalised parameter.")]
        [Input("knots", "Knot vector defining the basis function.")]
        [Input("i", "Index the function is evaluated at. The value of the function is the sum of this functions value for all values of i.")]
        [Input("n", "Degree of the of the basis function. Affects how many adjacent knots control the value.")]
        [Input("t", "Parameter to evaluate the function at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Output("Value of the function for the specified index. The full value of the function should be a sum of all possible i's.")]
        public static double BasisFunction(this List<double> knots, int i, int n, double t)
        {
            t = t < 0 ? 0 : t > 1 ? 1 : t;

            double min = knots[n - 1];
            double max = knots[knots.Count - n];
            t = min + (max - min) * t;

            return BasisFunctionGlobal(knots, i, n, t);
        }

        /***************************************************/

        [Description("Gets the basis functions for the given knot vector for the parameter t in the given span and given degree.")]
        [Input("knots", "The knot vector to evaluate.")]
        [Input("span", "The span in which the parameter t resides. The KnotSpan method can be used to identify the span.")]
        [Input("degree", "Degree of the Curve/Surface in the direction of the provided knots.")]
        [Input("t", "The parameter to evaluate.")]
        [Output("basis", "The basis functions of the knot vector for the given parameter in the given span.")]
        public static List<double> BasisFunctions(this IReadOnlyList<double> knots, int span, int degree, double t)
        {
            List<double> basis = Enumerable.Repeat(0.0, degree + 1).ToList();
            basis[0] = 1.0;
            double[] left = new double[degree + 1];
            double[] right = new double[degree + 1];

            for (int j = 1; j <= degree; j++)
            {
                left[j] = t - knots[span + 1 - j];
                right[j] = knots[span + j] - t;
                double saved = 0.0;
                for (int r = 0; r < j; r++)
                {
                    double temp = basis[r] / (right[r + 1] + left[j - r]);
                    basis[r] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }
                basis[j] = saved;
            }
            return basis;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets the partial values of the B-spline basis function for t value as global parameter.")]
        private static double BasisFunctionGlobal(List<double> knots, int i, int n, double t)
        {
            if (n == 0)
            {
                double sKnot = knots[Math.Max(Math.Min(i, knots.Count - 1), 0)];
                double eKnot = knots[Math.Max(Math.Min(i + 1, knots.Count - 1), 0)];

                if (t >= knots[knots.Count - 1])
                    return t > sKnot && t <= eKnot ? 1 : 0;
                else
                    return t >= sKnot && t < eKnot ? 1 : 0;
            }

            double ret = 0.0;

            double linInter1 = LinearKnotInterpelation(knots, i, n, t);
            if (linInter1 != 0)
                ret += linInter1 * BasisFunctionGlobal(knots, i, n - 1, t);
            double linInter2 = 1.0 - LinearKnotInterpelation(knots, i + 1, n, t);
            if (linInter2 != 0)
                ret += linInter2 * BasisFunctionGlobal(knots, i + 1, n - 1, t);

            return ret;
        }

        /***************************************************/

        [Description("Finds the function value of f(t) in the knot-span (i,n)")]
        private static double LinearKnotInterpelation(List<double> knots, int i, int n, double t)
        {
            double sKnot = knots[Math.Max(Math.Min(i, knots.Count - 1), 0)];
            double eKnot = knots[Math.Max(Math.Min(i + n, knots.Count - 1), 0)];

            if (sKnot == eKnot)
                return 0;

            return (t - sKnot) / (eKnot - sKnot);
        }

        /***************************************************/
    }
}





