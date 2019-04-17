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
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Volume(this Space bHoMSpace)
        {
            //TODO: This does only work for a space where all of the building element panels have the same height. Make it work for all spaces

            /*List<BHEE.BuildingElement> bHoMBuildingElement = bHoMSpace.BuildingElements;

            double roomheight = 0;
            foreach (BHEE.BuildingElement element in bHoMBuildingElement)
            {
                if (Tilt(element.BuildingElementGeometry) == 90) // if wall
                {
                    roomheight = AltitudeRange(element.BuildingElementGeometry);
                    break;
                }
            }

            return FloorArea(bHoMSpace) * roomheight;*/

            throw new NotImplementedException("Calculating the volume of a space has not been implemented yet");
        }

        /***************************************************/

        public static double Volume(this List<BuildingElement> space)
        {
            //TODO: Make this more accurate for irregular spaces
            double maxHeight = 0;
            foreach (BuildingElement be in space)
                maxHeight = Math.Max(maxHeight, (be.MaximumLevel() - be.MinimumLevel()));

            double area = space.Area();

            return area * maxHeight;
        }
    }
}
