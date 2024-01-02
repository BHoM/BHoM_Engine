/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Filters a list of panels based on the number of adjacencies they have")]
        [Input("panels", "A list of panels to be filtered")]
        [Input("adjacencies", "The number of Adjacencies to filter by")]
        [Output("panels", "A list of panels which have the same number of adjacencies")]
        public static List<Panel> FilterByAdjacencies(this List<Panel> panels, int adjacencies)
        {
            if (adjacencies < 0)
                Base.Compute.RecordError("Input can't be less than 0");

            if (adjacencies > 3)
                Base.Compute.RecordWarning("A panel should not have more than 3 adjacencies. Any panels returned may want to be examined for errors in their data");

            return panels.Where(x => x.ConnectedSpaces.Count == adjacencies).ToList();
        }
    }
}




