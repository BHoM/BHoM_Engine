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

using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment.Elements;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Remove all openings which match the given name from a collection of panels")]
        [Input("panels", "The Environment Panels to have openings filtered")]
        [Input("openingName", "The name of the opening to be removed from the panels")]
        [Output("panels", "A collection of Environment Panels with the named opening removed")]
        public static List<Panel> RemoveOpeningsByName(this List<Panel> panels, string openingName)
        {
            List<Panel> rtnPanels = new List<Panel>();

            foreach (Panel p in panels)
            {
                Panel pan = p.ShallowClone();
                pan.Openings = new List<Opening>();
                foreach (Opening o in p.Openings)
                {
                    if (o.Name != openingName)
                        pan.Openings.Add(o.ShallowClone());
                }

                rtnPanels.Add(pan);
            }

            return rtnPanels;
        }
    }
}





