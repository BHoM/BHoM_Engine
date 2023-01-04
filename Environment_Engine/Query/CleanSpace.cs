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

        [Description("Removes panels which do not have a suitable connection to the space (i.e. panels with only 1 connection to the space)")]
        [Input("panelsAsSpace", "A collection of Environment Panels representing a single space")]
        [Output("panelsAsSpace", "A collection of Environment Panels representing a single space with incorrect panels removed")]
        [ToBeRemoved("3.3", "Removed as this method does not provide any meaningful computation to panels owing to the connected space attribute not being updated.")]
        public static List<Panel> CleanSpace(this List<Panel> panelsAsSpace)
        {
            //Remove elements which have 1 or less connections with other elements
            List<Panel> cleanSpace = new List<Panel>();

            List<Line> allEdges = new List<Line>();
            foreach (Panel p in panelsAsSpace)
                allEdges.AddRange(p.ToLines());

            foreach(Panel be in panelsAsSpace)
            {
                List<Line> edges = be.ToLines();
                bool addSpace = true;
                foreach (Line l in edges)
                {
                    if (allEdges.Where(x => x.BooleanIntersection(l) != null).ToList().Count < 2)
                        addSpace = false;
                }

                if(addSpace)
                    cleanSpace.Add(be);
            }

            cleanSpace = cleanSpace.CullDuplicates();

            return cleanSpace;
        }
    }
}



