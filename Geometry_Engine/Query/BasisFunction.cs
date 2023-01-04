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

            return LinearKnotInterpelation(knots, i, n, t) * BasisFunctionGlobal(knots, i, n - 1, t) +
                   (1 - LinearKnotInterpelation(knots, i + 1, n, t)) * BasisFunctionGlobal(knots, i + 1, n - 1, t);
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




