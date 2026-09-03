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

        [Description("Gets all distinct non-empty categories from the material classifications in a collection of materials, optionally filtered by type, grade, and constituent.")]
        [Input("materials", "The collection of materials to query.")]
        [Input("type", "The type to filter by. If omitted or empty, all types will be queried.")]
        [Input("grade", "The grade to filter by. If omitted or empty, all grades will be queried.")]
        [Input("constituent", "The constituent to filter by. If omitted or empty, all constituents will be queried.")]
        [Output("categories", "The distinct material classification categories.")]
        public static List<string> AvailableCategories(this IEnumerable<Material> materials, string type = "", string grade = "", string constituent = "")
        {
            List<Material> filteredMaterials = materials.FilterByClassification("", type, grade, constituent);
            if (filteredMaterials == null)
                return null;

            return filteredMaterials.Select(material => material.MaterialClassification()?.Category)
                            .Where(category => !string.IsNullOrWhiteSpace(category))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(category => category)
                            .ToList();
        }

        /******************************************/

        [Description("Gets all distinct non-empty categories from the material classifications in a collection of material properties, optionally filtered by type, grade, and constituent.")]
        [Input("materialProperties", "The collection of material properties to query.")]
        [Input("type", "The type to filter by. If omitted or empty, all types will be queried.")]
        [Input("grade", "The grade to filter by. If omitted or empty, all grades will be queried.")]
        [Input("constituent", "The constituent to filter by. If omitted or empty, all constituents will be queried.")]
        [Output("categories", "The distinct material classification categories.")]
        public static List<string> AvailableCategories(this IEnumerable<IMaterialProperties> materialProperties, string type = "", string grade = "", string constituent = "")
        {
            List<IMaterialProperties> filteredProperties = materialProperties.FilterByClassification("", type, grade, constituent);
            if (filteredProperties == null)
                return null;

            return filteredProperties.Select(materialProperty => materialProperty.MaterialClassification()?.Category)
                                     .Where(category => !string.IsNullOrWhiteSpace(category))
                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                     .OrderBy(category => category)
                                     .ToList();
        }

        /******************************************/
    }
}