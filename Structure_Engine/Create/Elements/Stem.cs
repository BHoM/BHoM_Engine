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

using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a tapered Stem to be used in the RetainingWall object.")]
        [Input("perimeter", "The outer edge of the centre of the Stem in elevation.")]
        [Input("thicknessTop", "Thickness at the top of the stem.")]
        [Input("thicknessBottom", "Thickness at the bottom of the stem.")]
        [Input("normal", "Normal to the surface of the stem denoting the direction of the retained face.")]
        [Input("material", "Material of the Stem.")]
        [Output("stem", "A tapered stem.")]
        public static Stem Stem(PolyCurve perimeter, double thicknessTop, double thicknessBottom, Vector normal, IMaterialFragment material = null)
        {
            return perimeter.IsNull() ? null : new Stem() { Perimeter = perimeter, ThicknessTop = thicknessTop, ThicknessBottom = thicknessBottom, Normal = normal, Material = material };
        }

        /***************************************************/

        [Description("Creates a Stem with a constant thickness.")]
        [Input("perimeter", "The outer edge of the centre of the Stem in elevation.")]
        [Input("thickness", "Thickness at the top and bottom of the stem.")]
        [Input("normal", "Normal to the surface of the stem denoting the direction of the retained face.")]
        [Input("material", "Structural material of the property.")]
        [Output("stem", "A stem with constant thickness.")]
        public static Stem Stem(PolyCurve perimeter, double thickness, Vector normal, IMaterialFragment material = null)
        {
            return perimeter.IsNull() ? null : new Stem() { Perimeter = perimeter, ThicknessTop = thickness, ThicknessBottom = thickness, Normal = normal, Material = material };
        }

        /***************************************************/

    }
}
