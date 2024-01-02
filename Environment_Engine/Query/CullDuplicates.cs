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

using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Environment.Elements;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Removes panels which are duplicates")]
        [Input("panels", "A collection of Environment Panels")]
        [Output("panels", "A collection of Environment Panels with no duplicates")]
        public static List<Panel> CullDuplicates(this List<Panel> panels)
        {
            //Go through each building element and compare vertices and centre points - if there is a matching element, remove it
            for(int x = 0; x < panels.Count; x++)
            {
                if (panels[x] == null) continue;

                for(int y = x + 1; y < panels.Count; y++)
                {
                    if (panels[x].IsIdentical(panels[y]))
                        panels[y] = null;
                }
            }

            return panels.Where(x => x != null).ToList();
        }

        [Description("Removes panels which are duplicates from panels representing spaces")]
        [Input("panelsAsSpaces", "The nested collection of Environment Panels that represent the spaces to cull duplicates from")]
        [Output("panelsAsSpaces", "A nested collection of Environment Panels representing spaces with no duplicates")]
        public static List<List<Panel>> CullDuplicates(this List<List<Panel>> panelsAsSpaces)
        {
            //Go through each set of building elements and find those that match
            for(int x = 0; x < panelsAsSpaces.Count; x++)
            {
                List<Panel> space = panelsAsSpaces[x];
                for(int y = x+1; y < panelsAsSpaces.Count; y++)
                {
                    List<Panel> space2 = panelsAsSpaces[y];
                    if (space2.Count != space.Count) continue; //Numbers don't match so no point checking equality
                    bool allMatch = true;
                    
                    foreach(Panel be in space)
                    {
                        allMatch &= space2.Contains(be);
                        if (!allMatch) break; //No point checking everything if we find a non-match
                    }

                    if(allMatch)
                    {
                        //This space matches another space, set all the BEs to null
                        panelsAsSpaces[y] = new List<Panel>(); //Empty list
                    }                    
                }
            }

            return panelsAsSpaces.Where(x => x.Count > 0).ToList();
        }
    }
}





