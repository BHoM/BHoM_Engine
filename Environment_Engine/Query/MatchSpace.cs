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
                Space foundSp = null;

                var be1 = bes.Where(x => x.CustomData["AdjacentSpaceID"].ToString() == "-1").FirstOrDefault();
                if (be1 != null)
                    foundSp = spaces.Where(x => x.CustomData["Revit_elementId"].ToString() == be1.CustomData["SpaceID"].ToString()).FirstOrDefault();
                else
                {
                    Dictionary<String, int> spaceNames = new Dictionary<string, int>();
                    foreach(BuildingElement be in bes)
                    {
                        if (spaceNames.ContainsKey(be.CustomData["SpaceID"].ToString()))
                            spaceNames[be.CustomData["SpaceID"].ToString()]++;
                        else
                            spaceNames.Add(be.CustomData["SpaceID"].ToString(), 1);

                        if (be.CustomData["AdjacentSpaceID"].ToString() != "-1")
                        {
                            if (spaceNames.ContainsKey(be.CustomData["AdjacentSpaceID"].ToString()))
                                spaceNames[be.CustomData["AdjacentSpaceID"].ToString()]++;
                            else
                                spaceNames.Add(be.CustomData["AdjacentSpaceID"].ToString(), 1);
                        }
                    }

                    string spaceID = spaceNames.Where(x => x.Value == bes.Count).FirstOrDefault().Key;
                    foundSp = spaces.Where(x => x.CustomData["Revit_elementId"].ToString() == spaceID).FirstOrDefault();
                }

                /*int count = 0;
                while(foundSp == null && count < bes.Count)
                {
                    foundSp = spaces.Where(x => x.CustomData["Revit_elementId"].ToString() == bes[count].CustomData["SpaceID"].ToString() || x.CustomData["Revit_elementId"].ToString() == bes[count].CustomData["AdjacentSpaceID"].ToString()).FirstOrDefault();
                    count++;
                }

                /*foreach (Space s in spaces)
                {
                    if (s.Location == null)
                    {
                        foundSp = s; break;
                    }
                    if (!bes.IsContaining(s.Location))
                        continue;
                    else
                    {
                        foundSp = s;
                        break;
                    }
                }*/
                

                if (foundSp != null)
                {
                    foreach (BuildingElement be in bes)
                    {
                        if (be.CustomData.ContainsKey("Space_Custom_Data"))
                            be.CustomData["Space_Custom_Data"] = foundSp.CustomData;
                        else
                            be.CustomData.Add("Space_Custom_Data", foundSp.CustomData);

                        if (be.CustomData.ContainsKey("SAM_SPACE_NAME_TEST"))
                            be.CustomData["SAM_SPACE_NAME_TEST"] = foundSp.CustomData["SAM_SpaceName"];
                        else
                            be.CustomData.Add("SAM_SPACE_NAME_TEST", foundSp.CustomData["SAM_SpaceName"]);
                    }

                    spaces.Remove(foundSp);
                }
            }

            return elementsAsSpaces;
        }
    }
}