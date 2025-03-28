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

using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Collections.Generic;

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






