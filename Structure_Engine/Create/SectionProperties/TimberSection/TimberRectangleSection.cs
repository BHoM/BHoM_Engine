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
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
 
using BH.oM.Base.Attributes;
using BH.Engine.Spatial;
using System.Linq;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a rectangular solid timber section from input dimensions.")]
        [Input("height", "Height of the section.", typeof(Length))]
        [Input("width", "Width of the section.", typeof(Length))]
        [Input("cornerRadius", "Optional corner radius for the section.", typeof(Length))]
        [Input("material", "Timber material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the timber section. This is required for most structural packages to create the section.")]
        [Output("section", "The created rectangular solid timber section.")]
        public static TimberSection TimberRectangleSection(double height, double width, double cornerRadius = 0, ITimber material = null, string name = "")
        {
            return TimberSectionFromProfile(Spatial.Create.RectangleProfile(height, width, cornerRadius), material, name);
        }

        /***************************************************/
    }
}





