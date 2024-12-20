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
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Curvature of the NurbsCurve at the parameter t, where t is a normalised parameter. Points towards the center of a fitted circle at the parameter with a magnitude equal to the inverse of the circles radius.")]
        [Input("curve", "Curve to evaluate.")]
        [Input("t", "Parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Output("curvature", "Curvature of the NurbsCurve at the parameter t.")]
        public static Vector CurvatureAtParameter(this NurbsCurve curve, double t)
        {
            // Note for those questioning this back and forth. The second derivative would be the curvature if the first derivative had a constant magnitude.
            // There is however no such luck. But ||C'xC''|| / (||C'||)^3 is the magnitude regardless. 
            // So we set it to that magnitude while correcting the direction through a repeated crossproduct.

            Vector dC = curve.DerivativeAtParameter(t, 1);
            Vector ddC = curve.DerivativeAtParameter(t, 2);

            Vector orth = dC.CrossProduct(ddC);

            double denominator = dC.Length();
            denominator = denominator * denominator * denominator;

            orth /= denominator;

            return orth.CrossProduct(dC.Normalise());
        }

        /***************************************************/

        [Description("Evaluates the Gaussian and Mean curvature at the parameter.")]
        [Input("surface", "Surface to evaluate.")]
        [Input("u", "Parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("v", "Parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [MultiOutput(0, "gauss", "Gaussian curvature at the parameter.")]
        [MultiOutput(1, "mean", "Mean curvature at the parameter.")]
        public static Output<double, double> CurvatureAtParameter(this NurbsSurface surface, double u, double v)
        {
            var result = PrincipalCurvatureAtParameter(surface, u, v);

            return new Output<double, double>()
            {
                Item1 = result.Item1 * result.Item2,
                Item2 = (result.Item1 + result.Item2) / 2
            };
        }

        /***************************************************/

    }
}






