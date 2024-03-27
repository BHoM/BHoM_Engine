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
using BH.oM.Base;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;

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

        [Description("Copies a Panel into a new object")]
        [Input("panel", "An Environment Panel to copy from")]
        [Output("panel", "The copied Environment Panel")]
        public static Panel Copy(this Panel panel)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot copy a null panel.");
                return null;
            }

            Panel aPanel = panel.ShallowClone(true);
            aPanel.ExternalEdges = new List<Edge>(panel.ExternalEdges);
            aPanel.Openings = new List<Opening>(panel.Openings);
            aPanel.ConnectedSpaces = new List<string>(panel.ConnectedSpaces);
            aPanel.Construction = panel.Construction;
            aPanel.Type = panel.Type;

            if (panel.Fragments != null && panel.Fragments.Count > 0)
                aPanel.Fragments = new FragmentSet(panel.Fragments);
            else
                aPanel.Fragments = new FragmentSet();

            return aPanel;
        }
    }
}





