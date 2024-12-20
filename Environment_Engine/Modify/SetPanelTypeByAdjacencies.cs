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

using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Environment.Fragments;
using System;
using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the Panel type by the spaces adjacent to it. This is only valid for wall panels and is NOT valid for roof or floor panels")]
        [Input("panels", "A collection of Environment Panels to calculate the type of")]
        [Output("panels", "A collection of Environment Panels with their type set")]
        public static List<Panel> SetPanelTypeByAdjacencies(this List<Panel> panels)
        {
            List<Panel> clones = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());

            foreach (Panel panel in clones)
            {
                if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 0)
                    panel.Type = PanelType.Shade;
                else if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 1)
                    panel.Type = PanelType.WallExternal;
                else if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 2)
                    panel.Type = PanelType.WallInternal;
            }

            return clones;
        }
    }
}






