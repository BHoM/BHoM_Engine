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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Physical.Materials;
using BH.oM.Physical.Constructions;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a unique name for a given IConstruction object based on the material names, and the construction name if the flag is set")]
        [Input("construction", "A physical construction object")]
        [Input("includeConstructionName", "Flag to determine whether or not the construction name itself should be included in the unique name. Default false")]
        [Output("constructionName", "A unique name for the construction")]
        public static string UniqueConstructionName(this IConstruction construction, bool includeConstructionName = false)
        {
            if (construction == null)
                return "";

            string name = includeConstructionName ? construction.Name : "construction-";
            Construction c = construction as Construction;
            foreach (Layer l in c.Layers)
                name += l.Material.Name + "-";

            return name;
        }
    }
}



