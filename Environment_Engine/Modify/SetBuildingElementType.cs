/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Environment.Properties;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> SetBuildingElementTypeByAdjacencies(this List<BuildingElement> elements)
        {
            foreach(BuildingElement be in elements)
            {
                ElementProperties elementProps = be.ElementProperties() as ElementProperties;
                if(elementProps == null)
                {
                    elementProps = new ElementProperties();
                    be.ExtendedProperties.Add(elementProps);
                }

                BuildingElementContextProperties contextProps = be.ContextProperties() as BuildingElementContextProperties;
                if(contextProps == null)
                {
                    contextProps = new BuildingElementContextProperties();
                    be.ExtendedProperties.Add(contextProps);
                }

                if (contextProps.ConnectedSpaces.Count == 0)
                    elementProps.BuildingElementType = BuildingElementType.Shade;
                else if (contextProps.ConnectedSpaces.Count == 1)
                    elementProps.BuildingElementType = BuildingElementType.WallExternal;
                else if (contextProps.ConnectedSpaces.Count == 2)
                    elementProps.BuildingElementType = BuildingElementType.WallInternal;                
            }

            return elements;
        }

        public static List<BuildingElement> UpdateBuildingElementTypeByCustomData(this List<BuildingElement> elements)
        {
            //Temporary fix for Revit...
            foreach(BuildingElement be in elements)
            {
                if(be.CustomData.ContainsKey("Type SAM_BuildingElementType"))
                {
                    BuildingElementType bType = BuildingElementType.Undefined;
                    string type = be.CustomData["Type SAM_BuildingElementType"] as string;
                    type = type.ToLower();

                    if (type == "underground wall")
                        bType = BuildingElementType.UndergroundWall;
                    else if (type == "curtain wall")
                        bType = BuildingElementType.CurtainWall;
                    else if (type == "external wall")
                        bType = BuildingElementType.WallExternal;
                    else if (type == "internal wall")
                        bType = BuildingElementType.WallInternal;
                    else if (type == "no type")
                        bType = BuildingElementType.Undefined;
                    else if (type == "shade")
                        bType = BuildingElementType.Shade;
                    else if (type == "solar/pv panel")
                        bType = BuildingElementType.SolarPanel;
                    else if (type == "glazing")
                        bType = BuildingElementType.Glazing;
                    else if (type == "rooflight")
                        bType = BuildingElementType.Rooflight;
                    else if (type == "door")
                        bType = BuildingElementType.Door;
                    else if (type == "vehicle door")
                        bType = BuildingElementType.VehicleDoor;
                    else if (type == "roof")
                        bType = BuildingElementType.Roof;
                    else if (type == "underground ceiling")
                        bType = BuildingElementType.UndergroundCeiling;
                    else if (type == "internal floor")
                        bType = BuildingElementType.FloorInternal;
                    else if (type == "exposed floor")
                        bType = BuildingElementType.FloorExposed;
                    else if (type == "slab on grade")
                        bType = BuildingElementType.SlabOnGrade;

                    if (bType != BuildingElementType.Undefined)
                    {
                        if ((be.ElementProperties() as ElementProperties) == null)
                            be.ExtendedProperties.Add(new ElementProperties());

                        (be.ElementProperties() as ElementProperties).BuildingElementType = bType;
                    }
                }
            }

            return elements;
        }

        public static BuildingElement UpdateBuildingElementTypeByCustomData(this BuildingElement element)
        {
            return (new List<BuildingElement> { element }).UpdateBuildingElementTypeByCustomData()[0];
        }
    }
}
