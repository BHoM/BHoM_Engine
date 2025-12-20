/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all basis functions up to the specified degree for the given knot vector at parameter t in the given span.")]
        [Input("knots", "The knot vector to evaluate.")]
        [Input("span", "The span in which the parameter t resides.")]
        [Input("degree", "Degree of the Curve/Surface in the direction of the provided knots.")]
        [Input("t", "The parameter to evaluate.")]
        [Output("bases", "All basis functions from degree 0 up to the specified degree.")]
        public static List<List<double>> AllBasisFunctions(this IList<double> knots, int span, int degree, double t)
        {
            List<List<double>> bases = new List<List<double>>
            {
                new List<double> { 1.0 }
            };

            double[] left = new double[degree + 1];
            double[] right = new double[degree + 1];

            for (int j = 1; j <= degree; j++)
            {
                List<double> current = new List<double>();
                left[j] = t - knots[span + 1 - j];
                right[j] = knots[span + j] - t;
                double saved = 0.0;
                for (int r = 0; r < j; r++)
                {
                    double temp = bases[j - 1][r] / (right[r + 1] + left[j - r]);
                    current.Add(saved + right[r + 1] * temp);
                    saved = left[j - r] * temp;
                }
                current.Add(saved);
                bases.Add(current);
            }
            return bases;
        }

        /***************************************************/
    }
}
