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

using BH.oM.Base.Attributes;
using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Replaces the Connected Space Name for the panel if the panel was connected to the old space. If the panel was not connected to the old space, then the space names are not affected. E.G. To replace the space name Room1 to Room2, provide 'Room1' as the old space name (matching case), and 'Room2' as the new space name. If the panel is not connected to Room1, the panel will not be modified.\n\n" +
            "This will go through every panels connected spaces and check to see if they are within the provided old space names. If they are, they will be updated with the corresponding item in the new space names matching the index of the old space names. Therefore it is vital to ensure the new space names order matches the order of the old space names.")]
        [Input("panels", "The collection of Environment Panels to update space names for.")]
        [Input("oldSpaceNames", "The old space names to replace for the panel.")]
        [Input("newSpaceNames", "The replacement space names for the panel.")]
        [Output("panel", "The panel with updated space names if the panel contained the old space name.")]
        public static List<Panel> ReplaceConnectedSpaceName(this List<Panel> panels, List<string> oldSpaceNames, List<string> newSpaceNames)
        {
            if (panels == null)
                return panels;

            if(oldSpaceNames.Count != newSpaceNames.Count)
            {
                BH.Engine.Base.Compute.RecordError("Length of the list of the old space names must be equal to the length of the list of new space names to ensure appropriate mapping between the old and new. The index of each old space name is matched to the index of the new space name to become the replacement. No changes have been made.");
                return panels;
            }

            List<Panel> rtnPanels = new List<Panel>();

            foreach(Panel p in  panels)
            {
                if(p.ConnectedSpaces == null)
                {
                    rtnPanels.Add(p);
                    continue;
                }

                for (int x = 0; x < p.ConnectedSpaces.Count; x++)
                {
                    if (oldSpaceNames.Contains(p.ConnectedSpaces[x]))
                    {
                        int index = oldSpaceNames.IndexOf(p.ConnectedSpaces[x]);
                        p.ConnectedSpaces[x] = newSpaceNames[index];
                    }
                }

                rtnPanels.Add(p);
            }

            return rtnPanels;
        }
    }
}


