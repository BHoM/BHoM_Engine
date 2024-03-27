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

using BH.Engine.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes a tolerance to be used to check if a value is zero when computing the RowEchelonForm based on a distance tolerance. Attempts to find a numerical equivalent value of the provided geometrical tolerance, i.e. the value that would give correct results for, for example, Coplanar and Colinear checks assuming a given geometrical tolerance.")]
        [Input("matrix", "The matrix to calculate the numerical tolerance for.")]
        [Input("tolerance", "Distance tolerance as a baseline for the row echelon form numerical tolerance.")]
        [Output("refTol", "The computed numerical tolerance matching the provided matrix and distance tolerance.")]
        public static double REFTolerance(this double[,] matrix, double tolerance = Tolerance.Distance)
        {
            int d1 = matrix.GetLength(0);
            int d2 = matrix.GetLength(1);
            double maxRowSum = 0;

            for (int i = 0; i < d1; i++)
            {
                double rowSum = 0;
                for (int j = 0; j < d2; j++)
                {
                    rowSum += Math.Abs(matrix[i, j]);
                }
                maxRowSum = Math.Max(maxRowSum, rowSum);
            }

            double result = tolerance * Math.Max(d1, d2) * maxRowSum;
            if (result >= 1)
                result = 1 - tolerance;

            return result;
        }

        /***************************************************/
    }
}





