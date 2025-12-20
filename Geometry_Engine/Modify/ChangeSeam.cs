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

        [Description("Changes the seam of a closed NurbsCurve to a specified parameter location. The curve must be closed or periodic for this operation to succeed.")]
        [Input("curve", "The NurbsCurve to change the seam of. Must be closed or periodic.")]
        [Input("t", "The parameter at which to place the new seam.")]
        [Input("normalisedParameter", "If true, the parameter t is assumed to be normalised between 0 and 1. If false, t is in the curve's knot domain.")]
        [Input("tolerance", "The tolerance used for geometric calculations.")]
        [Output("curve", "The NurbsCurve with the seam changed to the specified parameter location.")]
        public static NurbsCurve ChangeSeam(this NurbsCurve curve, double t, bool normalisedParameter = true, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
                return null;

            int degree = curve.Degree();

            if (normalisedParameter)
                t = Convert.ToKnotDomain(t, curve.Knots, degree);

            double[] domain = curve.Knots.Domain(degree);
            if (t <= domain[0] || t >= domain[1])
                return curve;

            bool isPeriodic = curve.IsPeriodic();
            if (!isPeriodic)
            {
                if (curve.ControlPoints.First().SquareDistance(curve.ControlPoints.Last()) > tolerance * tolerance)
                {
                    BH.Engine.Base.Compute.RecordWarning("Can only change the seam of closed curves. Input curve returned.");
                    return curve;
                }
            }

            List<double[]> cw = curve.ControlPoints.Zip(curve.Weights, (p, w) => new double[] { p.X * w, p.Y * w, p.Z * w, w }).ToList();
            List<double> knots = curve.Knots;

            if (isPeriodic)
            {
                Output<List<double[]>, List<double>> clamped = EnsureClamped(cw, knots, degree);
                cw = clamped.Item1;
                knots = clamped.Item2;
            }

            Output<List<double>, List<double[]>> inserted = InsertKnot(degree, knots, cw, t, degree);

            List<double[]> insertedCw = inserted.Item2;
            List<double> insertedKnots = inserted.Item1;

            List<Point> newPts = new List<Point>();
            List<double> newWeights = new List<double>();
            List<double> newKnots = new List<double>();

            int shiftFactor = insertedKnots.KnotSpan(degree, t) - degree + 1;
            double maxT = knots[knots.Count - degree];

            for (int i = 0; i < insertedCw.Count - 1; i++)
            {
                double knotAdditionFactor = 0;
                int insertedIndex = (i + shiftFactor);
                if (insertedIndex > insertedCw.Count - 1)
                {
                    insertedIndex %= (insertedCw.Count - 1); //Shift around second last, as last is to be removed
                    knotAdditionFactor = maxT;
                }

                double[] ctrlPt = insertedCw[insertedIndex];
                double w = ctrlPt[3];
                newPts.Add(new Point() { X = ctrlPt[0] / w, Y = ctrlPt[1] / w, Z = ctrlPt[2] / w });
                newWeights.Add(w);
                newKnots.Add(insertedKnots[insertedIndex] + knotAdditionFactor);
            }

            //Add start point as end point
            newPts.Add(newPts[0]);
            newWeights.Add(newWeights[0]);

            //Add the first knot shifted by max factor degree times
            for (int i = 0; i < degree; i++)
            {
                newKnots.Add(newKnots[i] + maxT);
            }

            return new NurbsCurve { ControlPoints = newPts, Knots = newKnots, Weights = newWeights };
        }


        /***************************************************/

    }
}

