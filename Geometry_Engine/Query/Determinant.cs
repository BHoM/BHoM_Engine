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

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****         Determinant of a matrix           ****/
        /***************************************************/

        // Strongly inspired by https://www.geeksforgeeks.org/determinant-of-a-matrix/

        public static double Determinant(this TransformMatrix matrix)
        {
            return Determinant(matrix.Matrix, 0);
        }

        /***************************************************/

        public static double Determinant(this double[,] mat)
        {
            return Determinant(mat, 0);
        }


        /***************************************************/
        /****             Private methods               ****/
        /***************************************************/

        private static double Determinant(double[,] mat, int n)
        {
            int N = mat.GetLength(0);
            if (N != mat.GetLength(1))
                throw new Exception("The matrix needs to be square to find its determinant.");

            // Initial step of the recursive code
            if (n == 0)
                n = N;

            double D = 0; // Initialize result

            // If a single item matrix
            if (n == 1)
                return mat[0, 0];

            // To store cofactors
            double[,] temp = new double[N, N];

            // To store sign multiplier
            double sign = 1;

            // Iterate for each element
            // of first row
            for (int f = 0; f < n; f++)
            {
                // Getting Cofactor of mat[0][f]
                GetCofactor(mat, temp, 0, f, n);
                D += sign * mat[0, f] * Determinant(temp, n - 1);

                // terms are to be added with 
                // alternate sign
                sign = -sign;
            }

            return D;
        }

        /***************************************************/

        private static void GetCofactor(double[,] mat, double[,] temp, int p, int q, int n)
        {
            int i = 0, j = 0;

            // Looping for each element of 
            // the matrix
            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < n; col++)
                {

                    // Copying into temporary matrix 
                    // only those element which are 
                    // not in given row and column
                    if (row != p && col != q)
                    {
                        temp[i, j++] = mat[row, col];

                        // Row is filled, so increase 
                        // row index and reset col 
                        //index
                        if (j == n - 1)
                        {
                            j = 0;
                            i++;
                        }
                    }
                }
            }
        }

        /***************************************************/
    }
}






