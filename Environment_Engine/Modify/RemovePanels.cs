/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

        [Description("Removes a collection of Environment Panels from a collection of Environment Panels if they exist within the list")]
        [Input("panels", "A collection of Environment Panels to modify")]
        [Input("panelsToRemove", "The collection of Environment Panels to remove")]
        [Output("panels", "A collection of Environment Panels with the panelsToRemove excluded from the list")]
        public static List<Panel> RemovePanels(this List<Panel> panels, List<Panel> panelsToRemove)
        {
            List<Panel> clones = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());
            List<Panel> toRemove = new List<Panel>(panelsToRemove.Select(x => x.DeepClone<Panel>()).ToList());

            foreach (Panel p in toRemove)
                clones = clones.RemovePanel(p);

            return clones;
        }
    }
}






