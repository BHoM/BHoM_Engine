/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels which are connected to the space name provided")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("spaceName", "The name of the space the panels should enclose")]
        [Output("panelsAsSpace", "A collection of Environment Panels which have the given space name as a connected space")]
        public static List<Panel> ToSpace(this List<Panel> panels, string spaceName)
        {
            List<Panel> result = panels.Where(x => x.ConnectedSpaces.Contains(spaceName)).ToList();
            
            if (result.Count == 0)
                BH.Engine.Base.Compute.RecordWarning("Could not find following space: " + spaceName);

            if (result.Count > 0 && result.Count < 5)
                BH.Engine.Base.Compute.RecordWarning("This space have less than 5 panels, we have " + result.Count + " panels in space: " + spaceName);

            return result;
        }

        [Description("Returns a nested collection of Environment Panels which are grouped by the spaces they are connected to")]
        [Input("panels", "A collection of Environment Panels")]
        [Output("panelsAsSpaces", "A nested collection of Environment Panels grouped by the space they enclose")]
        public static List<List<Panel>> ToSpaces(this List<Panel> panels)
        {
            List<List<Panel>> panelsAsSpaces = new List<List<Panel>>();

            List<string> uniqueSpaceNames = panels.UniqueSpaceNames();
            foreach (string s in uniqueSpaceNames)
                panelsAsSpaces.Add(panels.ToSpace(s));

            return panelsAsSpaces;
        }
    }
}




