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

using System;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Environment.Elements;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Environment
{
    public static partial class Convert
    {
        [Description("Applies an opening to the full area of a panel. Existing openings will be removed/replaced by the opening generated to match the size of the panel.")]
        [Input("panel", "Panel to generate an opening from.")]
        [Input("openingType", "Speficy the type of the opening to convert the panel into.")]
        [Output("panel", "Returns the panel with the opening applied.")]
        public static Panel OpeningFromPanel(this Panel panel, OpeningType openingType = OpeningType.Window)
        {
            Opening o = new Opening();
            o.Edges = new List<Edge>(panel.ExternalEdges);
            o.Type = openingType;
            panel.Openings = new List<Opening>();
            panel.Openings.Add(o);

            return panel;
        }
    }
}


