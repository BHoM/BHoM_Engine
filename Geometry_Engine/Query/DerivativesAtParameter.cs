/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
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

        [Description("Gets the vectors that are the derivatives of the curve at the point of t, where t is a normalised parameter. List index correspond to the derivative, i.e. index 0 is no derivative (position) and index 1 is the 1st derivative.")]
        [Input("curve", "Curve to evaluate.")]
        [Input("t", "Parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("numberOfDerivates", "Number of derivatives to evaluate. Will only return non-vanishing derivatives, this is, maximum derivative is the degree of the curve.")]
        [Output("derivatives", "List containing the derivatives where the index correspond to the level of derivation, i.e. index 0 is no derivative (position) and index 1 is the 1st derivative.")]
        public static List<Vector> DerivativesAtParameter(this NurbsCurve curve, int numberOfDerivates, double t, bool normalisedParameter)
        {
            if (curve == null)
                return new List<Vector>();

            int degree = curve.Degree();

            if (normalisedParameter)
                t = Convert.ToKnotDomain(t, curve.Knots, degree);

            numberOfDerivates = Math.Min(numberOfDerivates, degree);

            //Construct list of homogenous controlpoints as double[] where the the first three values corespond to the coordinates sclaed by the weight and 4th value correspond to the weights
            List<double[]> cw = curve.ControlPoints.Zip(curve.Weights, (p, w) => new double[] { p.X * w, p.Y * w, p.Z * w, w }).ToList();
            
            //Compute the derivatives for the homogenous coordinates
            List<double[]> cwDers = CurveDerivatives(curve.Knots, degree, cw, numberOfDerivates, t);

            //Split into Aders containing the still scaled derivative vectors and wDers containing the derivatives of the weights
            List<Vector> aDers = new List<Vector>();
            List<double> wDers = new List<double>();

            for (int i = 0; i < cwDers.Count; i++)
            {
                double[] cwDer = cwDers[i];
                aDers.Add(new Vector { X = cwDer[0], Y = cwDer[1], Z = cwDer[2] });
                wDers.Add(cwDer[3]);
            }

            List<Vector> derivates = new List<Vector>();

            //Compute the Derivatives in Cartesian coordinates
            for (int k = 0; k <= numberOfDerivates; k++)
            {
                Vector v = aDers[k];
                for (int i = 1; i <= k; i++)
                {
                    v -= Binomal(k, i) * wDers[i] * derivates[k - i];
                }
                derivates.Add(v / wDers[0]);
            }

            return derivates;
        }


        [Description("Gets the vectors that are the derivatives of the surface at the point of u, v, where u and v are normalised parameters.\n"
                    +"Outer list index correspond to the derivative in u direction and inner list index to the derivative in v direction, i.e. index 0,0 is no derivative (position) and index 1,0 is the 1st derivative in u direction.")]
        [Input("surface", "Surface to evaluate.")]
        [Input("u", "Parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("v", "Parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("numberOfDerivates", "Maximum total derivation degree to evaluate. value of 2 will evaluate up to [2,0], [1,1] and [0,2]. Will only return non-vanishing derivatives, this is, maximum derivative is the degree of the curve.")]
        [Output("derivatives", "Nested list of derivative vectors where the outer list index correspond to the derivative in u direction and inner list index to the derivative in v direction, i.e. index 0,0 is no derivative (position) and index 1,0 is the 1st derivative in u direction on no derivative in the v direction.")]
        public static List<List<Vector>> DerivativesAtParameter(this NurbsSurface surface, int numberOfDerivates, double u, double v, bool normalisedParameter)
        {
            if (surface == null)
                return new List<List<Vector>>();

            if (normalisedParameter)
            {
                u = Convert.ToKnotDomain(u, surface.UKnots, surface.UDegree);
                v = Convert.ToKnotDomain(v, surface.VKnots, surface.VDegree);
            }

            List<List<double[]>> ptsArray = new List<List<double[]>>();

            var uvCount = surface.UVCount();

            int uIndexAddtion = KnotSpan(surface.UKnots, surface.UDegree, u) - surface.UDegree + 1;
            int vIndexAddtion = KnotSpan(surface.VKnots, surface.VDegree, v) - surface.VDegree + 1;

            //Construct list of list of relevant controlpoints for the parameters in homogenous coordinates as double[] where the the first three values corespond to the coordinates sclaed by the weight and 4th value correspond to the weights
            for (int i = 0; i <= surface.UDegree; i++)
            {
                List<double[]> list = new List<double[]>();
                int uIndexFactor = (i + uIndexAddtion) * uvCount[1];
                for (int j = 0; j <= surface.VDegree; j++)
                {
                    int ptIndex = uIndexFactor + j + vIndexAddtion;
                    Point p = surface.ControlPoints[ptIndex];
                    double w = surface.Weights[ptIndex];
                    list.Add(new double[] { p.X * w, p.Y * w, p.Z * w, w });
                }
                ptsArray.Add(list);
            }

            //Compute the derivatives in homogenous coordinates
            List<List<double[]>> derivatives = SurfaceDerivatives(surface.UKnots, surface.VKnots, surface.UDegree, surface.VDegree, ptsArray, numberOfDerivates, u, v);

            //Split into Aders containing the still scaled derivative vectors and wDers containing the derivatives of the weights
            List<List<Vector>> aDers = new List<List<Vector>>();
            List<List<double>> wDers = new List<List<double>>();

            for (int i = 0; i < derivatives.Count; i++)
            {
                List<Vector> aRow = new List<Vector>();
                List<double> wRow = new List<double>();
                for (int j = 0; j < derivatives[i].Count; j++)
                {
                    aRow.Add(new Vector { X = derivatives[i][j][0], Y = derivatives[i][j][1], Z = derivatives[i][j][2] });
                    wRow.Add(derivatives[i][j][3]);
                }
                aDers.Add(aRow);
                wDers.Add(wRow);
            }

            //Vector[,] surfaceDerivatives = new Vector[numberOfDerivates + 1, numberOfDerivates + 1];
            List<List<Vector>> surfaceDerivatives = new List<List<Vector>>();

            //Compute derivatives in cartesian coordinates
            for (int k = 0; k <= numberOfDerivates; k++)
            {
                surfaceDerivatives.Add(new List<Vector>());
                for (int l = 0; l <= numberOfDerivates - k; l++)
                {
                    if (k > surface.UDegree || l > surface.VDegree) //If level of derivation exceeds the degree, add 0 vector
                    {
                        surfaceDerivatives[k].Add(new Vector());
                        continue;
                    }
                    Vector der = aDers[k][l];

                    for (int j = 1; j <= l; j++)
                    {
                        der -= Binomal(l, j) * wDers[0][j] * surfaceDerivatives[k][l - j];
                    }

                    for (int i = 1; i <= k; i++)
                    {
                        der -= Binomal(k, i) * wDers[i][0] * surfaceDerivatives[k - i][l];

                        Vector temp = new Vector();
                        for (int j = 1; j <= l; j++)
                        {
                            temp += Binomal(l, j) * wDers[i][j] * surfaceDerivatives[k - i][l - j];
                        }

                        der -= Binomal(k, i) * temp;
                    }

                    surfaceDerivatives[k].Add(der / wDers[0][0]);
                }
            }

            return surfaceDerivatives;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Computes the non vanishing Curve derivatives. Method is c# implementation of method found in the Nurbs Book.")]
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
            for (int i = degree + 1; i < numberOfDers; i++)
            {
                derArrays.Add(new double[dim]);
            }
            return derArrays;

        }

        /***************************************************/

        [Description("Computes the non vanishing Surface derivatives. Method is C# implementation of method found in the Nurbs Book.")]
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


            //int vCount = knotsV.Count - degreeV + 1;
            //int uCount = knotsU.Count - degreeU + 1;

            //int uIndexAddtion = spanU - degreeU + 1;
            //int vIndexAddtion = spanU - degreeU + 1;

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

        [Description("Utility method to add and scaled factor of values in one array to another.")]
        private static void AddMultiply(double[] addTo, double[] toAdd, double multi)
        {
            for (int i = 0; i < addTo.Length; i++)
            {
                addTo[i] += toAdd[i] * multi;
            }
        }

        /***************************************************/

        
    }

}

