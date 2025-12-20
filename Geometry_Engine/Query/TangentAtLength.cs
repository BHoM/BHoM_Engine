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

using System;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Gets the tangent Vector of an Arc at the specified length along the curve.")]
        [Input("curve", "The Arc to get the tangent from.")]
        [Input("length", "The length along the curve to get the tangent at.", typeof(Length))]
        [Input("tolerance", "The tolerance for the calculation.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified length.")]
        public static Vector TangentAtLength(this Arc curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }

        /***************************************************/

        [Description("Gets the tangent Vector of a Circle at the specified length along the curve.")]
        [Input("curve", "The Circle to get the tangent from.")]
        [Input("length", "The length along the curve to get the tangent at.", typeof(Length))]
        [Input("tolerance", "The tolerance for the calculation.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified length.")]
        public static Vector TangentAtLength(this Circle curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }

        /***************************************************/

        [Description("Gets the tangent Vector of a Line at the specified length along the curve.")]
        [Input("curve", "The Line to get the tangent from.")]
        [Input("length", "The length along the curve to get the tangent at.", typeof(Length))]
        [Input("tolerance", "The tolerance for the calculation.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified length.")]
        public static Vector TangentAtLength(this Line curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }

        /***************************************************/

        [Description("Gets the tangent Vector of a PolyCurve at the specified length along the curve.")]
        [Input("curve", "The PolyCurve to get the tangent from.")]
        [Input("length", "The length along the curve to get the tangent at.", typeof(Length))]
        [Input("tolerance", "The tolerance for the calculation.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified length.")]
        public static Vector TangentAtLength(this PolyCurve curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }

        /***************************************************/

        [Description("Gets the tangent Vector of a Polyline at the specified length along the curve.")]
        [Input("curve", "The Polyline to get the tangent from.")]
        [Input("length", "The length along the curve to get the tangent at.", typeof(Length))]
        [Input("tolerance", "The tolerance for the calculation.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified length.")]
        public static Vector TangentAtLength(this Polyline curve, double length, double tolerance = Tolerance.Distance)
        {
            double parameter = length / curve.Length();
            return curve.TangentAtParameter(parameter, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the tangent Vector of any ICurve at the specified length along the curve.")]
        [Input("curve", "The ICurve to get the tangent from.")]
        [Input("length", "The length along the curve to get the tangent at.", typeof(Length))]
        [Input("tolerance", "The tolerance for the calculation.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified length.")]
        public static Vector ITangentAtLength(this ICurve curve, double length, double tolerance = Tolerance.Distance)
        {
            return TangentAtLength(curve as dynamic, length, tolerance);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static Vector TangentAtLength(this ICurve curve, double length, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"TangentAtLength is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}
