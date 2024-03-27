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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel; 

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines which Environment Panels are externally facing.")]
        [Input("panels", "List of Environment Panels.")]
        [MultiOutput(0, "externalPanels", "List of external panels.")]
        [MultiOutput(1,"internalPanels","List of internal panels.")]
        public static Output<List<Panel>, List<Panel>> IsExternal(this List<Panel> panels)
         {
             List<List<Panel>> definedSpaces = panels.ToSpaces();
             List<Panel> internalPanels = new List<Panel>();
             List<Panel> externalPanels = new List<Panel>();

            foreach (Panel p in panels)
             {      
                 Vector panelNormal = p.Polyline().Normal();
                 Point panelCentre = p.Polyline().Centroid();
                 Point normalPt = panelCentre + (0.01 * panelNormal);
                 Point negNormalPt = panelCentre - (0.01 * panelNormal);
                int insideCounter = 0;
                foreach (List<Panel> space in definedSpaces)
                {
                    if (space.IsContaining(normalPt))
                    {
                        insideCounter++;
                        if (insideCounter > 1)
                            break;
                    }
                    if (space.IsContaining(negNormalPt))
                    {
                        insideCounter++;
                        if (insideCounter > 1)
                            break;
                    }
                }
                if (insideCounter > 1)
                    internalPanels.Add(p);
                else
                    externalPanels.Add(p);
            }
             return new Output<List<Panel>, List<Panel>>() { Item1 = externalPanels, Item2 = internalPanels };
        }
    }
}







