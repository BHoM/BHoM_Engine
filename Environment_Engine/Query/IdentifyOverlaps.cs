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

using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment.Elements;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels overlap with the given element")]
        [Input("panel", "An Environment Panel to check against")]
        [Input("panelsToCompare", "A collection of Environment Panels to find overlaps from")]
        [Output("panels", "A collection of Environment Panels that overlap with the first panel")]
        public static List<Panel> IdentifyOverlaps(this Panel panel, List<Panel> panelsToCompare)
        {
            List<Panel> overlappingElements = new List<Panel>();

            foreach(Panel p in panelsToCompare)
            {
                if(panel.BHoM_Guid != p.BHoM_Guid && panel.BooleanIntersect(p))
                    overlappingElements.Add(p);
            }

            return overlappingElements;
        }
    }
}




