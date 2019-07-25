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

using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a single Environment Panel with the provided opening. Opening is added to the provided panel regardless of geometric association")]
        [Input("panel", "A single Environment Panel to add the opening to")]
        [Input("opening", "The Environment Opening to add to the panel")]
        [Output("panel", "A modified Environment Panel with the provided opening added")]
        public static Panel AddOpening(this Panel panel, Opening opening)
        {
            if (panel.Openings == null) panel.Openings = new List<Opening>();
            panel.Openings.Add(opening);
            return panel;
        }

        [Description("Returns a list of Environment Panel with the provided openings added. Openings are added to the panels which contain them geometrically.")]
        [Input("panels", "A collection of Environment Panels to add the opening to")]
        [Input("openings", "A collection of Environment Openings to add to the panels")]
        [Output("panels", "A collection of modified Environment Panels with the provided openings added")]
        public static List<Panel> AddOpenings(this List<Panel> panels, List<Opening> openings)
        {
            foreach(Opening o in openings)
            {
                Point centre = o.Polyline().Centre();
                if(centre != null)
                {
                    Panel panel = panels.PanelsContainingPoint(centre).FirstOrDefault();
                    if (panel != null)
                    {
                        if (panel.Openings == null) panel.Openings = new List<Opening>();
                        panel.Openings.Add(o);
                    }
                }
            }

            return panels;
        }
    }
}
