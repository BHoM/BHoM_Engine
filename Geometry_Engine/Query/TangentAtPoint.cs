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

        [Description("Gets the tangent Vector of an Arc at the specified Point.")]
        [Input("curve", "The Arc to get the tangent from.")]
        [Input("point", "The Point to get the tangent at.")]
        [Input("tolerance", "The tolerance for finding the point on the curve.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified Point, or null if the Point is not on the curve.")]
        public static Vector TangentAtPoint(this Arc curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        [Description("Gets the tangent Vector of a Circle at the specified Point.")]
        [Input("curve", "The Circle to get the tangent from.")]
        [Input("point", "The Point to get the tangent at.")]
        [Input("tolerance", "The tolerance for finding the point on the curve.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified Point, or null if the Point is not on the curve.")]
        public static Vector TangentAtPoint(this Circle curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        [Description("Gets the tangent Vector of a Line at the specified Point.")]
        [Input("curve", "The Line to get the tangent from.")]
        [Input("point", "The Point to get the tangent at.")]
        [Input("tolerance", "The tolerance for finding the point on the curve.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified Point, or null if the Point is not on the curve.")]
        public static Vector TangentAtPoint(this Line curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        [Description("Gets the tangent Vector of a PolyCurve at the specified Point.")]
        [Input("curve", "The PolyCurve to get the tangent from.")]
        [Input("point", "The Point to get the tangent at.")]
        [Input("tolerance", "The tolerance for finding the point on the curve.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified Point, or null if the Point is not on the curve.")]
        public static Vector TangentAtPoint(this PolyCurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        [Description("Gets the tangent Vector of a Polyline at the specified Point.")]
        [Input("curve", "The Polyline to get the tangent from.")]
        [Input("point", "The Point to get the tangent at.")]
        [Input("tolerance", "The tolerance for finding the point on the curve.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified Point, or null if the Point is not on the curve.")]
        public static Vector TangentAtPoint(this Polyline curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the tangent Vector of any ICurve at the specified Point.")]
        [Input("curve", "The ICurve to get the tangent from.")]
        [Input("point", "The Point to get the tangent at.")]
        [Input("tolerance", "The tolerance for finding the point on the curve.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified Point, or null if the Point is not on the curve.")]
        public static Vector ITangentAtPoint(this ICurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            return Query.TangentAtPoint(curve as dynamic, point, tolerance);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        [Description("Gets the tangent Vector of any ICurve at the specified Point using dynamic dispatch.")]
        [Input("curve", "The ICurve to get the tangent from.")]
        [Input("point", "The Point to get the tangent at.")]
        [Input("tolerance", "The tolerance for finding the point on the curve.", typeof(Length))]
        [Output("tangent", "The tangent Vector at the specified Point, or null if the Point is not on the curve.")]
        public static Vector TangentAtPoint(this ICurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"TangentAtPoint is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}
