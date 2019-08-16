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

using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("merges the properties two Environment Panels together and returns a copied panel with the smallest area")]
        [Input("panel1", "An Environment Panel to merge from")]
        [Input("panel2", "A second Environment Panel to merge from")]
        [Output("mergedPanel", "The Environment Panel with the smallest area of the two provided but with the combined properties of both")]
        public static Panel MergePanels(this Panel panel1, Panel panel2)
        {
            Panel rtnPanel = null;

            if(panel1.Area() > panel2.Area())
                rtnPanel = panel2.Copy();
            else
                rtnPanel = panel1.Copy();

            List<string> connectedSpaces = panel1.ConnectedSpaces;
            connectedSpaces.AddRange(panel2.ConnectedSpaces);
            connectedSpaces = connectedSpaces.Where(x => !x.Equals("-1")).ToList();
            connectedSpaces = connectedSpaces.Distinct().ToList();

            rtnPanel.ConnectedSpaces = new List<string>(connectedSpaces);
            
            return rtnPanel;
        }
    }
}
