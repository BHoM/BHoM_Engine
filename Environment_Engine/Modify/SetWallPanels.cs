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

        [Description("Modifies a collection of Panels and sets their type to be interior or exterior wall if they have a tilt of 90 degrees. If the panel has one connected space then it is deemed to be an internal wall panel, otherwise it is an external wall panel")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a closed space")]
        [Output("panelsAsSpace", "BHoM Environment panels representing a closed space where the internal or external wall panels have had their type set")]
        [PreviousVersion("6.3", "BH.Engine.Environment.Modify.SetWallPanels(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>)")]
        public static List<Panel> SetWallPanels(this List<Panel> panelsAsSpace, PanelType externalWallType = PanelType.WallExternal, PanelType internalWallType = PanelType.WallInternal)
        {
            List<Panel> clones = new List<Panel>(panelsAsSpace.Select(x => x.DeepClone<Panel>()).ToList());

            //Find the panel(s) that are horizontal
            double minZ = 1e10;
            foreach (Panel panel in clones)
            {
                if (panel.MinimumLevel() == panel.MaximumLevel())
                    minZ = Math.Min(minZ, panel.MinimumLevel());
            }

            List<Panel> wallPanels = clones.Where(x => x.Tilt() < 92 && x.Tilt() > 88).ToList();

            if (wallPanels.Count == 0)
            {
                BH.Engine.Base.Compute.RecordWarning("No wall panels were found to set the type for");
                return clones;
            }

            foreach (Panel panel in wallPanels)
            {
                if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 1)
                    panel.Type = externalWallType;
                else if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 2)
                    panel.Type = internalWallType;
            }

            return clones;
        }
    }
}




