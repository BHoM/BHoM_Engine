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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment.Elements;
using BH.Engine.Geometry;
using BH.oM.Environment.Properties;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/

        public static List<BuildingElement> BuildSpace(this List<BuildingElement> elements, string spaceName)
        {
            List<BuildingElement> rtnElements = new List<BuildingElement>();

            foreach(BuildingElement be in elements)
            {
                BuildingElementContextProperties contextProps = be.ContextProperties() as BuildingElementContextProperties;
                if(contextProps != null)
                {
                    if (contextProps.ConnectedSpaces.Count == 1 && contextProps.ConnectedSpaces[0] == spaceName)
                        rtnElements.Add(be);
                    else if (contextProps.ConnectedSpaces.Count == 2)
                    {
                        if (contextProps.ConnectedSpaces[0] == spaceName || contextProps.ConnectedSpaces[1] == spaceName)
                            rtnElements.Add(be);
                    }
                }
            }

            return rtnElements;
        }

        public static List<List<BuildingElement>> BuildSpaces(this List<BuildingElement> elements, List<string> spaceNames)
        {
            List<List<BuildingElement>> spaces = new List<List<BuildingElement>>();

            foreach (String s in spaceNames)
                spaces.Add(elements.BuildSpace(s));

            return spaces;
        }
    }
}
