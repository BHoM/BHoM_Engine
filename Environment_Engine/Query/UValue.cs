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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double UValue(this BuildingElement element)
        {
            if (element == null) return 0;
            if (element.BuildingElementProperties == null) return -1;
            if (element.BuildingElementProperties.Construction == null) return -1;

            if (element.BuildingElementProperties.Construction.UValues.Count == 0) return -1;

            if (element.BuildingElementProperties.Construction.UValues.Count == 1) return element.BuildingElementProperties.Construction.UValues[0];

            switch(element.BuildingElementProperties.BuildingElementType)
            {
                case BuildingElementType.Ceiling:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 5) return -1;
                    return element.BuildingElementProperties.Construction.UValues[4];
                case BuildingElementType.CurtainWall:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 7) return -1;
                    return element.BuildingElementProperties.Construction.UValues[6];
                case BuildingElementType.Door:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 1) return -1;
                    return element.BuildingElementProperties.Construction.UValues[0];
                case BuildingElementType.Floor:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 6) return -1;
                    return element.BuildingElementProperties.Construction.UValues[5]; //Exposed floor would be 2...
                case BuildingElementType.Roof:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 2) return -1;
                    return element.BuildingElementProperties.Construction.UValues[1];
                case BuildingElementType.Rooflight:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 7) return -1;
                    return element.BuildingElementProperties.Construction.UValues[6];
                case BuildingElementType.Wall:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 4) return -1;
                    return element.BuildingElementProperties.Construction.UValues[3];
                case BuildingElementType.Window:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 1) return -1;
                    return element.BuildingElementProperties.Construction.UValues[0]; //Not sure if correct...
                case BuildingElementType.RooflightWithFrame:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 7) return -1;
                    return element.BuildingElementProperties.Construction.UValues[6];
                case BuildingElementType.WindowWithFrame:
                    if (element.BuildingElementProperties.Construction.UValues.Count < 1) return -1;
                    return element.BuildingElementProperties.Construction.UValues[0]; //Not sure if correct...
                default:
                    return -1;
            }
        }
    }
}
