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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Environment.Properties;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Space MatchSpace(this List<Space> spaces, Point pt)
        {
            Space s = null;

            foreach(Space space in spaces)
            {
                double distToThisSpace = space.Location.Distance(pt);
                double distToCurrSpace = (s == null ? 1e10 : s.Location.Distance(pt));

                if (distToThisSpace < distToCurrSpace)
                    s = space;
            }

            return s;
        }

        public static List<List<BuildingElement>> MatchSpaces(this List<List<BuildingElement>> elementsAsSpaces, List<Space> spaces)
        {
            spaces = new List<oM.Environment.Elements.Space>(spaces);

            foreach (List<BuildingElement> bes in elementsAsSpaces)
            {
                foreach(BuildingElement be in bes)
                {
                    BuildingElementContextProperties props = be.ContextProperties() as BuildingElementContextProperties;
                    if(props != null && props.ConnectedSpaces.Count > 0)
                    {
                        Space foundSpace = spaces.Where(x => x.Name == props.ConnectedSpaces[0]).FirstOrDefault();

                        if(foundSpace != null)
                        {
                            if(!foundSpace.CustomData.ContainsKey("SAM_SpaceName"))
                                foundSpace.CustomData.Add("SAM_SpaceName", foundSpace.Name);

                            if (be.CustomData.ContainsKey("Space_Custom_Data"))
                                be.CustomData["Space_Custom_Data"] = foundSpace.CustomData;
                            else
                                be.CustomData.Add("Space_Custom_Data", foundSpace.CustomData);
                        }

                    }
                }
            }

            return elementsAsSpaces;
        }
    }
}