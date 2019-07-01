/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment.Elements;

using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<Panel>> FixNormal(this List<List<Panel>> panelsAsSpaces)
        {
            List<List<Panel>> returnPanels = new List<List<Panel>>();

            foreach(List<Panel> space in panelsAsSpaces)
            {
                returnPanels.Add(new List<Panel>());
                foreach(Panel p in space)
                    returnPanels.Last().Add(p.GetShallowClone() as Panel);
            }

            foreach(List<Panel> space in returnPanels)
            {
                string connectedSpaceName = space.ConnectedSpaceName();
                foreach(Panel p in space)
                {
                    if(p.ConnectedSpaces.Count > 0 && p.ConnectedSpaces[0] == connectedSpaceName)
                    {
                        if(!p.NormalAwayFromSpace(space))
                            p.ExternalEdges = p.Polyline().Flip().ToEdges();
                    }
                }
            }

            return returnPanels;
        }

    }
}
