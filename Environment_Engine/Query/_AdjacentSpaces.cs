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

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<BuildingElement>> AdjacentSpaces(this BuildingElement element, List<List<BuildingElement>> spaces)
        {
            //Get the lists which contain this building element
            return spaces.Where(x => x.Where(y => y.BHoM_Guid == element.BHoM_Guid).Count() > 0).ToList();
        }

        public static List<Space> AdjacentSpaces(this BuildingElement element, List<List<BuildingElement>> besAsSpace, List<Space> spaces)
        {
            List<Space> rtn = new List<oM.Environment.Elements.Space>();

            List<Point> spaces1 = element.AdjacentSpaces(besAsSpace).SpaceCentres();

            foreach(Point p in spaces1)
            {
                Space add = spaces.MatchSpace(p);
                if (add != null)
                    rtn.Add(add);
            }

            return rtn;
        }

        public static bool MatchAdjacencies(this BuildingElement element, BuildingElement compare)
        {
            if (element.CustomData.ContainsKey("Revit_spaceId") && compare.CustomData.ContainsKey("Revit_spaceId"))
            {
                if ((element.CustomData.ContainsKey("Revit_adjacentSpaceId") && compare.CustomData.ContainsKey("Revit_adjacentSpaceId")))
                    return element.CustomData["Revit_spaceId"].ToString().Equals(compare.CustomData["Revit_spaceId"].ToString()) &&
                        element.CustomData["Revit_adjacentSpaceId"].ToString().Equals(compare.CustomData["Revit_adjacentSpaceId"].ToString());
                else
                    return element.CustomData["Revit_spaceId"].ToString().Equals(compare.CustomData["Revit_spaceId"].ToString());
            }
            else return false;
        }
    }
}