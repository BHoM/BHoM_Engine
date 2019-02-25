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
    }
}
