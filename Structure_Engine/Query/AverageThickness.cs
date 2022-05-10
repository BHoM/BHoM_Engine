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

        [Description("Gets the average thickness of the property as if it was applied to an infinite plane.")]
        [Input("property", "The property to evaluate the average thickness of.")]
        [Output("averageThickness", "the average thickness of the property as if it was applied to an infinite plane.", typeof(Length))]
        public static double AverageThickness(this ConstantThickness property)
        {
            return property.IsNull() ? 0 : property.Thickness;
        }

        /***************************************************/

        [Description("Gets the average thickness of the property as if it was applied to an infinite plane.")]
        [Input("property", "The property to evaluate the average thickness of.")]
        [Output("averageThickness", "the average thickness of the property as if it was applied to an infinite plane.", typeof(Length))]
        public static double AverageThickness(this Ribbed property)
        {
            if (property.IsNull())
                return 0;

            if (property.StemWidth == 0 || property.Spacing < property.StemWidth ||
                property.TotalDepth < property.Thickness)
            {
                Base.Compute.RecordError("Invalid Ribbed slab, returning null.");
                return double.NaN;
            }
            double avrageThicknessLower = (property.TotalDepth - property.Thickness) * (property.StemWidth / property.Spacing);
            return property.Thickness + avrageThicknessLower;
        }

        /***************************************************/

        [Description("Gets the average thickness of the property as if it was applied to an infinite plane.")]
        [Input("property", "The property to evaluate the average thickness of.")]
        [Output("averageThickness", "the average thickness of the property as if it was applied to an infinite plane.", typeof(Length))]
        public static double AverageThickness(this Waffle property)
        {
            if (property.IsNull())
                return 0;

            if (property.StemWidthX == 0 || property.SpacingX < property.StemWidthX ||
                property.StemWidthY == 0 || property.SpacingY < property.StemWidthY ||
                property.TotalDepthX < property.Thickness || property.TotalDepthY < property.Thickness)
            {
                Base.Compute.RecordError("Invalid Waffle slab, returning null.");
                return double.NaN;
            }

            double avrageThicknessLowerX = (property.TotalDepthX - property.Thickness) * (property.StemWidthX / property.SpacingX);
            double avrageThicknessLowerY = (property.TotalDepthY - property.Thickness) * (property.StemWidthY / property.SpacingY);

            double theExtraVolume = Math.Min(property.TotalDepthX - property.Thickness, property.TotalDepthY - property.Thickness) *
                                    (property.StemWidthX * property.StemWidthY) / (property.SpacingX * property.SpacingY);

            return property.Thickness + avrageThicknessLowerX + avrageThicknessLowerY - theExtraVolume;
        }

        /***************************************************/

        [Description("Gets the average thickness of the property as if it was applied to an infinite plane.")]
        [Input("property", "The property to evaluate the average thickness of.")]
        [Output("averageThickness", "the average thickness of the property as if it was applied to an infinite plane.", typeof(Length))]
        public static double AverageThickness(this LoadingPanelProperty property)
        {
            Base.Compute.RecordWarning("Structural IAreaElements are defined without volume.");
            return 0;
        }

        /***************************************************/

        [Description("Gets the average thickness of the property as if it was applied to an infinite plane.")]
        [Input("property", "The property to evaluate the average thickness of.")]
        [Output("averageThickness", "the average thickness of the property as if it was applied to an infinite plane.", typeof(Length))]
        public static double AverageThickness(this Layered property)
        {
            if (property.IsNull())
                return 0;

            if (property.Layers.Any(x => x.Material == null))
                Base.Compute.RecordWarning("At least one Material in a Layered surface property was null. Thickness has been reported including this layer as void space.");

            return property.Layers.Sum(x => x.Thickness);
        }

        /***************************************************/

        [Description("Gets the average thickness of the property as if it was applied to an infinite plane.")]
        [Input("property", "The property to evaluate the average thickness of.")]
        [Output("averageThickness", "the average thickness of the property as if it was applied to an infinite plane.", typeof(Length))]
        public static double AverageThickness(this SlabOnDeck property)
        {
            if (property.IsNull())
                return 0;

            double t = property.DeckTopWidth;
            double b = property.DeckBottomWidth;
            double h = property.DeckHeight;
            double s = property.DeckSpacing;

            return property.Thickness + h * (b + (s - (b + t)) / 2) / s;
        }

        [Description("Gets the average thickness of the property as if it was applied to an infinite plane.")]
        [Input("property", "The property to evaluate the average thickness of.")]
        [Output("averageThickness", "the average thickness of the property as if it was applied to an infinite plane.", typeof(Length))]
        public static double AverageThickness(this CorrugatedDeck property)
        {
            if (property.IsNull())
                return 0;

            return property.Thickness * property.VolumeFactor;
        }

        /***************************************************/


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the average thickness of the property as if it was applied to an infinite plane.")]
        [Input("property", "The property to evaluate the average thickness of.")]
        [Output("averageThickness", "the average thickness of the property as if it was applied to an infinite plane.", typeof(Length))]
        public static double IAverageThickness(this ISurfaceProperty property)
        {
            return property.IsNull() ? 0 : AverageThickness(property as dynamic);
        }


        /***************************************************/
        /**** Private Methods - Fallbacks               ****/
        /***************************************************/

        private static double AverageThickness(this ISurfaceProperty property)
        {
            Base.Compute.RecordError(property.GetType().Name + " does not have an implementation for AverageThickness. Returning NaN.");
            return double.NaN;
        }

        /***************************************************/

    }

}



