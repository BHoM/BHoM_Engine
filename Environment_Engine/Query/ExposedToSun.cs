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

using BH.oM.Environment.Elements;
using BH.oM.Environment.Properties;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        public static bool ExposedToSun(string surfaceType)
        {
            if (String.IsNullOrEmpty(surfaceType)) return false;

            surfaceType = surfaceType.Replace(" ", String.Empty).ToLower();

            return surfaceType == "raisedfloor" || surfaceType == "exteriorwall" || surfaceType == "roof";
        }

        public static bool ExposedToSun(this BuildingElement element)
        {
            if((element.ElementProperties() as ElementProperties) != null)
            {
                BuildingElementType elementType = (element.ElementProperties() as ElementProperties).BuildingElementType;
                if (elementType == BuildingElementType.Roof || elementType == BuildingElementType.Rooflight || elementType == BuildingElementType.RooflightWithFrame || elementType == BuildingElementType.WallExternal)
                    return true;
            }

            return false;
        }
    }
}
