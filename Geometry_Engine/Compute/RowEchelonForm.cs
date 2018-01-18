using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double[,] RowEchelonForm(this double[,] imatrix, bool reduced = true)
        {
            // Strongly inspired by https://rosettacode.org/wiki/Reduced_row_echelon_form

            double[,] matrix = (double[,])imatrix.Clone();
            int lead = 0, rowCount = matrix.GetLength(0), columnCount = matrix.GetLength(1);
            for (int r = 0; r < rowCount; r++)
            {
                if (columnCount == lead) break;
                int i = r;
                while (Math.Abs(matrix[i, lead]) < Tolerance.Distance)
                {
                    i++;
                    if (i == rowCount)
                    {
                        i = r;
                        lead++;
                        if (columnCount == lead)
                        {
                            lead--;
                            break;
                        }
                    }
                }
                for (int j = 0; j < columnCount; j++)
                {
                    double temp = matrix[r, j];
                    matrix[r, j] = matrix[i, j];
                    matrix[i, j] = temp;
                }
                double div = matrix[r, lead];
                if (Math.Abs(div) >= Tolerance.Distance)
                    for (int j = 0; j < columnCount; j++) matrix[r, j] /= div;

                int w;
                if (reduced) w = 0;
                else w = r + 1;

                for (; w < rowCount; w++)
                {
                    if (w != r)
                    {
                        double sub = matrix[w, lead];
                        for (int k = 0; k < columnCount; k++) matrix[w, k] -= (sub * matrix[r, k]);
                    }
                }
                lead++;
            }
            return matrix;
        }

        /***************************************************/

        public static int CountNonZeroRows(this double[,] matrix)
        {
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);
            int c = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Math.Abs(matrix[i, j]) >= Tolerance.Distance)
                    {
                        c++;
                        break;
                    }
                }
            }
            return c;
        }
    }
}
