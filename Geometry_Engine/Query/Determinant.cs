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
        /****             Private methods               ****/
        /***************************************************/

        private static double Determinant(this double[,] mat)
        {
            return Determinant(mat, 0);
        }

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
                getCofactor(mat, temp, 0, f, n);
                D += sign * mat[0, f] * Determinant(temp, n - 1);

                // terms are to be added with 
                // alternate sign
                sign = -sign;
            }

            return D;
        }

        /***************************************************/

        private static void getCofactor(double[,] mat, double[,] temp, int p, int q, int n)
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
