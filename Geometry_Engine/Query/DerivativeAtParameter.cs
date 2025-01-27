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

using BH.oM.Geometry;
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

        [Description("Gets the vector which is the k'th derivative of the curve at the point of t, where t is a normalised parameter.")]
        [Input("curve", "Curve to evaluate.")]
        [Input("t", "Parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("k", "Degree of the derivation.")]
        [Output("Vector which is the k'th derivative of the curve at the point of t.")]
        public static Vector DerivativeAtParameter(this NurbsCurve curve, double t, int k)
        {
            if (k == 0)
                return PointAtParameter(curve, t) - new Point();

            int n = curve.Degree();
            if (k > n)
                return new Vector();

            if (k > 2)
            {
                Engine.Base.Compute.RecordError("Binomial not implemented, hence derivatives higher than 2 of the NurbsCurve can't be calculated.");
                return null;
            }

            List<double> knots = curve.Knots;

            Point ak = new Point();
            for (int j = 0; j < curve.Weights.Count; j++)
                ak += curve.ControlPoints[j] * DerivativeFunction(knots, j - 1, n, t, k) * curve.Weights[j];

            Point sum = new Point();
            for (int i = 1; i <= k; i++)
            {
                // binomial thing (k i) for these specific cases k <= 2 && i >= 1 !!!
                double factor = (i * k) % 3;    // k.Choose(k);

                double da = 0;
                for (int j = 0; j < curve.Weights.Count; j++)
                    da += DerivativeFunction(knots, j - 1, n, t, i) * curve.Weights[j];

                sum += factor * da * DerivativeAtParameter(curve, t, k - i);
            }

            double a = 0;
            for (int j = 0; j < curve.Weights.Count; j++)
                a += BasisFunction(knots, j - 1, n, t) * curve.Weights[j];

            return (ak - sum) / a;
        }

        /***************************************************/

        [Description("Gets the vector which is the k'th derivative of the surface at the point of u, v, where u and v are normalised parameters.")]
        [Input("surface", "Surface to evaluate.")]
        [Input("u", "Parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("v", "Parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("k", "Degree of derivative for u.")]
        [Input("l", "Degree of derivative for v.")]
        [Output("Vector which is the k'th derivative of the surface at the point of t.")]
        public static Vector DerivativeAtParameter(this NurbsSurface surface, double u, double v, int k, int l)
        {
            if (k == 0 && l == 0)
            {
                return PointAtParameter(surface, u, v) - new Point();
            }
            if (k > surface.UDegree || l > surface.VDegree)
                return new Vector();

            if (k > 2 || l > 2)
            {
                Engine.Base.Compute.RecordError("Binomial not implemented, hence derivatives higher than 2 of the NurbsCurve can't be calculated.");
                return null;
            }

            List<double> uKnots = surface.UKnots.ToList();
            List<double> vKnots = surface.VKnots.ToList();

            var uv = surface.UVCount();
            Func<int, int, int> ind = (i,j) => i * uv[1] + j;

            Point Akl = new Point();
            for (int i = 0; i < uv[0]; i++)
            {
                for (int j = 0; j < uv[1]; j++)
                {
                    Akl += surface.ControlPoints[ind(i,j)] * 
                           DerivativeFunction(uKnots, i - 1, surface.UDegree, u, k) * 
                           DerivativeFunction(vKnots, j - 1, surface.VDegree, v, l) *
                           surface.Weights[ind(i,j)];
                }
            }

            Point sumUV = new Point();
            for (int i = 1; i <= k; i++)
            {
                // binomial thing (k i) for these specific cases k <= 2 && i >= 1 !!!
                double factor = (i * k) % 3;    // k.Choose(k);

                double dau = 0;
                for (int iu = 0; iu < uv[0]; iu++)
                {
                    for (int jv = 0; jv < uv[1]; jv++)
                    {
                        dau += DerivativeFunction(uKnots, iu - 1, surface.UDegree, u, i) *
                               DerivativeFunction(vKnots, jv - 1, surface.VDegree, v, 0) *
                               surface.Weights[ind(iu, jv)];
                    }
                }

                Vector derAtP = DerivativeAtParameter(surface, u, v, k - i, l);
                sumUV += factor * dau * derAtP;
            }

            for (int j = 1; j <= l; j++)
            {
                // binomial thing (l j) for these specific cases l <= 2 && j >= 1 !!!
                double factor = (j * l) % 3;    // k.Choose(k);

                double dau = 0;
                for (int iu = 0; iu < uv[0]; iu++)
                {
                    for (int jv = 0; jv < uv[1]; jv++)
                    {
                        dau += DerivativeFunction(uKnots, iu - 1, surface.UDegree, u, 0) *
                               DerivativeFunction(vKnots, jv - 1, surface.VDegree, v, j) *
                               surface.Weights[ind(iu, jv)];
                    }
                }

                Vector derAtP = DerivativeAtParameter(surface, u, v, k, l - j);
                sumUV += factor * dau * derAtP;
            }

            for (int i = 1; i <= k; i++)
            {
                // binomial thing (k i) for these specific cases k <= 2 && i >= 1 !!!
                double factorI = (i * k) % 3;    // k.Choose(k);

                Point sum = new Point();

                for (int j = 1; j <= l; j++)
                {
                    // binomial thing (l j) for these specific cases l <= 2 && j >= 1 !!!
                    double factorJ = (j * l) % 3;    // k.Choose(k);

                    double dau = 0;
                    for (int iu = 0; iu < uv[0]; iu++)
                    {
                        for (int jv = 0; jv < uv[1]; jv++)
                        {
                            dau += DerivativeFunction(uKnots, iu - 1, surface.UDegree, u, i) *
                                   DerivativeFunction(vKnots, jv - 1, surface.VDegree, v, j) *
                                   surface.Weights[ind(iu, jv)];
                        }
                    }

                    Vector derAtP = DerivativeAtParameter(surface, u, v, k - i, l - j);
                    sum += factorJ * dau * derAtP;
                }
                sumUV += factorI * sum;
            }

            double w = 0;
            for (int i = 0; i < uv[0]; i++)
            {
                for (int j = 0; j < uv[1]; j++)
                {
                    w += BasisFunction(uKnots, i - 1, surface.UDegree, u) *
                         BasisFunction(vKnots, j - 1, surface.VDegree, v) *
                         surface.Weights[ind(i, j)];
                }
            }

            return (Akl - sumUV) / w;
        }

        /***************************************************/

    }
}






