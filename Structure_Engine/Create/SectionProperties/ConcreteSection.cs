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
using System.ComponentModel;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Geometry;
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.MaterialFragments;
using System.Linq;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a rectangular solid concrete section from input dimensions.")]
        [Input("height", "Height of the section.", typeof(Length))]
        [Input("width", "Width of the section.", typeof(Length))]
        [Input("material", "Concrete material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the concrete section. This is required for various structural packages to create the object.")]
        [Input("reinforcement", "Optional list of reinforcement to be applied to the section.")]
        [Output("section", "The created rectangular concrete section.")]
        public static ConcreteSection ConcreteRectangleSection(double height, double width, Concrete material = null, string name = "", List<IBarReinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(Geometry.Create.RectangleProfile(height, width, 0), material, name, reinforcement);
        }

        /***************************************************/

        [Description("Creates a rectangular solid concrete section from input dimensions.")]
        [Input("height", "Full height of the section.", typeof(Length))]
        [Input("webThickness", "Thickness of the web.", typeof(Length))]
        [Input("flangeWidth", "Width of the flange.", typeof(Length))]
        [Input("flangeThickness", "Thickness of the flange.", typeof(Length))]
        [Input("material", "Concrete material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the concrete section. This is required for various structural packages to create the object.")]
        [Input("reinforcement", "Optional list of reinforcement to be applied to the section.")]
        [Output("section", "The created concrete T-section.")]
        public static ConcreteSection ConcreteTSection(double height, double webThickness, double flangeWidth, double flangeThickness, Concrete material = null, string name = "", List<IBarReinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(Geometry.Create.TSectionProfile(height, flangeWidth, webThickness, flangeThickness, 0, 0), material, name, reinforcement);
        }


        /***************************************************/

        [Description("Creates a circular solid concrete section from input dimensions.")]
        [Input("diameter", "Diameter of the section.", typeof(Length))]
        [Input("material", "Concrete material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the concrete section. This is required for most structural packages to create the section.")]
        [Input("reinforcement", "Optional list of reinforcement to be applied to the section.")]
        [Output("section", "The created circular concrete section.")]
        public static ConcreteSection ConcreteCircularSection(double diameter, Concrete material = null, string name = "", List<IBarReinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(Geometry.Create.CircleProfile(diameter), material, name, reinforcement);
        }

        /***************************************************/

        [Description("Creates a concrete freeform section based on edge curves. Please note that this type of section generally will have less support in adapters. If the type of section being created can be achieved by any other profile, aim use them instead.")]
        [Input("edges", "Edges defining the section. Should consist of closed curve(s) in the global xy-plane.")]
        [Input("material", "Concrete material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the concrete section. This is required for most structural packages to create the section.")]
        [Input("reinforcement", "Optional list of reinforcement to be applied to the section.")]
        [Output("section", "The created free form concrete section.")]
        public static ConcreteSection ConcreteFreeFormSection(List<ICurve> edges, Concrete material = null, string name = "", List<IBarReinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(Geometry.Create.FreeFormProfile(edges), material, name, reinforcement);
        }

        /***************************************************/

        [Description("Generates a concrete section based on a Profile and a material. \n This is the main create method for concrete sections, responsible for calculating section constants etc. and is being called from all other create methods for concrete sections.")]
        [Input("profile", "The section profile the concrete section. All section constants are derived based on the dimensions of this.")]
        [Input("material", "concrete material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the concrete section. If null or empty the name of the profile will be used. This is required for most structural packages to create the section.")]
        [Input("reinforcement", "Optional list of reinforcement to be applied to the section.")]
        [Output("section", "The created concrete section.")]
        public static ConcreteSection ConcreteSectionFromProfile(IProfile profile, Concrete material = null, string name = "", List<IBarReinforcement> reinforcement = null)
        {
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
            if (reinforcement != null)
                section.Reinforcement = reinforcement;

            return PostProcessSectionCreate(section, name, material, MaterialType.Concrete);
        }

        /***************************************************/
    }
}

