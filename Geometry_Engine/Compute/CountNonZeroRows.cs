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

        [Description("Counts the number of rows in the matrix that contain at least one non-zero value.")]
        [Input("matrix","The matrix to evaluate.")]
        [Input("tolerance", "Tolerance to be used to determine if the value is zero. A value of the matrix that has a magnitude less than this tolerance will be deemed as a zero value.")]
        [Output("count", "The number of non-zero rows of the matrix.")]
        public static int CountNonZeroRows(this double[,] matrix, double tolerance = Tolerance.Distance)
        {
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);
            int c = 0;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Math.Abs(matrix[i, j]) >= tolerance)
                    {
                        c++;
                        break;
                    }
                }
            }

            return c;
        }

        /***************************************************/

    }
}




