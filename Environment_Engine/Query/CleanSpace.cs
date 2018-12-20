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

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> CleanSpace(this List<BuildingElement> space)
        {
            //Remove elements which have 1 or less connections with other elements
            List<BuildingElement> cleanSpace = new List<BuildingElement>();

            List<Line> allEdges = space.Edges();

            foreach(BuildingElement be in space)
            {
                List<Line> edges = be.Edges();
                bool addSpace = true;
                foreach (Line l in edges)
                {
                    if (allEdges.Where(x => x.BooleanIntersection(l) != null).ToList().Count < 2)
                        addSpace = false;
                }

                if(addSpace)
                    cleanSpace.Add(be);
            }

            cleanSpace = cleanSpace.CullDuplicates();

            return cleanSpace;
        }
    }
}