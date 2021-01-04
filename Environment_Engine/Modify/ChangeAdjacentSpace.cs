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

        [Description("Returns a single Environment Panel with an updated connected space name")]
        [Input("panel", "A single Environment Panel to change the connected space name of")]
        [Input("spaceNameToChange", "The space name to replace")]
        [Input("replacementSpaceName", "The new space name to use")]
        [Output("panel", "A modified Environment Panel with the changed connected space name")]
        public static Panel ChangeAdjacentSpace(this Panel panel, string spaceNameToChange, string replacementSpaceName)
        {
            Panel clonedPanel = panel.DeepClone<Panel>();
            for (int x = 0; x < clonedPanel.ConnectedSpaces.Count; x++)
            {
                if (clonedPanel.ConnectedSpaces[x] == spaceNameToChange)
                    clonedPanel.ConnectedSpaces[x] = replacementSpaceName;
            }

            return clonedPanel;
        }
    }
}


