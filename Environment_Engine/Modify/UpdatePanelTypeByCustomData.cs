/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets the panel type based on some custom data for the panel")]
        [Input("panels", "A collection of Environment Panels to set the type of")]
        [Input("customDataKey", "The key of the custom data dictionary the algorithm should use to find the custom data information to set the panel type from, default empty string")]
        [Output("panels", "A collection of Environment Panels with their type set")]
        public static List<Panel> UpdatePanelTypeByCustomData(this List<Panel> panels, string customDataKey = "")
        {
            List<Panel> clones = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());
            if (customDataKey == "")
            {
                BH.Engine.Reflection.Compute.RecordWarning("You did not set a custom data key and so no modifications were made to these Panels");
                return clones; //Make no changes if no one has presented a key
            }

            foreach (Panel panel in clones)
            {
                if (panel.CustomData.ContainsKey(customDataKey))
                {
                    PanelType pType = PanelType.Undefined;
                    string type = panel.CustomData[customDataKey] as string;
                    type = type.ToLower();

                    if (type == "underground wall")
                        pType = PanelType.UndergroundWall;
                    else if (type == "curtain wall")
                        pType = PanelType.CurtainWall;
                    else if (type == "external wall")
                        pType = PanelType.WallExternal;
                    else if (type == "internal wall")
                        pType = PanelType.WallInternal;
                    else if (type == "no type")
                        pType = PanelType.Undefined;
                    else if (type == "shade")
                        pType = PanelType.Shade;
                    else if (type == "solar/pv panel")
                        pType = PanelType.SolarPanel;
                    else if (type == "roof")
                        pType = PanelType.Roof;
                    else if (type == "underground ceiling")
                        pType = PanelType.UndergroundCeiling;
                    else if (type == "internal floor")
                        pType = PanelType.FloorInternal;
                    else if (type == "exposed floor")
                        pType = PanelType.FloorExposed;
                    else if (type == "slab on grade")
                        pType = PanelType.SlabOnGrade;

                    panel.Type = pType;
                }
            }

            return clones;
        }
    }
}

