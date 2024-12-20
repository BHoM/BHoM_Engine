/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

        [Description("Returns a collection of Environment Edges that are unconnected in a space")]
        [Input("panel", "An Environment Panel to check if the edges are all connected")]
        [Input("panelsAsSpace", "A collection of Environment Panels representing a single space")]
        [Output("unconnectedEdges", "A collection of Environment Edges that are not properly connected with the rest of the space")]
        public static List<Edge> UnconnectedEdges(this Panel panel, List<Panel> panelsAsSpace)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the unconnected edges of a null panel.");
                return null;
            }

            if(panelsAsSpace == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the unconnected edges of a panel if the panels comprising the space are null.");
                return null;
            }

            List<Edge> edges = panel.ExternalEdges;
            List<Line> allEdges = new List<Line>();
            foreach (Panel p in panelsAsSpace)
                allEdges.AddRange(p.ToLines());

            List<Edge> unconnected = new List<Edge>();
            foreach (Edge e in edges)
            {
                List<Line> lines = e.ToLines();
                foreach (Line l in lines)
                {
                    if (allEdges.Where(x => x.BooleanIntersection(l) != null).ToList().Count < 2)
                        unconnected.Add(e);
                }
            }

            return unconnected;
        }
    }
}






