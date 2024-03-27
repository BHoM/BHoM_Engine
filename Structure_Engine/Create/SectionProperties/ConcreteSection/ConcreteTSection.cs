/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Reinforcement;
using BH.oM.Structure.SectionProperties;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a concrete T-section section from input dimensions.")]
        [Input("height", "Full height of the section.", typeof(Length))]
        [Input("webThickness", "Thickness of the web.", typeof(Length))]
        [Input("flangeWidth", "Width of the flange.", typeof(Length))]
        [Input("flangeThickness", "Thickness of the flange.", typeof(Length))]
        [Input("material", "Concrete material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the concrete section. This is required for various structural packages to create the object.")]
        [Input("rebarIntent", "Optional RebarIntent to be applied to the section.")]
        [Output("section", "The created concrete T-section.")]
        public static ConcreteSection ConcreteTSection(double height, double webThickness, double flangeWidth, double flangeThickness, Concrete material = null, string name = "", BarRebarIntent rebarIntent = null)
        {
            return ConcreteSectionFromProfile(Spatial.Create.TSectionProfile(height, flangeWidth, webThickness, flangeThickness, 0, 0), material, name, rebarIntent);
        }


        /***************************************************/

    }
}





