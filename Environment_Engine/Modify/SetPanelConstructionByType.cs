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

using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Environment;
using BH.oM.Geometry;

using BH.oM.Physical.Constructions;

using System.Collections.Generic;
using System.Linq;
using BH.oM.Environment.Fragments;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;
using System;
using BH.oM.Physical.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Update a panel construction based on the panel type")]
        [Input("panels", "A collection of Environment Panels to update the constructions of")]
        [Input("newConstruction", "The new construction to assign to the panels")]
        [Input("panelType", "The type of the panels to update")]
        [Output("panels", "The collection of Environment Panels with updated constructions")]
        public static List<Panel> SetPanelConstructionByType(this List<Panel> panels, IConstruction newConstruction, PanelType panelType)
        {
            List<Panel> clones = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());

            foreach (Panel clone in clones)
            {  
                if (clone.Type == panelType)
                {
                    clone.Construction = newConstruction; 
                }
            }

            return clones;
        }
    }
}




