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

using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Structure.SurfaceProperties;
using System;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this ConstantThickness property)
        {
            return property.IsNull() ? 0 : property.Thickness;
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this Ribbed property)
        {
            return property.IsNull() ? 0 : property.TotalDepth;
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this Waffle property)
        {
            return property.IsNull() ? 0 : Math.Max(property.TotalDepthX, property.TotalDepthY);
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this LoadingPanelProperty property)
        {
            Reflection.Compute.RecordWarning("LoadingPanelProperties do not have a thickness.");
            return 0;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double ITotalThickness(this ISurfaceProperty property)
        {
            return property.IsNull() ? 0 : TotalThickness(property as dynamic);
        }


        /***************************************************/
        /**** Private Methods - Fallbacks               ****/
        /***************************************************/

        private static double TotalThickness(this ISurfaceProperty property)
        {
            Reflection.Compute.RecordError(property.GetType().Name + " does not have an implementation for TotalThickness. Returning NaN.");
            return double.NaN;
        }

        /***************************************************/

    }

}
