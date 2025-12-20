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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes Gauss-Legendre quadrature points and weights for numerical integration over the knot intervals of a NURBS curve or surface.")]
        [Input("knots", "The knot vector.")]
        [Input("degree", "The degree of the curve or surface.")]
        [Input("level", "The number of Gauss points per knot interval.")]
        [MultiOutput(0, "values", "The parameter values for Gauss quadrature.")]
        [MultiOutput(1, "weights", "The weights for Gauss quadrature.")]
        public static Output<List<double>, List<double>> GaussPairs(this IList<double> knots, int degree, int level)
        {
            List<double> knotIntervals = knots.KnotIntervals(degree);

            List<double> values = new List<double>();
            List<double> weights = new List<double>();
            double min = knots[degree - 1];
            double max = knots[knots.Count - degree];
            double scaleFactor = 1 / (max - min);

            for (int i = 0; i < knotIntervals.Count - 1; i++)
            {
                Output<List<double>, List<double>> intervalPairs = GaussPairs(level, knotIntervals[i], knotIntervals[i + 1], 1.0);
                values.AddRange(intervalPairs.Item1);
                weights.AddRange(intervalPairs.Item2);
            }

            return new Output<List<double>, List<double>>() { Item1 = values, Item2 = weights };
        }

        /***************************************************/

        [Description("Computes Gauss-Legendre quadrature points and weights for numerical integration over a specified interval.")]
        [Input("n", "The number of Gauss points to compute.")]
        [Input("a", "The start of the integration interval.")]
        [Input("b", "The end of the integration interval.")]
        [Input("weightScale", "Scale factor for the weights.")]
        [MultiOutput(0, "values", "The parameter values for Gauss quadrature.")]
        [MultiOutput(1, "weights", "The weights for Gauss quadrature.")]
        public static Output<List<double>, List<double>> GaussPairs(int n, double a = -1, double b = 1, double weightScale = 1.0)
        {
            if (n < 1)
                return new Output<List<double>, List<double>>();
            if (n <= 100)
                return PreComputedGaussPairs(a, b, n, weightScale);
            else
                return ComputeGaussPairs(a, b, n, weightScale);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Output<List<double>, List<double>> PreComputedGaussPairs(double a, double b, int n, double weightScale)
        {
            lock (m_gaussCalcLock)
            {
                if (m_GaussValues == null)
                {
                    m_GaussValues = new List<List<double>>();
                    m_GaussWeights = new List<List<double>>();

                    for (int i = 1; i <= 100; i++)
                    {
                        Output<List<double>, List<double>> computedPairs = ComputeGaussPairs(0, 1, i, 1.0);
                        m_GaussValues.Add(computedPairs.Item1);
                        m_GaussWeights.Add(computedPairs.Item2);
                    }
                }
            }

            if (a == 0 && b == 1)
                return new Output<List<double>, List<double>>() { Item1 = m_GaussValues[n - 1], Item2 = m_GaussWeights[n - 1] };
            else
                return RescaleValuesAndWeights(a, b, m_GaussValues[n - 1], m_GaussWeights[n - 1], weightScale);


        }

        /***************************************************/

        private static Output<List<double>, List<double>> RescaleValuesAndWeights(double a, double b, List<double> values, List<double> weights, double weightScale)
        {
            List<double> scaledValues = new List<double>();
            List<double> scaledWeights = new List<double>();

            double diff = b - a;

            for (int i = 0; i < values.Count; i++)
            {
                scaledValues.Add(a + values[i] * diff);
                scaledWeights.Add(weights[i] * diff * weightScale);
            }

            return new Output<List<double>, List<double>> { Item1 = scaledValues, Item2 = scaledWeights };
        }

        /***************************************************/

        private static Output<List<double>, List<double>> ComputeGaussPairs(double a, double b, int n, double weightScale)
        {
            int m = (n + 1) / 2;
            double xAve = (b + a) / 2;
            double xDiff = (b - a) / 2;

            double[] x = new double[n];
            double[] w = new double[n];

            double tol = 1e-15;

            for (int i = 0; i < m; i++)
            {
                double z = Math.Cos(Math.PI * ((double)(i + 1) - 0.25) / (double)(n + 0.5));
                double z1;
                double pp;
                do
                {
                    double p1 = 1.0;
                    double p2 = 0.0;

                    for (int j = 0; j < n; j++)
                    {
                        double p3 = p2;
                        p2 = p1;
                        p1 = ((2 * (j + 1) - 1.0) * z * p2 - j * p3) / (double)(j + 1.0);
                    }

                    pp = n * (z * p1 - p2) / (z * z - 1.0);
                    z1 = z;
                    z = z1 - p1 / pp;

                } while (Math.Abs(z - z1) > tol);
                x[i] = xAve - xDiff * z;
                x[n - 1 - i] = xAve + xDiff * z;
                w[i] = 2.0 * xDiff / ((1.0 - z * z) * pp * pp) * weightScale;
                w[n - 1 - i] = w[i] * weightScale;
            }
            return new Output<List<double>, List<double>> { Item1 = x.ToList(), Item2 = w.ToList() };
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<List<double>> m_GaussValues = null;
        private static List<List<double>> m_GaussWeights = null;
        private static object m_gaussCalcLock = new object();

        /***************************************************/
    }
}
