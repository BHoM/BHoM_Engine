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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BH.oM.Environment.Elements;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Base.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Modifies a collection of panels to ensure that their normal is pointing away from the space that they enclose.\nAny openings on the panels will also be tested to ensure the normal is pointing away from the space. This is for a single space's worth of panels.\nIf wishing to use this on multiple spaces, use the ToSpaces component to split the panels per space, plug that into here and flatten the output.")]
        [Input("panelsAsSpace", "A collection of Environment Panels to check normal direction for.")]
        [Output("panelsAsSpace", "A collection of modified Environment Panels with normal away from space.")]
        public static void FlipPanels(this List<Panel> panelsAsSpace)
        {
            if (panelsAsSpace == null)
                return;

            foreach (Panel p in panelsAsSpace)
            {
                if (!p.NormalAwayFromSpace(panelsAsSpace))
                    p.ExternalEdges = p.Polyline().Flip().ToEdges();

                for (int x = 0; x < p.Openings.Count; x++)
                {
                    if (!p.Openings[x].Polyline().NormalAwayFromSpace(panelsAsSpace))
                        p.Openings[x].Edges = p.Openings[x].Polyline().Flip().ToEdges();
                }
            }
        }
    }
}



