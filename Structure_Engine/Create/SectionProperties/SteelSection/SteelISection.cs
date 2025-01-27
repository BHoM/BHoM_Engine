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

using System.ComponentModel;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a steel I-section from input dimensions.")]
        [Input("height", "Full height of the section.", typeof(Length))]
        [Input("webThickness", "Thickness of the web.", typeof(Length))]
        [Input("flangeWidth", "Width of the top and bottom flange.", typeof(Length))]
        [Input("flangeThickness", "Thickness of the top and bottom flange.", typeof(Length))]
        [Input("rootRadius", "Optional fillet radius between inner face of flange and face of web.", typeof(Length))]
        [Input("toeRadius", "Optional fillet radius at the outer edge of the flange.", typeof(Length))]
        [Input("material", "Steel material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the steel section. This is required for most structural packages to create the section.")]
        [Output("section", "The created steel I-section.")]
        public static SteelSection SteelISection(double height, double webThickness, double flangeWidth, double flangeThickness, double rootRadius = 0, double toeRadius = 0, Steel material = null, string name = null)
        {
            return SteelSectionFromProfile(Spatial.Create.ISectionProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius), material, name);
        }

    }
}




