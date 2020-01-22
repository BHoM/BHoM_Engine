/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Generates a aluminium section based on a Profile and a material. \n This is the main create method for aluminium sections, responsible for calculating section constants etc. and is being called from all other create methods for aluminium sections")]
        [Input("profile", "The section profile the aluminium section. All section constants are derived based on the dimensions of this")]
        [Input("material", "aluminium material to be used on the section.")]
        [Input("name", "Name of the aluminium section. If null or empty the name of the profile will be used")]
        [Output("section", "The created aluminium section")]
        public static AluminiumSection AluminiumSectionFromProfile(IProfile profile, Aluminium material = null, string name = "")
        {
            //Check name
            if (string.IsNullOrWhiteSpace(name) && profile.Name != null)
                name = profile.Name;

            //Check profile and raise warnings
            if (profile.Edges.Count == 0)
            {
                Engine.Reflection.Compute.RecordWarning("Profile with name " + profile.Name + " does not contain any edges. Section named " + name + " made with this profile will have 0 value sections constants");
            }

            Output<IProfile, Dictionary<string, object>> result = Compute.Integrate(profile, Tolerance.MicroDistance);

            profile = result.Item1;
            Dictionary<string, object> constants = result.Item2;

            constants["J"] = profile.ITorsionalConstant();
            constants["Iw"] = profile.IWarpingConstant();

            AluminiumSection section = new AluminiumSection(profile,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"], (double)constants["Wely"],
                (double)constants["Welz"], (double)constants["Wply"], (double)constants["Wplz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            if (material == null)
            {
                material = Query.Default(oM.Structure.MaterialFragments.MaterialType.Aluminium) as Aluminium;
            }

            section.Material = material;
            section.Name = name;


            return section;

        }

        /***************************************************/
    }
}
