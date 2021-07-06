/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Geometry;
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Spatial;
using System.Linq;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Generates a concrete section based on a Profile and a material. \n This is the main create method for concrete sections, responsible for calculating section constants etc. and is being called from all other create methods for concrete sections.")]
        [Input("profile", "The section profile the concrete section. All section constants are derived based on the dimensions of this.")]
        [Input("material", "concrete material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the concrete section. If null or empty the name of the profile will be used. This is required for most structural packages to create the section.")]
        [Input("rebarIntent", "Optional BarRebarIntent to be applied to the section.")]
        [Output("section", "The created concrete section.")]
        public static ConcreteSection ConcreteSectionFromProfile(IProfile profile, Concrete material = null, string name = "", BarRebarIntent rebarIntent = null)
        {
            if (profile.IsNull())
                return null;

            //Run pre-process for section create. Calculates all section constants and checks name of profile
            var preProcessValues = PreProcessSectionCreate(name, profile);
            name = preProcessValues.Item1;
            profile = preProcessValues.Item2;
            Dictionary<string, double> constants = preProcessValues.Item3;

            ConcreteSection section = new ConcreteSection(profile,
                constants["Area"], constants["Rgy"], constants["Rgz"], constants["J"], constants["Iy"], constants["Iz"], constants["Iw"],
                constants["Wely"], constants["Welz"], constants["Wply"], constants["Wplz"], constants["CentreZ"], constants["CentreY"], constants["Vz"],
                constants["Vpz"], constants["Vy"], constants["Vpy"], constants["Asy"], constants["Asz"]);

            //Set reinforcement if any provided
            if (rebarIntent != null)
                section.RebarIntent = rebarIntent;

            return PostProcessSectionCreate(section, name, material, MaterialType.Concrete);
        }

        /***************************************************/
    }
}


