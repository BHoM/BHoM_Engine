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

using System.Collections.Generic;
using BH.oM.Environment.Elements;
using System.Linq;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Removes a single Environment Panel from a collection of Environment Panels if it exists within the list")]
        [Input("panels", "A collection of Environment Panels to modify")]
        [Input("panelToRemove", "The Environment Panel to remove")]
        [Output("panels", "A collection of Environment Panels with the panelToRemove excluded from the list")]
        public static List<Panel> RemovePanel(this List<Panel> panels, Panel panelToRemove)
        {
            List<Panel> clones = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());
            List<Panel> rtnElements = clones.Where(x => x.BHoM_Guid != panelToRemove.BHoM_Guid).ToList();

            return rtnElements;
        }
    }
}





