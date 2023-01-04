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

        [Description("Modifies a collection of Panels and sets their type to be slab on grade or internal floor if they are the lowest panel in the space. If the panel has one connected space then it is deemed to be a slab on grade panel, otherwise it is an internal floor panel")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a closed space")]
        [Output("panelsAsSpace", "BHoM Environment panels representing a closed space where the slab on grade or internal floor panels have had their type set")]
        public static List<Panel> SetFloorPanels(this List<Panel> panelsAsSpace)
        {
            List<Panel> clones = new List<Panel>(panelsAsSpace.Select(x => x.DeepClone<Panel>()).ToList());

            //Find the panel(s) that are at the lowest point of the space...
            double minZ = 1e10;
            foreach (Panel panel in clones)
            {
                if (panel.MinimumLevel() == panel.MaximumLevel())
                    minZ = Math.Min(minZ, panel.MinimumLevel());
            }

            List<Panel> floorPanels = clones.Where(x => x.MinimumLevel() == minZ && x.MaximumLevel() == minZ).ToList();

            if (floorPanels.Count == 0)
            {
                BH.Engine.Base.Compute.RecordWarning("No floor panels were found to set the type for");
                return clones;
            }

            foreach (Panel panel in floorPanels)
            {
                if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 1)
                    panel.Type = PanelType.SlabOnGrade;
                else if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 2)
                    panel.Type = PanelType.FloorInternal;
            }


            return clones;
        }
    }
}




