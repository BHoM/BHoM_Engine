﻿using BH.oM.Geometry;
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
            if (matrix.GetLength(0) != 3 || matrix.GetLength(1) != 3) throw new Exception("Only 3x3 symmetric matrix is implemented.");
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

        // Solve Ax^3 + Bx^2 + Cx + D = 0 following http://www.code-kings.com/2013/11/cubic-equation-roots-in-csharp-code.html
        private static double[] RealCubicRoots(double A, double B, double C, double D)
        {
            double f = (3 * C / A - B * B / (A * A)) / 3;
            double g = (2 * Math.Pow(B, 3) / Math.Pow(A, 3) - (9 * B * C) / Math.Pow(A, 2) + 27 * D / A) / 27;
            double h = Math.Pow(g, 2) * 0.25 + Math.Pow(f, 3) / 27;

            if (h <= 0)
            {
                double i = Math.Pow(Math.Pow(g, 2) * 0.25 - h, 0.5);
                double j = Math.Pow(i, 0.333333333333333333333333);
                double k = Math.Acos(-g / (2 * i));
                double l = -j;
                double m = Math.Cos(k / 3);
                double n = Math.Pow(3, 0.5) * Math.Sin(k / 3);
                double p = -B / (3 * A);
                double x = 2 * j * Math.Cos(k / 3) - B / (3 * A);
                double y = l * (m + n) + p;
                double z = l * (m - n) + p;
                return new double[] { x, y, z };
            }
            else return null;
        }

        /***************************************************/
    }
}
