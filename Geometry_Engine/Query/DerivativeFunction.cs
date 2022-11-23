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

using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.Base;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the partial value derivatives of the B-spline Basis function for t value as normalised parameter.")]
        [Input("knots", "Knot vector defining the basis function.")]
        [Input("i", "Index the function is evaluated at. The value of the function is the sum of this functions value for all values of i.")]
        [Input("n", "Degree of the of the basis function. Affects how many adjacent knots control the value.")]
        [Input("t", "Parameter to evaluate the function at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("k", "Degree of the derivation.")]
        [Output("Value of the function for the specified index. The full value of the function should be a sum of all possible i's.")]
        public static double DerivativeFunction(this List<double> knots, int i, int n, double t, int k = 1)
        {
            t = t < 0 ? 0 : t > 1 ? 1 : t;
            
            double min = knots[n - 1];
            double max = knots[knots.Count - n];
            t = min + (max - min) * t;

            return DerivativeFunctionGlobal(knots, i, n, t, k);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets the partial value derivatives of the B-spline Basis function for t value as global parameter.")]
        private static double DerivativeFunctionGlobal(List<double> knots, int i, int n, double t, int k = 1)
        {
            if (k == 0)
                return BasisFunctionGlobal(knots, i, n, t);

            double result = n * (
                KnotFactor(knots, i, n) * DerivativeFunctionGlobal(knots, i, n - 1, t, k - 1) -
                KnotFactor(knots, i + 1, n) * DerivativeFunctionGlobal(knots, i + 1, n - 1, t, k - 1));

            return result;
        }

        /***************************************************/

        private static double KnotFactor(List<double> knots, int i, int n)
        {
            double sKnot = knots[Math.Max(Math.Min(i, knots.Count - 1), 0)];
            double eKnot = knots[Math.Max(Math.Min(i + n, knots.Count - 1), 0)];

            if (eKnot == sKnot)
                return 0;

            return 1 / (eKnot - sKnot);
        }

        /***************************************************/

        public static List<Vector> DerivativeVectors(this NurbsCurve curve, int nbDers, double t)
        {
            int degree = curve.Degree();
            nbDers = Math.Min(nbDers, degree);


            List<double[]> cw = curve.ControlPoints.Zip(curve.Weights, (p, w) => new double[] { p.X * w, p.Y * w, p.Z * w, w }).ToList();
            List<double[]> cwDers = CurveDerivatives(curve.Knots, degree, cw, nbDers, t);

            List<Vector> aDers = new List<Vector>();
            List<double> wDers = new List<double>();

            for (int i = 0; i < cwDers.Count; i++)
            {
                double[] cwDer = cwDers[i];
                aDers.Add(new Vector { X = cwDer[0], Y = cwDer[1], Z = cwDer[2] });
                wDers.Add(cwDer[3]);
            }

            int[][] binomals = Binomals();

            List<Vector> derivates = new List<Vector>();

            for (int k = 0; k <= nbDers; k++)
            {
                Vector v = aDers[k];
                for (int i = 1; i <= k; i++)
                {
                    v -= binomals[k][i] * wDers[i] * derivates[k - i];
                }
                derivates.Add(v / wDers[0]);
            }

            return derivates;
        }

        /***************************************************/

        public static List<List<Vector>> DerivativeVectors(this NurbsSurface surface, int nbDers, double u, double v)
        {

            List<List<double[]>> ptsArray = new List<List<double[]>>();

            var uvCount = surface.UVCount();

            int uIndexAddtion = KnotSpan(surface.UKnots, surface.UDegree, u) - surface.UDegree + 1;
            int vIndexAddtion = KnotSpan(surface.VKnots, surface.VDegree, v) - surface.VDegree + 1;

            for (int i = 0; i <= surface.UDegree; i++)
            {
                List<double[]> list = new List<double[]>();
                int uIndexFactor = (i + uIndexAddtion) * uvCount[1];
                for (int j = 0; j <= surface.VDegree; j++)
                {
                    int ptIndex = uIndexFactor + j + vIndexAddtion;
                    Point p = surface.ControlPoints[ptIndex];
                    list.Add(new double[] { p.X, p.Y, p.Z });
                }
                ptsArray.Add(list);
            }

            //List<double[]> pts = surface.ControlPoints.Select(x => new double[] { x.X, x.Y, x.Z }).ToList();
            List<List<double[]>> derivatives = SurfaceDerivatives(surface.UKnots, surface.VKnots, surface.UDegree, surface.VDegree, ptsArray, nbDers, u, v);

            return derivatives.Select(x => x.Select(ve => new Vector { X = ve[0], Y = ve[1], Z = ve[2] }).ToList()).ToList();
        }

        /***************************************************/

        public static List<List<double>> DerivativeFunctions(this IReadOnlyList<double> knots, int span, int degree, int numberOfDerivates, double t)
        {

            //int numDers = Math.Min(numberOfDerivates, degree);
            int numDers = numberOfDerivates;
            double[] left = new double[degree + 1];
            double[] right = new double[degree + 1];
            double[,] ndu = new double[degree + 1, degree + 1];
            ndu[0, 0] = 1.0;

            for (int j = 1; j <= degree; j++)
            {
                left[j] = t - knots[span + 1 - j];
                right[j] = knots[span + j] - t;
                double saved = 0.0;
                for (int r = 0; r < j; r++)
                {
                    //Lower triangle
                    ndu[j, r] = right[r + 1] + left[j - r];
                    double temp = ndu[r, j - 1] / ndu[j, r];
                    //Upper triangle
                    ndu[r, j] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }
                ndu[j, j] = saved;
            }

            double[,] ders = new double[numDers + 1, degree + 1];


            for (int j = 0; j <= degree; j++)
            {
                ders[0, j] = ndu[j, degree];
            }

            double[,] a = new double[2, degree + 1];

            for (int r = 0; r <= degree; r++)
            {
                int s1 = 0;
                int s2 = 1;
                a[0, 0] = 1.0;
                for (int k = 1; k <= numDers; k++)
                {
                    double d = 0.0;
                    int rk = r - k;
                    int pk = degree - k;

                    if (r >= k)
                    {
                        a[s2, 0] = a[s1, 0] / ndu[pk + 1, rk];
                        d = a[s2, 0] * ndu[rk, pk];
                    }
                    int j1, j2;

                    if (rk >= -1)
                        j1 = 1;
                    else
                        j1 = -rk;

                    if (r - 1 <= pk)
                        j2 = k - 1;
                    else
                        j2 = degree - r;

                    for (int j = j1; j <= j2; j++)
                    {
                        a[s2, j] = (a[s1, j] - a[s1, j - 1]) / ndu[pk + 1, rk + j];
                        d += a[s2, j] * ndu[rk + j, pk];
                    }

                    if (r <= pk)
                    {
                        a[s2, k] = -a[s1, k - 1] / ndu[pk + 1, r];
                        d += a[s2, k] * ndu[r, pk];
                    }
                    ders[k, r] = d;

                    int jTemp = s1;
                    s1 = s2;
                    s2 = jTemp;
                }
            }
            double mult = degree;
            List<List<double>> dersList = new List<List<double>>();

            for (int k = 1; k <= numDers; k++)
            {
                for (int j = 0; j <= degree; j++)
                {
                    ders[k, j] *= mult;
                }
                mult *= (double)(degree - k);
            }

            for (int k = 0; k <= numDers; k++)
            {
                List<double> list = new List<double>();
                for (int j = 0; j <= degree; j++)
                {
                    list.Add(ders[k, j]);
                }
                dersList.Add(list);
            }
            return dersList;
        }

        /***************************************************/

        private static List<double[]> CurveDerivatives(this IReadOnlyList<double> knots, int degree, List<double[]> pts, int numberOfDers, double t)
        {

            int maxDers = Math.Min(degree, numberOfDers);

            int dim = pts[0].Length;

            int span = KnotSpan(knots, degree, t);
            List<List<double>> nDers = DerivativeFunctions(knots, span, degree, maxDers, t);

            List<double[]> derArrays = new List<double[]>();

            int ptIndexAddtion = span - degree + 1;

            for (int k = 0; k <= maxDers; k++)
            {
                double[] curr = new double[dim];
                for (int j = 0; j <= degree; j++)
                {
                    AddMultiply(curr, pts[ptIndexAddtion + j], nDers[k][j]);
                }
                derArrays.Add(curr);
            }

            //Higher order derivates to zero:
            for (int i = degree+1; i < numberOfDers; i++)
            {
                derArrays.Add(new double[dim]);
            }
            return derArrays;

        }

        /***************************************************/

        private static List<List<double[]>> SurfaceDerivatives(this IReadOnlyList<double> knotsU, IReadOnlyList<double> knotsV, int degreeU, int degreeV, List<List<double[]>> pts, int numberOfDers, double u, double v)
        {

            int dim = pts[0][0].Length;

            int spanU = KnotSpan(knotsU, degreeU, u);
            int spanV = KnotSpan(knotsV, degreeV, v);
            List<List<double>> dersU = DerivativeFunctions(knotsU, spanU, degreeU, numberOfDers, u);
            List<List<double>> dersV = DerivativeFunctions(knotsV, spanV, degreeV, numberOfDers, v);

            List<List<double[]>> derArrays = new List<List<double[]>>();

            //Initialise empty arrays
            for (int i = 0; i <= numberOfDers; i++)
            {
                List<double[]> list = new List<double[]>();
                for (int j = 0; j <= numberOfDers; j++)
                {
                    list.Add(new double[dim]);
                }
                derArrays.Add(list);
            }

            int maxU = Math.Min(numberOfDers, degreeU);
            int maxV = Math.Min(numberOfDers, degreeV);


            int vCount = knotsV.Count - degreeV + 1;
            int uCount = knotsU.Count - degreeU + 1;

            int uIndexAddtion = spanU - degreeU + 1;
            int vIndexAddtion = spanU - degreeU + 1;

            double[][] temp = new double[degreeV + 1][];

            for (int k = 0; k <= maxU; k++)
            {
                for (int s = 0; s <= degreeV; s++)
                {
                    temp[s] = new double[dim];
                    for (int r = 0; r <= degreeU; r++)
                    {
                        AddMultiply(temp[s], pts[r][s], dersU[k][r]);  
                    }
                }

                int dd = Math.Min(numberOfDers - k, maxV);

                for (int l = 0; l <= dd; l++)
                {
                    for (int s = 0; s <= degreeV; s++)
                    {
                        AddMultiply(derArrays[k][l], temp[s], dersV[l][s]);
                    }
                }
            }

            return derArrays;

        }

        /***************************************************/

        private static void AddMultiply(double[] addTo, double[] toAdd, double multi)
        {
            for (int i = 0; i < addTo.Length; i++)
            {
                addTo[i] += toAdd[i] * multi;
            }
        }

        /***************************************************/

        private static int[][] Binomals()
        { 
            if(m_binomals != null)
                return m_binomals;

            int maxDim = 20;

            m_binomals = new int[maxDim][];

            for (int i = 0; i < maxDim; i++)
            {
                m_binomals[i] = new int[i + 1];

                for (int j = 0; j < i+1; j++)
                {
                    if (j == 0)
                        m_binomals[i][j] = 1;
                    else if (j == i)
                        m_binomals[i][j] = 1;
                    else
                        m_binomals[i][j] = m_binomals[i - 1][j - 1] + m_binomals[i - 1][j];
                }
            }

            return m_binomals;
        }


        /***************************************************/

        private static int[][] m_binomals = null;

        /***************************************************/
    }
}





