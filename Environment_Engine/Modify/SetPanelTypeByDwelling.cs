/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Analytical.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of Environment Panels with panel type set by a dwelling perimeter.\n Walls with Bottom ControlPoints on the dwelling perimeter are set as WallExternal, the rest as WallInternal")]
        [Input("panels", "A collection of Environment Panels to set the type for")]
        [Input("regions", "A collection of IRegions to set the type by")]
        [Output("panels", "A collection of modified Environment Panels with the with panel type set by a dwelling perimeter")]
        public static List<Panel> SetPanelTypeByDwelling(List<Panel> panels, List<Dwelling> dwellings)
        {
            List<Panel> fixedPanels = new List<Panel>();
            List<Guid> handledPanels = new List<Guid>();
            for (int i = 0; i < dwellings.Count; i++)
            {
                List<Panel> wallPanels = panels.Where(x => x.Type == PanelType.Wall).ToList();
                List<Panel> floors = panels.Where(x => x.Type == PanelType.Floor).ToList();
                List<Panel> ceilings = panels.Where(x => x.Type == PanelType.Ceiling).ToList();
                List<Panel> exteriorWalls = wallPanels.Where(x => (x.Bottom() as Polyline).ControlPoints.Where(y => y.IIsOnCurve(dwellings[i].Perimeter as Polyline)).Count() == 2).ToList();
                foreach (Panel p in exteriorWalls)
                {
                    if (p != null && !handledPanels.Contains(p.BHoM_Guid))
                    {
                        p.Type = PanelType.WallExternal;
                        fixedPanels.Add(p);
                        handledPanels.Add(p.BHoM_Guid);
                    }
                }

                foreach (Panel p in floors)
                {
                    if (p != null && !handledPanels.Contains(p.BHoM_Guid))
                    {
                        p.Type = PanelType.Floor;
                        fixedPanels.Add(p);
                        handledPanels.Add(p.BHoM_Guid);
                    }
                }

                foreach (Panel p in ceilings)
                {
                    if (p != null && !handledPanels.Contains(p.BHoM_Guid))
                    {
                        p.Type = PanelType.Ceiling;
                        fixedPanels.Add(p);
                        handledPanels.Add(p.BHoM_Guid);
                    }
                }

            }

            foreach (Panel p in panels)
            {
                if (!handledPanels.Contains(p.BHoM_Guid) && p.Type == PanelType.Wall)
                {
                    p.Type = PanelType.WallInternal;
                    handledPanels.Add(p.BHoM_Guid);
                    fixedPanels.Add(p);
                }
            }

            return fixedPanels;
        }
    }

}