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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using BH.Engine.Spatial;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Generates a steel section based on a Profile and a material. \n This is the main create method for steel sections, responsible for calculating section constants etc. and is being called from all other create methods for steel sections.")]
        [Input("profile", "The section profile the steel section. All section constants are derived based on the dimensions of this.")]
        [Input("material", "Steel material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the steel section. If null or empty the name of the profile will be used. This is required for most structural packages to create the section.")]
        [Output("section", "The created steel section.")]
        public static SteelSection SteelSectionFromProfile(IProfile profile, Steel material = null, string name = "")
        {
            if (profile.IsNull())
                return null;

            //Run pre-process for section create. Calculates all section constants and checks name of profile
            var preProcessValues = PreProcessSectionCreate(name, profile);
            name = preProcessValues.Item1;
            profile = preProcessValues.Item2;
            Dictionary<string, double> constants = preProcessValues.Item3;

            SteelSection section = new SteelSection(profile,
                constants["Area"], constants["Rgy"], constants["Rgz"], constants["J"], constants["Iy"], constants["Iz"], constants["Iw"], constants["Wely"],
                constants["Welz"], constants["Wply"], constants["Wplz"], constants["CentreZ"], constants["CentreY"], constants["Vz"],
                constants["Vpz"], constants["Vy"], constants["Vpy"], constants["Asy"], constants["Asz"]);

            //Postprocess section. Sets default name if null, and grabs default material for section if noting is provided
            return PostProcessSectionCreate(section, name, material, MaterialType.Steel);

        }

    }
}



