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
using BH.oM.Base;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the partial value derivatives of the B-spline Basis function for t value as normalised parameter.")]
        [Input("knots", "Knot vector defining the basis function.")]
        [Input("i", "Index the function is evaluated at. The value of the function is the sum of this functions value for all values of i.")]
        [Input("n", "Degree of the of the basis function. Affects how many adjacent knots control the value.")]
        [Input("t", "Parameter to evaluate the function at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("k", "Degree of the derivation.")]
        [Output("Value of the function for the specified index. The full value of the function should be a sum of all possible i's.")]
        public static double DerivativeFunction(this List<double> knots, int i, int n, double t, int k = 1)
        {
            t = t < 0 ? 0 : t > 1 ? 1 : t;

            double min = knots[n - 1];
            double max = knots[knots.Count - n];
            t = min + (max - min) * t;

            return DerivativeFunctionGlobal(knots, i, n, t, k);
        }

        /***************************************************/

        [Description("Gets the basis functions for the given knot vector for the parameter t in the given span and given degree. Method is a C# implementation of method found in the Nurbs book.")]
        [Input("knots", "The knot vector to evaluate.")]
        [Input("span", "The span in which the parameter t resides. The KnotSpan method can be used to identify the span.")]
        [Input("degree", "Degree of the Curve/Surface in the direction of the provided knots.")]
        [Input("t", "The parameter to evaluate.")]
        [Output("basis", "The basis functions of the knot vector for the given parameter in the given span.")]
        public static List<List<double>> DerivativeFunctions(this IReadOnlyList<double> knots, int span, int degree, int numberOfDerivates, double t)
        {
            double[] left = new double[degree + 1];
            double[] right = new double[degree + 1];
            double[,] ndu = new double[degree + 1, degree + 1];
            ndu[0, 0] = 1.0;

            for (int j = 1; j <= degree; j++)
            {
                left[j] = t - knots[span + 1 - j];
                right[j] = knots[span + j] - t;
                double saved = 0.0;
                for (int r = 0; r < j; r++)
                {
                    //Lower triangle
                    ndu[j, r] = right[r + 1] + left[j - r];
                    double temp = ndu[r, j - 1] / ndu[j, r];
                    //Upper triangle
                    ndu[r, j] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }
                ndu[j, j] = saved;
            }

            double[,] ders = new double[numberOfDerivates + 1, degree + 1];


            for (int j = 0; j <= degree; j++)
            {
                ders[0, j] = ndu[j, degree];
            }

            double[,] a = new double[2, degree + 1];

            for (int r = 0; r <= degree; r++)
            {
                int s1 = 0;
                int s2 = 1;
                a[0, 0] = 1.0;
                for (int k = 1; k <= numberOfDerivates; k++)
                {
                    double d = 0.0;
                    int rk = r - k;
                    int pk = degree - k;

                    if (r >= k)
                    {
                        a[s2, 0] = a[s1, 0] / ndu[pk + 1, rk];
                        d = a[s2, 0] * ndu[rk, pk];
                    }
                    int j1, j2;

                    if (rk >= -1)
                        j1 = 1;
                    else
                        j1 = -rk;

                    if (r - 1 <= pk)
                        j2 = k - 1;
                    else
                        j2 = degree - r;

                    for (int j = j1; j <= j2; j++)
                    {
                        a[s2, j] = (a[s1, j] - a[s1, j - 1]) / ndu[pk + 1, rk + j];
                        d += a[s2, j] * ndu[rk + j, pk];
                    }

                    if (r <= pk)
                    {
                        a[s2, k] = -a[s1, k - 1] / ndu[pk + 1, r];
                        d += a[s2, k] * ndu[r, pk];
                    }
                    ders[k, r] = d;

                    int jTemp = s1;
                    s1 = s2;
                    s2 = jTemp;
                }
            }


            List<List<double>> dersList = new List<List<double>>();

            double sFac = 1.0;
            for (int k = 0; k <= numberOfDerivates; k++)
            {
                List<double> list = new List<double>();

                for (int j = 0; j <= degree; j++)
                {
                    list.Add(ders[k, j] * sFac);
                }
                dersList.Add(list);
                sFac *= (degree - k);
            }

            return dersList;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets the partial value derivatives of the B-spline Basis function for t value as global parameter.")]
        private static double DerivativeFunctionGlobal(List<double> knots, int i, int n, double t, int k = 1)
        {
            if (k == 0)
                return BasisFunctionGlobal(knots, i, n, t);

            double result = n * (
                KnotFactor(knots, i, n) * DerivativeFunctionGlobal(knots, i, n - 1, t, k - 1) -
                KnotFactor(knots, i + 1, n) * DerivativeFunctionGlobal(knots, i + 1, n - 1, t, k - 1));

            return result;
        }

        /***************************************************/

        private static double KnotFactor(List<double> knots, int i, int n)
        {
            double sKnot = knots[Math.Max(Math.Min(i, knots.Count - 1), 0)];
            double eKnot = knots[Math.Max(Math.Min(i + n, knots.Count - 1), 0)];

            if (eKnot == sKnot)
                return 0;

            return 1 / (eKnot - sKnot);
        }

        /***************************************************/

    }
}





