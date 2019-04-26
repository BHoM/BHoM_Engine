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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Physical.Properties.Construction;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an Environment Panel object")]
        [Input("name", "The name of the panel, default empty string")]
        [Input("externalEdges", "A collection of Environment Edge objects which define the external boundary of the panel, default null")]
        [Input("openings", "A collection of Environment Opening objects, default null")]
        [Input("construction", "A construction object providing layer and material information for the panel, default null")]
        [Input("type", "The type of panel from the Panel Type enum, default undefined")]
        [Input("connectedSpaces", "A collection of the spaces the panel is connected to, default null")]
        [Output("panel", "An Environment Panel object")]
        public static Panel Panel(string name = "", List<Edge> externalEdges = null, List<Opening> openings = null, IConstruction construction = null, PanelType type = PanelType.Undefined, List<string> connectedSpaces = null)
        {
            externalEdges = externalEdges ?? new List<Edge>();
            openings = openings ?? new List<Opening>();
            connectedSpaces = connectedSpaces ?? new List<string>();

            return new Panel
            {
                Name = name,
                ExternalEdges = externalEdges,
                Openings = openings,
                Construction = construction,
                Type = type,
                ConnectedSpaces = connectedSpaces,
            };
        }
    }
}
