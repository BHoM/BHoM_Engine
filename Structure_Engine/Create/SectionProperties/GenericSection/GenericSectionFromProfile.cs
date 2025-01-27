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
using System.Collections.ObjectModel;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
 
using BH.oM.Base.Attributes;
using BH.Engine.Spatial;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a generic section from any shape profile. Should only be used for material types not yet explicitly supported.")]
        [Input("profile", "Profile containing the geometric information to be used to create the generic section.")]
        [Input("material", "Material to use on the section.")]
        [Input("name", "Name of the section. If no name is provided, the name from the profile will be used. This is required for most structural packages to create the section.")]
        [Output("genericSec", "The created generic section.")]
        public static GenericSection GenericSectionFromProfile(IProfile profile, IMaterialFragment material = null, string name = "")
        {
            if (profile.IsNull())
                return null;

            //Run pre-process for section create. Calculates all section constants and checks name of profile
            var preProcessValues = PreProcessSectionCreate(name, profile);
            name = preProcessValues.Item1;
            profile = preProcessValues.Item2;
            Dictionary<string, double> constants = preProcessValues.Item3;

            GenericSection section = new GenericSection(profile,
                constants["Area"], constants["Rgy"], constants["Rgz"], constants["J"], constants["Iy"], constants["Iz"], constants["Iw"], constants["Wely"],
                constants["Welz"], constants["Wply"], constants["Wplz"], constants["CentreZ"], constants["CentreY"], constants["Vz"],
                constants["Vpz"], constants["Vy"], constants["Vpy"], constants["Asy"], constants["Asz"]);

            return PostProcessSectionCreate(section, name, material, MaterialType.Undefined);

        }

        /***************************************************/
    }
}






