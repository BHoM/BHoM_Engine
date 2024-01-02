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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        public static Vector[] Eigenvectors(this TransformMatrix matrix, double tolerance = Tolerance.Distance)
        {
            return matrix.Matrix.Eigenvectors(tolerance);
        }


        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static Vector[] Eigenvectors(this double[,] matrix, double tolerance = Tolerance.Distance)
        {
            double[] eigenvalues = matrix.Eigenvalues(tolerance);
            if (eigenvalues == null)
                return null;

            double a = matrix[0, 0];
            double b = matrix[0, 1];
            double c = matrix[0, 2];
            double d = matrix[1, 1];
            double e = matrix[1, 2];
            double f = matrix[2, 2];

            double sqTol = tolerance * tolerance;
            Vector[] result = new Vector[6];
            for (int i = 0; i < 3; i++)
            {
                double k = eigenvalues[i];
                double[,] equations = {{ a - k, b, c },
                                       { b, d - k, e },
                                       { c, e, f - k }};

                double REFTolerance = equations.REFTolerance(tolerance);
                double[,] REF = equations.RowEchelonForm(true, REFTolerance);
                
                double y = -REF[1, 2];
                double x = -REF[0, 2] - y * REF[0, 1];

                result[2 * i] = x * x + y * y <= sqTol ? new Vector { X = 0, Y = 0, Z = 1 } : new Vector { X = x, Y = y, Z = 0 }.Normalise();
                result[2 * i + 1] = new Vector { X = x, Y = y, Z = 1 }.Normalise();
            }
            return result;
        }

        /***************************************************/
    }
}





