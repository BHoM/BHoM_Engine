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
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Inserts a knot into a NurbsCurve at a specified parameter location with a given multiplicity. This operation does not change the shape of the curve but increases the number of control points.")]
        [Input("curve", "The NurbsCurve to insert the knot into.")]
        [Input("t", "The parameter location where the knot should be inserted.")]
        [Input("r", "The multiplicity of the knot to insert (number of times to insert the knot).")]
        [Output("curve", "The NurbsCurve with the knot inserted at the specified parameter location.")]
        public static NurbsCurve InsertKnot(this NurbsCurve curve, double t, int r)
        {
            if (curve == null)
                return null;

            Output<List<double[]>, bool> cw_isRational = curve.ControlPoints.ToDoubleArray(curve.Weights);
            List<double[]> cw = cw_isRational.Item1;
            bool isRational = cw_isRational.Item2;
            Output<List<double>, List<double[]>> newKnotAndPts = InsertKnot(curve.Degree(), curve.Knots, cw, t, r);
            List<double[]> newCw = newKnotAndPts.Item2;

            Output<List<Point>, List<double>> ptsAndWeights = newCw.ToPointAndWeight(isRational);

            return new NurbsCurve() { ControlPoints = ptsAndWeights.Item1, Knots = newKnotAndPts.Item1, Weights = ptsAndWeights.Item2 };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Output<List<double>, List<double[]>> InsertKnot(int degree, List<double> knots, List<double[]> ctrlPts, double t, int r)
        {
            int span = Geometry.Query.KnotSpan(knots, degree, t);
            int s = Query.KnotMultiplicity(knots, t);

            if (s >= degree)
            {
                //Max knot multiplicity is the degree. Current knot already has this multiplicity, hence just returning input values
                return new Output<List<double>, List<double[]>> { Item1 = knots, Item2 = ctrlPts };
            }

            //Maximum number of knots to insert
            if ((r + s) > degree)
            {
                r = degree - s;
            }

            return InsertKnot(degree, knots, ctrlPts, t, r, span, s);
        }

        /***************************************************/

        private static Output<List<double>, List<double[]>> InsertKnot(int degree, List<double> knots, List<double[]> ctrlPts, double t, int r, int span, int s)
        {

            //setup the new knot vector
            List<double> newKnots = Enumerable.Repeat(0.0, knots.Count + r).ToList();

            //Unalterned knots before
            for (int i = 0; i <= span; i++)
            {
                newKnots[i] = knots[i];
            }
            //Insert knot
            for (int i = 1; i <= r; i++)
            {
                newKnots[span + i] = t;
            }
            //Add knots after
            for (int i = span + 1; i < knots.Count; i++)
            {
                newKnots[i + r] = knots[i];
            }

            int dim = ctrlPts.First().Length;
            List<double[]> newCtrlPts = Enumerable.Repeat(new double[dim], ctrlPts.Count + r).ToList();

            //Save unaltered points
            for (int i = 0; i <= span - degree + 1; i++)
            {
                newCtrlPts[i] = ctrlPts[i];
            }
            for (int i = span - s + 1; i < ctrlPts.Count; i++)
            {
                newCtrlPts[i + r] = ctrlPts[i];
            }

            List<double[]> temp = new List<double[]>();

            for (int i = 0; i <= degree - s; i++)
            {
                temp.Add(ctrlPts[span - degree + i + 1]);
            }

            int low = 0;

            //Modify controlpoints affected by knot insertion
            for (int j = 1; j <= r; j++)
            {
                low = span - degree + j;

                for (int i = 0; i <= degree - j - s; i++)
                {
                    double ti = knots[low + i];
                    double tnext = knots[i + span + 1];
                    double alpha = (t - ti) / (tnext - ti);
                    temp[i] = Interpolate(temp[i], temp[i + 1], alpha);
                }
                newCtrlPts[low + 1] = temp[0];
                newCtrlPts[span + r - j - s + 1] = temp[degree - j - s];
            }

            for (int i = low + 1; i < span - s; i++)
            {
                newCtrlPts[i + 1] = temp[i - low];
            }

            return new Output<List<double>, List<double[]>> { Item1 = newKnots, Item2 = newCtrlPts };
        }

        /***************************************************/

        [Description("Performs linear interpolation between two double arrays using a parameter value.")]
        [Input("first", "The first array of values.")]
        [Input("second", "The second array of values.")]
        [Input("a", "The interpolation parameter (0 returns first, 1 returns second).")]
        [Output("result", "The interpolated array of values.")]
        private static double[] Interpolate(double[] first, double[] second, double a)
        {
            double[] ret = new double[first.Length];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (1 - a) * first[i] + a * second[i];
            }
            return ret;
        }

        /***************************************************/
    }
}

