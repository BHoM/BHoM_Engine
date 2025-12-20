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

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Ensures that a NurbsCurve has clamped knot vectors, meaning the first and last knots have multiplicity equal to the degree.")]
        [Input("curve", "The NurbsCurve to ensure is clamped.")]
        [Output("curve", "The clamped NurbsCurve with knot multiplicities equal to the degree at the start and end.")]
        public static NurbsCurve EnsureClamped(this NurbsCurve curve)
        {
            Output<List<double[]>, bool> toControlPoints = Convert.ToDoubleArray(curve.ControlPoints, curve.Weights);
            List<double[]> cw = toControlPoints.Item1;
            List<double> knots = curve.Knots;

            int degree = curve.Degree();

            Output<List<double[]>, List<double>> clamped = EnsureClamped(cw, knots, degree);

            Output<List<Point>, List<double>> ptsAndWeights = Convert.ToPointAndWeight(clamped.Item1, toControlPoints.Item2);
            return new NurbsCurve { ControlPoints = ptsAndWeights.Item1, Knots = clamped.Item2, Weights = ptsAndWeights.Item2 };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Output<List<double[]>, List<double>> EnsureClamped(List<double[]> cw, List<double> knots, int degree)
        {
            double[] domain = knots.Domain(degree);
            if (knots.KnotMultiplicity(domain[0]) >= degree && knots.KnotMultiplicity(domain[1]) >= degree) //Already clamped
                return new Output<List<double[]>, List<double>> { Item1 = cw, Item2 = knots };


            int startSpan = degree - 1;
            Output<List<double>, List<double[]>> startInsert = InsertKnot(degree, knots, cw, knots[startSpan], degree, startSpan, 0);
            int endSpan = startInsert.Item1.Count - degree - 1;
            Output<List<double>, List<double[]>> endInsert = InsertKnot(degree, startInsert.Item1, startInsert.Item2, startInsert.Item1[endSpan + 1], degree, endSpan, 0);

            List<double[]> newPts = new List<double[]>();
            List<double> newKnots = new List<double>();

            for (int i = degree; i < endInsert.Item1.Count - degree; i++)
            {
                newKnots.Add(endInsert.Item1[i]);
            }

            for (int i = degree; i < endInsert.Item2.Count - degree; i++)
            {
                newPts.Add(endInsert.Item2[i]);
            }
            return new Output<List<double[]>, List<double>> { Item1 = newPts, Item2 = newKnots };
        }

        /***************************************************/
    }
}

