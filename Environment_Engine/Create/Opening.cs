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

        [Description("BH.Engine.Environment.Create.Opening => Returns an Environment Opening object")]
        [Input("name", "The name of the opening, default empty string")]
        [Input("externalEdges", "A collection of Environment Edge objects which define the external boundary of the opening, default null")]
        [Input("innerEdges", "A collection of Environment Edge objects which define the internal boundary of the opening, default null")]
        [Input("frameConstruction", "A construction object providing construction information about the frame of the opening, default null")]
        [Input("openingConstruction", "A construction object providing construction information about the opening - typically glazing construction, default null")]
        [Input("type", "The type of opening from the Opening Type enum, default undefined")]
        [Output("An Environment Opening object")]
        public static Opening Opening(string name = "", List<Edge> externalEdges = null, List<Edge> innerEdges = null, IConstruction frameConstruction = null, IConstruction openingConstruction = null, OpeningType type = OpeningType.Undefined)
        {
            externalEdges = externalEdges ?? new List<Edge>();
            innerEdges = innerEdges ?? new List<Edge>();

            return new Opening
            {
                Name = name,
                Edges = externalEdges,
                InnerEdges = innerEdges,
                FrameConstruction = frameConstruction,
                OpeningConstruction = openingConstruction,
                Type = type,
            };
        }
    }
}
