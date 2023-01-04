/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using System.Linq;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a steel freeform section based on edge curves. Please note that this type of section generally will have less support in adapters. If the type of section being created can be achieved by any other profile, aim use them instead.")]
        [Input("edges", "Edges defining the section. Should consist of closed curve(s) in the global xy-plane.")]
        [Input("material", "Steel material to be applied to the section. If null a default material will be extracted from the database.")]
        [Input("name", "Name of the steel section. This is required for most structural packages to create the section.")]
        [Output("section", "The created free form steel section.")]
        public static SteelSection SteelFreeFormSection(List<ICurve> edges, Steel material = null, string name = null)
        {
            return edges.Count == 0 || edges.Any(x => x.IsNull()) ? null : SteelSectionFromProfile(Spatial.Create.FreeFormProfile(edges), material, name);
        }

    }
}

