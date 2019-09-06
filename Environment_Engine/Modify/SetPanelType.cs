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
using BH.oM.Environment.Fragments;
using System;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

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
            foreach(Panel panel in panels)
            {
                if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 0)
                    panel.Type = PanelType.Shade;
                else if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 1)
                    panel.Type = PanelType.WallExternal;
                else if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 2)
                    panel.Type = PanelType.WallInternal;                
            }

            return panels;
        }

        [Description("Sets the panel type based on some custom data for the panel")]
        [Input("panels", "A collection of Environment Panels to set the type of")]
        [Input("customDataKey", "The key of the custom data dictionary the algorithm should use to find the custom data information to set the panel type from, default empty string")]
        [Output("panels", "A collection of Environment Panels with their type set")]
        public static List<Panel> UpdatePanelTypeByCustomData(this List<Panel> panels, string customDataKey = "")
        {
            if (customDataKey == "")
            {
                BH.Engine.Reflection.Compute.RecordWarning("You did not set a custom data key and so no modifications were made to these Panels");
                return panels; //Make no changes if no one has presented a key
            }

            foreach(Panel panel in panels)
            {
                if(panel.CustomData.ContainsKey(customDataKey))
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

            return panels;
        }

        [Description("Sets the panel type based on the provided type")]
        [Input("panels", "A collection of Environment Panels to set the type of")]
        [Input("type", "The panel type to assign to the panels")]
        [Output("panels", "A collection of Environment Panels with their type set")]
        public static List<Panel> SetPanelType(this List<Panel> panels, PanelType type)
        {
            foreach (Panel p in panels)
                p.Type = type;

            return panels;
        }

        [Description("Returns the floor panels of a space represented by Environment Panels")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a closed space")]
        [Output("floorPanels", "BHoM Environment panel representing the floor of the space")]
        public static List<Panel> FloorPanels(this List<Panel> panelsAsSpace)
        {
            //Find the panel(s) that are at the lowest point of the space...
            double minZ = 1e10;
            foreach (Panel panel in panelsAsSpace)
            {
                if (panel.MinimumLevel() == panel.MaximumLevel())
                    minZ = Math.Min(minZ, panel.MinimumLevel());
            }

            List<Panel> floorPanels = panelsAsSpace.Where(x => x.MinimumLevel() == minZ && x.MaximumLevel() == minZ).ToList();

            if (floorPanels.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Could not find floor panel");
                return null;
            }

            foreach (Panel panel in floorPanels)
            {
                if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 1)
                    panel.Type = PanelType.SlabOnGrade;
                else if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 2)
                    panel.Type = PanelType.FloorInternal;
            }


            return floorPanels;
        }

        [Description("Returns the roof panels of a space represented by Environment Panels")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a closed space")]
        [Output("roofPanels", "BHoM Environment panel representing the roof of the space")]
        public static List<Panel> RoofPanels(this List<Panel> panelsAsSpace)
        {
            //Find the panel(s) that are at the highest point of the space...
            double minZ = 1e10;
            foreach (Panel panel in panelsAsSpace)
            {
                if (panel.MinimumLevel() == panel.MaximumLevel())
                    minZ = Math.Min(minZ, panel.MinimumLevel());
            }

            List<Panel> roofPanels = panelsAsSpace.Where(x => ((x.MinimumLevel() != minZ && x.MaximumLevel() != minZ) && Math.Round(x.Tilt()) != 90) && x.ConnectedSpaces.ToList().Count == 1).ToList();

            foreach (Panel panel in panelsAsSpace)
            {
                if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 1)
                    panel.Type = PanelType.Roof;
            }

            return roofPanels;
        }

        [Description("Returns the wall panels of a space represented by Environment Panels and fixes PanelType")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a closed space")]
        [Output("wallPanels", "BHoM Environment panel representing the wall of the space")]
        public static List<Panel> WallPanels(this List<Panel> panelsAsSpace)
        {
            //Find the panel(s) that ... is horizontal
            double minZ = 1e10;
            foreach (Panel panel in panelsAsSpace)
            {
                if (panel.MinimumLevel() == panel.MaximumLevel())
                    minZ = Math.Min(minZ, panel.MinimumLevel());
            }

            List<Panel> wallPanels = panelsAsSpace.Where(x => x.Tilt() < 92 && x.Tilt() > 88).ToList();

            if (wallPanels.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Could not find wall panel");
                return null;
            }

            foreach (Panel panel in wallPanels)
            {
                if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 1)
                    panel.Type = PanelType.WallExternal;
                else if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 2)
                    panel.Type = PanelType.WallInternal;
            }

            return wallPanels;
        }

        [Description("Returns the shade panels represented by Environment Panels with no adj spaces and fixes PanelType")]
        [Input("panels", "A collection of Environment Panels")]
        [Output("shadePanels", "BHoM Environment panel representing the shade")]
        public static List<Panel> ShadePanels(this List<Panel> panels)
        {
            //Find the panel(s) without connected spaces and set as shade
            List<Panel> shadePanels = panels;

            foreach (Panel panel in shadePanels)
            {
                if (panel.ConnectedSpaces.Where(x => x != "-1").ToList().Count == 0)
                    panel.Type = PanelType.Shade;
            }

            return shadePanels;
        }
    }
}
