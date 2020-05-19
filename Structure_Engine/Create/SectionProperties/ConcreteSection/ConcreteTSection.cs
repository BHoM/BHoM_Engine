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

        [PreviousVersion("3.2", "BH.Engine.Structure.Create.ConcreteTSection(System.Double, System.Double, System.Double, System.Double, BH.oM.Structure.MaterialFragments.Concrete, System.String, System.Collections.Generic.List<BH.oM.Structure.SectionProperties.Reinforcement.Reinforcement>)")]
        [Description("Creates a rectangular solid concrete section from input dimensions.")]
        [Input("height", "Full height of the section.", typeof(Length))]
        [Input("webThickness", "Thickness of the web.", typeof(Length))]
        [Input("flangeWidth", "Width of the flange.", typeof(Length))]
        [Input("flangeThickness", "Thickness of the flange.", typeof(Length))]
        [Input("material", "Concrete material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the concrete section. This is required for various structural packages to create the object.")]
        [Input("reinforcement", "Optional list of reinforcement to be applied to the section.")]
        [InputFromProperty("minimumCover")]
        [Output("section", "The created concrete T-section.")]
        public static ConcreteSection ConcreteTSection(double height, double webThickness, double flangeWidth, double flangeThickness, Concrete material = null, string name = "", List<IBarReinforcement> reinforcement = null, double minimumCover = 0)
        {
            return ConcreteSectionFromProfile(Geometry.Create.TSectionProfile(height, flangeWidth, webThickness, flangeThickness, 0, 0), material, name, reinforcement, minimumCover);
        }


        /***************************************************/

    }
}

