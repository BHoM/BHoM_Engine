/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

        [Description("Creates a rectangular solid timber section from input dimensions. Please note that all units are in S.I., that is meters [m]")]
        [Input("height", "Height of the section [m]")]
        [Input("width", "Width of the section [m]")]
        [Input("cornerRadius", "Optional corner radius for the section [m]")]
        [Input("material", "Timber material to be used on the section. If null a default material will be extracted from the database")]
        [Input("name", "Name of the timber section.")]
        [Output("section", "The created rectangular solid timber section")]
        public static TimberSection TimberRectangleSection(double height, double width, double cornerRadius = 0, Timber material = null, string name = "")
        {
            return TimberSectionFromProfile(Geometry.Create.RectangleProfile(height, width, cornerRadius), material, name);
        }

        /***************************************************/

        [Description("Generates a timber section based on a Profile and a material. \n This is the main create method for timber sections, responsible for calculating section constants etc. and is being called from all other create methods for timber sections")]
        [Input("profile", "The section profile the timber section. All section constants are derived based on the dimensions of this")]
        [Input("material", "timber material to be used on the section.")]
        [Input("name", "Name of the timber section. If null or empty the name of the profile will be used")]
        [Output("section", "The created timber section")]
        public static TimberSection TimberSectionFromProfile(IProfile profile, Timber material = null, string name = "")
        {
            //Run pre-process for section create. Calculates all section constants and checks name of profile
            var preProcessValues = PreProcessSectionCreate(name, profile);
            name = preProcessValues.Item1;
            profile = preProcessValues.Item2;
            Dictionary<string, double> constants = preProcessValues.Item3;

            TimberSection section = new TimberSection(profile,
                constants["Area"], constants["Rgy"], constants["Rgz"], constants["J"], constants["Iy"], constants["Iz"], constants["Iw"], constants["Wely"],
                constants["Welz"], constants["Wply"], constants["Wplz"], constants["CentreZ"], constants["CentreY"], constants["Vz"],
                constants["Vpz"], constants["Vy"], constants["Vpy"], constants["Asy"], constants["Asz"]);

            return PostProcessSectionCreate(section, name, material, MaterialType.Timber);

        }

        /***************************************************/
    }
}

