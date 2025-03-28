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
using System.Collections.Generic;
using System;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns if the curve is polylinear or not, i.e. if the Polycurve is built up of a set of Line segments.")]
        [Input("curve", "The PolyCurve to check if it is polylinear.")]
        [Output("isPolyLinear", "Return true if the curve is polylinear.")]
        public static bool IsPolylinear(this PolyCurve curve)
        {
            foreach (ICurve c in curve.SubParts())
            {
                if (!(c is Line))
                    return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Returns if the curve is polylinear or not. For a Arc this always returns false.")]
        [Input("curve", "The Arc to check if it is polylinear.")]
        [Output("isLinear", "Return true if the curve is polylinear. For a Arc this always return false.")]
        public static bool IsPolylinear(this Arc curve)
        {
            return false;
        }

        /***************************************************/

        [Description("Returns if the curve is polylinear or not. For a Circle this always returns false.")]
        [Input("curve", "The Circle to check if it is polylinear.")]
        [Output("isLinear", "Return true if the curve is polylinear. For a Circle this always return false.")]
        public static bool IsPolylinear(this Circle curve)
        {
            return false;
        }

        /***************************************************/

        [Description("Returns if the curve is polylinear or not. For a Ellipse this always returns false.")]
        [Input("curve", "The Ellipse to check if it is polylinear.")]
        [Output("isLinear", "Return true if the curve is polylinear. For a Ellipse this always return false.")]
        public static bool IsPolylinear(this Ellipse curve)
        {
            //Could argue that the ellipse is polylinear if one of the radii is 0, but given the use of this method,
            //which is to check if the curve is built up of multiple line segments, it makes more sense to simply return false.
            return false; 
        }

        /***************************************************/

        [Description("Returns if the curve is polylinear or not. For a Line this always returns true.")]
        [Input("curve", "The Line to check if it is polylinear.")]
        [Output("isLinear", "Return true if the curve is polylinear. For a Line this always return true.")]
        public static bool IsPolylinear(this Line curve)
        {
            return true;
        }

        /***************************************************/

        [Description("Returns if the curve is polylinear or not. For a Polyline this always returns true.")]
        [Input("curve", "The Polyline to check if it is polylinear.")]
        [Output("isLinear", "Return true if the curve is polylinear. For a Polyline this always return true.")]
        public static bool IsPolylinear(this Polyline curve)
        {
            return true;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Returns if the curve is polylinear or not.")]
        [Input("curve", "The curve to check if it is polylinear.")]
        [Output("isLinear", "Return true if the curve is polylinear.")]
        public static bool IIsPolylinear(this ICurve curve)
        {
            foreach (ICurve c in curve.ISubParts())
            {
                if (!(c is Line))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsPolylinear(this ICurve curve)
        {
            throw new NotImplementedException($"IsPolylinear is not implemented for ICurves of type: {curve.GetType().Name}.");
        }

        /***************************************************/
    }
}




