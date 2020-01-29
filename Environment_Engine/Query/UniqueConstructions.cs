/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
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
            string name = includeConstructionName ? construction.Name : "construction-";
            Construction c = construction as Construction;
            foreach (Layer l in c.Layers)
                name += l.Material.Name + "-";

            return name;
        }

        [Description("Returns a collection of unique constructions from a collection of Environment Panels")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("includeConstructionName", "Flag to determine whether or not to use the construction name as a parameter of uniqueness. Default false")]
        [Output("uniqueConstructions", "A collection of unique Construction objects")]
        public static List<Construction> UniqueConstructions(this List<Panel> panels, bool includeConstructionName = false)
        {
            List<Construction> unique = new List<Construction>();

            foreach (Panel be in panels)
            {
                if (be.Construction != null)
                {
                    Construction t = unique.Where(x => x.UniqueConstructionName(includeConstructionName) == be.Construction.UniqueConstructionName(includeConstructionName)).FirstOrDefault();
                    if (t == null)
                        unique.Add(be.Construction as Construction);
                }

                foreach (Opening o in be.Openings)
                {
                    if (o.FrameConstruction != null)
                    {
                        Construction t2 = unique.Where(x => x.UniqueConstructionName(includeConstructionName) == o.FrameConstruction.UniqueConstructionName(includeConstructionName)).FirstOrDefault();
                        if (t2 == null)
                            unique.Add(o.FrameConstruction as Construction);
                    }

                    if (o.OpeningConstruction != null)
                    {
                        Construction t3 = unique.Where(x => x.UniqueConstructionName(includeConstructionName) == o.OpeningConstruction.UniqueConstructionName(includeConstructionName)).FirstOrDefault();
                        if (t3 == null)
                            unique.Add(o.OpeningConstruction as Construction);
                    }
                }
            }

            return unique;
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

        [Description("Returns a collection of unique constructions from a collection of Environment Panels")]
        [Input("panels", "A collection of Environment Panels")]
        [Output("uniqueConstructions", "A collection of unique Construction objects")]
        [Deprecated("3.1", "Deprecated to allow to include option for including the construction name in its unique name")]
        public static List<Construction> UniqueConstructions(this List<Panel> panels)
        {
            return panels.UniqueConstructions(false);
        }

        [Description("Returns a collection of unique constructions from a nested collection of Environment Panels representing spaces")]
        [Input("panelsAsSpaces", "A nested collection of Environment Panels representing spaces")]
        [Output("uniqueConstructions", "A collection of unique Construction objects")]
        [Deprecated("3.1", "Deprecated to allow to include option for including the construction name in its unique name")]
        public static List<Construction> UniqueConstructions(this List<List<Panel>> panelsAsSpaces)
        {
            return panelsAsSpaces.UniqueConstructions(false);
        }

        [Description("Returns a unique name for a given IConstruction object based on the layer and material names")]
        [Input("construction", "A physical construction object")]
        [Output("constructionName", "A unique name for the construction")]
        [Deprecated("3.1", "Deprecated to allow to include option for including the construction name in its unique name")]
        public static string UniqueConstructionName(this IConstruction construction)
        {
            return construction.UniqueConstructionName(false);
        }
    }
}