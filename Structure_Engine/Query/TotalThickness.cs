/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
            Base.Compute.RecordWarning("LoadingPanelProperties do not have a thickness.");
            return 0;
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this Layered property)
        {
            if (property.IsNull())
                return 0;

            if (property.Layers.Any(x => x.Material == null))
                Base.Compute.RecordWarning("At least one Material in a Layered surface property was null. Thickness has been reported including this layer as void space.");

            return property.Layers.Sum(x => x.Thickness);
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this SlabOnDeck property)
        {
            if (property.IsNull())
                return 0;

            //Unlike AverageThickness, this assumes that the thickness of the deck is zero, and/or that the deck height is measured
            //from outside to outside, i.e. top of top flute to bottom of bottom flute.
            //This results in a deck with 3" concrete on 3" deck with a thickness of 6" exactly, which is expected.
            return property.SlabThickness + property.DeckHeight;
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this CorrugatedDeck property)
        {
            if (property.IsNull())
                return 0;

            return property.Height;
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this OneDirectionalVoided property)
        {
            if (property.IsNull())
                return 0;

            return property.TopThickness;
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this BiDirectionalVoided property)
        {
            if (property.IsNull())
                return 0;

            return property.TopThickness;
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this HollowCore property)
        {
            if (property.IsNull())
                return 0;

            return property.Thickness;
        }

        /***************************************************/

        [Description("Gets the total thickness of the surface property.")]
        [Input("property", "The property to evaluate.")]
        [Output("TotalThickness", "The total thickness, including any ribs or waffling.", typeof(Length))]
        public static double TotalThickness(this ToppedSlab property)
        {
            if (property.IsNull())
                return 0;

            return property.BaseProperty.ITotalThickness() + property.ToppingThickness;
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
            Base.Compute.RecordError(property.GetType().Name + " does not have an implementation for TotalThickness. Returning NaN.");
            return double.NaN;
        }

        /***************************************************/

    }

}

