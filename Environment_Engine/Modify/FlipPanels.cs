/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BH.oM.Environment.Elements;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of Environment Panels with normal away from the space they create")] // Fix
        [Input("panels", "A collection of Environment Panels to check normal direction for")] 
        [Output("panels", "A collection of modified Environment Panels with normal away from space")]
        public static List<Panel> FlipPanels(List<Panel> panels)
        {
            List<Panel> modifiedPanels = new List<Panel>();
            foreach (Panel p in panels)
            {
                Panel p2 = p.DeepClone();
                if (!p2.NormalAwayFromSpace(panels))
                    p2.ExternalEdges = p2.Polyline().Flip().ToEdges();

                List<Opening> openings = p2.Openings.Select(x => x.DeepClone()).ToList();
                for (int x = 0; x < openings.Count; x++)
                {
                    if (!openings[x].Polyline().NormalAwayFromSpace(panels))
                        openings[x].Edges = openings[x].Polyline().Flip().ToEdges();
                }

                p2.Openings = openings;
                modifiedPanels.Add(p2);
            }

            return modifiedPanels;
        }
    }
}
