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

using BH.oM.Physical.Properties;
using BH.oM.Physical.Properties.Construction;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Query.UniqueConstructions => Returns a collection of unique constructions from a collection of Environment Panels")]
        [Input("panels", "A collection of Environment Panels")]
        [Output("uniqueConstructions", "A collection of unique Construction objects")]
        public static List<Construction> UniqueConstructions(this List<Panel> panels)
        {
            List<Construction> unique = new List<Construction>();

            foreach(Panel be in panels)
            {
                Construction t = unique.Where(x => x.UniqueConstructionName() == be.Construction.UniqueConstructionName()).FirstOrDefault();
                if (t == null)
                    unique.Add(be.Construction as Construction);

                foreach(Opening o in be.Openings)
                {
                    Construction t2 = unique.Where(x => x.UniqueConstructionName() == o.FrameConstruction.UniqueConstructionName()).FirstOrDefault();
                    if (t2 == null)
                        unique.Add(o.FrameConstruction as Construction);

                    Construction t3 = unique.Where(x => x.UniqueConstructionName() == o.OpeningConstruction.UniqueConstructionName()).FirstOrDefault();
                    if (t3 == null)
                        unique.Add(o.OpeningConstruction as Construction);
                }
            }

            return unique;
        }

        [Description("BH.Engine.Environment.Query.UniqueConstructions => Returns a collection of unique constructions from a nested collection of Environment Panels representing spaces")]
        [Input("panelsAsSpaces", "A nested collection of Environment Panels representing spaces")]
        [Output("uniqueConstructions", "A collection of unique Construction objects")]
        public static List<Construction> UniqueConstructions(this List<List<Panel>> panelsAsSpaces)
        {
            List<Panel> elements = new List<Panel>();
            foreach (List<Panel> e in panelsAsSpaces)
                elements.AddRange(e);

            return elements.UniqueConstructions();
        }

        [Description("BH.Engine.Environment.Query.UniqueConstructionName => Returns a unique name for a given IConstruction object based on the layer and material names")]
        [Input("construction", "A physical construction object")]
        [Output("constructionName", "A unique name for the construction")]
        public static string UniqueConstructionName(this IConstruction construction)
        {
            string name = "construction-";
            Construction c = construction as Construction;
            foreach (Layer l in c.Layers)
                name += l.Material.Name + "-";

            return name;
        }
    }
}