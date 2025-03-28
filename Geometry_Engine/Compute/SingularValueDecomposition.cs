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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Performs factorization of a matrix into three matrices known as Sigular Value Decomposition." +
            "\nReturns 3 matrices that represent the formula A = U * S * Vh." +
            "\nImplementation uses Jacobi eigenvalue algorithm.")]
        [Input("matrix", "Matrix to run the factorization against, A component in formula A = U * S * Vh.")]
        [MultiOutput(0, "U", "U component in formula A = U * S * Vh.")]
        [MultiOutput(1, "S", "S component in formula A = U * S * Vh.")]
        [MultiOutput(2, "Vh", "Vh component in formula A = U * S * Vh.")]
        public static Output<double[,], double[], double[,]> SingularValueDecomposition(this double[,] matrix)
        {
            // Rewrite of https://github.com/ampl/gsl/blob/master/linalg/svd.c
            double epsilon = 1e-15;
            double [,] A = (double[,])matrix.Clone();

            int m = A.GetLength(0);
            int n = A.GetLength(1);
            double[,] Q = Identity(n);
            double[] S = new double[n];

            // Initialize the rotation counter and the sweep counter
            int count = 1;
            int sweep = 0;

            double tolerance = 10 * m * epsilon;

            // Always at least 12 sweeps
            int sweepmax = Math.Max(5 * n, 12);

            // Store the column error estimates in S, for use during orthogonalization

            for (int j = 0; j < n; j++)
            {
                double[] cj = A.GetColumn(j);
                double sj = cj.Normalise();
                S[j] = epsilon * sj;
            }

            // Orthogonalize A by plane rotations
            while (count > 0 && sweep < sweepmax)
            {
                // Initialize rotation counter
                count = n * (n - 1) / 2;

                for (int j = 0; j < n - 1; j++)
                {
                    for (int k = j + 1; k < n; k++)
                    {
                        double cosine = 0;
                        double sine = 0;

                        double[] cj = A.GetColumn(j);
                        double[] ck = A.GetColumn(k);

                        double p = 2 * DotProduct(cj, ck);
                        double a = cj.Normalise();
                        double b = ck.Normalise();

                        double q = a * a - b * b;
                        double v = Hypot(p, q);

                        // Test for columns j,k orthogonal or dominant errors 
                        double absErrA = S[j];
                        double absErrB = S[k];

                        bool sorted = (a >= b);
                        bool orthog = (Math.Abs(p) <= tolerance * (a * b));
                        bool noisya = (a < absErrA);
                        bool noisyb = (b < absErrB);

                        if (sorted && (orthog || noisya || noisyb))
                        {
                            count--;
                            continue;
                        }

                        // Calculate rotation angles
                        if (v == 0 || !sorted)
                        {
                            cosine = 0;
                            sine = 1;
                        }
                        else
                        {
                            cosine = Math.Sqrt((v + q) / (2 * v));
                            sine = p / (2 * v * cosine);
                        }

                        // Apply rotation to A (U)
                        for (int i = 0; i < m; i++)
                        {
                            double Aik = A[i, k];
                            double Aij = A[i, j];
                            A[i, j] = Aij * cosine + Aik * sine;
                            A[i, k] = -Aij * sine + Aik * cosine;
                        }

                        S[j] = Math.Abs(cosine) * absErrA + Math.Abs(sine) * absErrB;
                        S[k] = Math.Abs(sine) * absErrA + Math.Abs(cosine) * absErrB;

                        // Apply rotation to Q (Vh)
                        for (int i = 0; i < n; i++)
                        {
                            double Qij = Q[i, j];
                            double Qik = Q[i, k];
                            Q[i, j] = Qij * cosine + Qik * sine;
                            Q[i, k] = -Qij * sine + Qik * cosine;
                        }
                    }
                }

                // Sweep completed
                sweep++;
            }

            // Orthogonalization complete, compute singular values
            double prevNorm = -1;

            for (int j = 0; j < n; j++)
            {
                double[] column = A.GetColumn(j);
                double norm = column.Normalise();

                // Determine if singular value is zero, according to the criteria used in the main loop above
                // (i.e. comparison with norm of previous column)
                if (norm == 0 || prevNorm == 0 || (j > 0 && norm <= tolerance * prevNorm))
                {
                    S[j] = 0;
                    for (int i = 0; i < m; i++)
                    {
                        A[i, j] = 0;
                    }

                    prevNorm = 0;
                }
                else
                {
                    S[j] = norm;
                    for (int i = 0; i < m; i++)
                    {
                        A[i, j] = A[i, j] / norm;
                    }

                    prevNorm = norm;
                }
            }

            if (count > 0)
            {
                BH.Engine.Base.Compute.RecordError("Jacobi algorithm did not converge.");
                return null;
            }

            double[,] Vh = Q.Transpose();

            // Trim the results to the number of rows if needed
            if (m < n)
            {
                A = A.TakeColumns(m);
                S = S.TakeComponents(m);
                Vh = Vh.TakeRows(m);
            }

            return new Output<double[,], double[], double[,]> { Item1 = A, Item2 = S, Item3 = Vh };
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static double Hypot(double x, double y)
        {
            // As described in https://pubs.opengroup.org/onlinepubs/009695399/functions/hypot.html
            double xAbs = Math.Abs(x);
            double yAbs = Math.Abs(y);
            double min, max;

            if (xAbs < yAbs)
            {
                min = xAbs;
                max = yAbs;
            }
            else
            {
                min = yAbs;
                max = xAbs;
            }

            if (min == 0)
                return max;
            else
            {
                double u = min / max;
                return max * Math.Sqrt(1 + u * u);
            }
        }

        /***************************************************/

        private static double[,] Identity(int n)
        {
            double[,] result = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                result[i, i] = 1;
            }

            return result;
        }

        /***************************************************/

        private static double Normalise(this double[] v)
        {
            double sum = 0;
            for (int i = 0; i < v.Length; i++)
            {
                sum += v[i] * v[i];
            }

            return Math.Sqrt(sum);
        }

        /***************************************************/

        private static double DotProduct(double[] v1, double[] v2)
        {
            double sum = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                sum += v1[i] * v2[i];
            }

            return sum;
        }

        /***************************************************/

        private static double[] GetColumn(this double[,] m, int j)
        {
            int rows = m.GetLength(0);
            double[] result = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                result[i] = m[i, j];
            }

            return result;
        }

        /***************************************************/

        private static double[,] TakeColumns(this double[,] src, int n)
        {
            int rows = src.GetLength(0);

            double[,] result = new double[rows, n];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = src[i, j];
                }
            }

            return result;
        }

        /***************************************************/

        private static double[,] TakeRows(this double[,] src, int n)
        {
            int columns = src.GetLength(1);
            double[,] result = new double[n, columns];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < columns; j++)
                    result[i, j] = src[i, j];
            }

            return result;
        }

        /***************************************************/

        private static double[] TakeComponents(this double[] v, int n)
        {
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = v[i];
            }

            return result;
        }

        /***************************************************/
    }
}

