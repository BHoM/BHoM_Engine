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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Physical.Materials;
using BH.oM.Physical.Constructions;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Diffing;
using BH.Engine.Physical;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of unique constructions from a collection of Environment Panels")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("includeConstructionName", "Flag to determine whether or not to use the construction name as a parameter of uniqueness. Default false")]
        [Output("uniqueConstructions", "A collection of unique Construction objects")]
        public static List<Construction> UniqueConstructions(this List<Panel> panels, bool includeConstructionName = false)
        {
            List<Construction> allConstructions = panels.Where(x => x.Construction != null).Where(x => (x.Construction as Construction) != null).Select(x => x.Construction as Construction).ToList();

            return allConstructions.UniqueConstructions(includeConstructionName);
        }

        [Description("Returns a collection of unique constructions from a collection of Environment Openings")]
        [Input("openings", "A collection of Environment Openings")]
        [Input("includeConstructionName", "Flag to determine whether or not to use the construction name as a parameter of uniqueness. Default false")]
        [Output("uniqueConstructions", "A collection of unique Construction objects")]
        public static List<Construction> UniqueConstructions(this List<Opening> openings, bool includeConstructionName = false)
        {
            List<Construction> allConstructions = openings.Where(x => x.OpeningConstruction != null).Where(x => (x.OpeningConstruction as Construction) != null).Select(x => x.OpeningConstruction as Construction).ToList();
            allConstructions.AddRange(openings.Where(x => x.FrameConstruction != null).Where(x => (x.FrameConstruction as Construction) != null).Select(x => x.FrameConstruction as Construction));

            return allConstructions.UniqueConstructions(includeConstructionName);
        }

        [Description("Returns a collection of unique constructions from a nested collection of Environment Panels representing spaces")]
        [Input("panelsAsSpaces", "A nested collection of Environment Panels representing spaces")]
        [Input("includeConstructionName", "Flag to determine whether or not to use the construction name as a parameter of uniqueness. Default false")]
        [Output("uniqueConstructions", "A collection of unique Construction objects")]
        public static List<Construction> UniqueConstructions(this List<List<Panel>> panelsAsSpaces, bool includeConstructionName = false)
        {
            List<Panel> elements = new List<Panel>();
            foreach (List<Panel> e in panelsAsSpaces)
                elements.AddRange(e);

            return elements.UniqueConstructions(includeConstructionName);
        }
    }
}




