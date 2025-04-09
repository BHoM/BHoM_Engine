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

        public static double[] Eigenvalues(this TransformMatrix matrix, double tolerance = Tolerance.Distance)
        {
            return matrix.Matrix.Eigenvalues(tolerance);
        }


        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static double[] Eigenvalues(this double[,] matrix, double tolerance = Tolerance.Distance)
        {
            if (matrix.GetLength(0) != 3 || matrix.GetLength(1) != 3)
                throw new Exception("Only 3x3 symmetric matrix is implemented.");

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Math.Abs(matrix[i, j] - matrix[j, i]) > tolerance) throw new Exception("Only 3x3 symmetric matrix is implemented.");
                }
            }

            double a = matrix[0, 0];
            double b = matrix[0, 1];
            double c = matrix[0, 2];
            double d = matrix[1, 1];
            double e = matrix[1, 2];
            double f = matrix[2, 2];

            double A = 1;
            double B = -(a + d + f);
            double C = a * d + a * f + d * f - b * b - c * c - e * e;
            double D = -(a * d * f + 2 * b * c * e - a * e * e - d * c * c - f * b * b);

            return RealCubicRoots(A, B, C, D);
        }
        
        /***************************************************/

        // Solve ax^3 + bx^2 + cx + d = 0 following http://www.code-kings.com/2013/11/cubic-equation-roots-in-csharp-code.html
        private static double[] RealCubicRoots(double a, double b, double c, double d)
        {
            double f = (3 * c / a - b * b / (a * a)) / 3;
            double g = (2 * Math.Pow(b, 3) / Math.Pow(a, 3) - (9 * b * c) / Math.Pow(a, 2) + 27 * d / a) / 27;
            double h = Math.Pow(g, 2) * 0.25 + Math.Pow(f, 3) / 27;

            if (h <= 0)
            {
                double i = Math.Pow(Math.Pow(g, 2) * 0.25 - h, 0.5);
                double j = Math.Pow(i, 0.333333333333333333333333);
                double k = Math.Acos(-g / (2 * i));
                double l = -j;
                double m = Math.Cos(k / 3);
                double n = Math.Pow(3, 0.5) * Math.Sin(k / 3);
                double p = -b / (3 * a);
                double x = 2 * j * Math.Cos(k / 3) - b / (3 * a);
                double y = l * (m + n) + p;
                double z = l * (m - n) + p;
                return new double[] { x, y, z };
            }
            else
                return null;
        }

        /***************************************************/
    }
}






