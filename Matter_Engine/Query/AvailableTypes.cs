/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /******************************************/
        /****  Public Methods                  ****/
        /******************************************/

        [Description("Gets all distinct non-empty types from the material classifications in a collection of materials, optionally filtered by category, grade, and constituent.")]
        [Input("materials", "The collection of materials to query.")]
        [Input("category", "The category to filter by. If omitted or empty, all categories will be queried.")]
        [Input("grade", "The grade to filter by. If omitted or empty, all grades will be queried.")]
        [Input("constituent", "The constituent to filter by. If omitted or empty, all constituents will be queried.")]
        [Output("types", "The distinct material classification types.")]
        public static List<string> AvailableTypes(this IEnumerable<Material> materials, string category = "", string grade = "", string constituent = "")
        {
            List<Material> filteredMaterials = materials.FilterByClassification(category, "", grade, constituent);
            if (filteredMaterials == null)
                return null;

            return filteredMaterials.Select(material => material.MaterialClassification()?.Type)
                                     .Where(type => !string.IsNullOrWhiteSpace(type))
                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                     .ToList();
        }

        /******************************************/

        [Description("Gets all distinct non-empty types from the material classifications in a collection of material properties, optionally filtered by category, grade, and constituent.")]
        [Input("materialProperties", "The collection of material properties to query.")]
        [Input("category", "The category to filter by. If omitted or empty, all categories will be queried.")]
        [Input("grade", "The grade to filter by. If omitted or empty, all grades will be queried.")]
        [Input("constituent", "The constituent to filter by. If omitted or empty, all constituents will be queried.")]
        [Output("types", "The distinct material classification types.")]
        public static List<string> AvailableTypes(this IEnumerable<IMaterialProperties> materialProperties, string category = "", string grade = "", string constituent = "")
        {
            List<IMaterialProperties> filteredProperties = materialProperties.FilterByClassification(category, "", grade, constituent);
            if (filteredProperties == null)
                return null;

            return filteredProperties.Select(materialProperty => materialProperty.MaterialClassification()?.Type)
                                     .Where(type => !string.IsNullOrWhiteSpace(type))
                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                     .ToList();
        }

        /******************************************/
    }
}