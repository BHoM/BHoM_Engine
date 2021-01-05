/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using System.Linq;
using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels where any connected spaces which are detailed within the spaceNamesToChange are replaced by a replacementSpaceName. The spaceNamesToChange and replacementSpaceNames should match length to provide a 1:1 change")]
        [Input("panels", "A collection of Environment Panels to update the connected space names of")]
        [Input("spaceNamesToChange", "A collection of space names which should be updated")]
        [Input("replacementSpaceNames", "A collection of space names to replace with")]
        [Output("panels", "A collection of Environment Panels modified so that space names are changed as appropriate")]
        public static List<Panel> ChangeAdjacentSpaces(this List<Panel> panels, List<string> spaceNamesToChange, List<string> replacementSpaceNames)
        {
            List<Panel> clonedPanels = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());
            if (spaceNamesToChange.Count != replacementSpaceNames.Count)
            {
                BH.Engine.Reflection.Compute.RecordError("Please ensure the number of replacement space names matches the number of changing space names. Panels returned without change");
                return clonedPanels;
            }

            for (int x = 0; x < spaceNamesToChange.Count; x++)
            {
                for (int a = 0; a < clonedPanels.Count; a++)
                    clonedPanels[a] = ChangeAdjacentSpace(clonedPanels[a], spaceNamesToChange[x], replacementSpaceNames[x]);
            }

            return clonedPanels;
        }
    }
}


