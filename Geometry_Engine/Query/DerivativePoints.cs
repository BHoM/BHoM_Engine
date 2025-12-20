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
using BH.oM.Geometry;
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

        [Description("Computes the derivative control points of a NurbsCurve up to a specified order. These are used in the computation of curve derivatives.")]
        [Input("curve", "The NurbsCurve to compute derivative control points for.")]
        [Input("numDers", "The number of derivative orders to compute.")]
        [MultiOutput(0, "derivativePoints", "Lists of derivative control points for each derivative order.")]
        [MultiOutput(1, "derivativeWeights", "Lists of derivative weights for each derivative order.")]
        public static Output<List<List<Point>>, List<List<double>>> DerivativePoints(this NurbsCurve curve, int numDers)
        {
            int degree = curve.Degree();
            Output<List<double[]>, bool> cw_isRat = curve.ControlPoints.ToDoubleArray(curve.Weights);
            List<List<double[]>> cw = DerivativePoints(curve.Knots, cw_isRat.Item1, numDers, degree);
            IEnumerable<Output<List<Point>, List<double>>> toCartAndW = cw.Select(x => x.ToPointAndWeight(cw_isRat.Item2));
            return new Output<List<List<Point>>, List<List<double>>>
            {
                Item1 = toCartAndW.Select(x => x.Item1).ToList(),
                Item2 = toCartAndW.Select(x => x.Item2).ToList(),
            };
        }

        /***************************************************/

        [Description("Computes the derivative control points from knot vector and control points up to a specified order.")]
        [Input("knots", "The knot vector.")]
        [Input("cw", "The control points in homogeneous coordinates.")]
        [Input("numDers", "The number of derivative orders to compute.")]
        [Input("degree", "The degree of the curve.")]
        [Output("derivativePoints", "Lists of derivative control points for each derivative order.")]
        public static List<List<double[]>> DerivativePoints(this IList<double> knots, List<double[]> cw, int numDers, int degree)
        {
            int n = cw.Count;
            List<List<double[]>> derivativePts = new List<List<double[]>>();

            derivativePts.Add(cw);  //0th derivative is the initial points

            for (int k = 1; k <= numDers; k++)
            {
                double tmp = (degree - k + 1);
                List<double[]> tempPts = new List<double[]>();
                for (int i = 0; i < n - k; i++)
                {
                    double scaleFac = tmp / (knots[i + degree] - knots[i + k - 1]);
                    tempPts.Add(ScaleSubtract(derivativePts[k - 1][i + 1], derivativePts[k - 1][i], scaleFac));
                }

                derivativePts.Add(tempPts);
            }

            return derivativePts;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double[] ScaleSubtract(double[] a, double[] b, double sFac)
        {
            double[] temp = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
            {
                temp[i] = sFac * (a[i] - b[i]);
            }
            return temp;
        }

        /***************************************************/
    }
}

