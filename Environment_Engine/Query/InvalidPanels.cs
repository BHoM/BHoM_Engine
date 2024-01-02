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
using BH.oM.Environment;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return a collection of panels which we identified as Invalid due to wrong data (checks: 1-external adj space, 2-shade check, 3-internal adj space, 4-internal two diff spaces, 5-area to small, 6-segment to short, 7-polyline closed, 8-polyline interesect")]
        [Input("panels", "A collection of environment panels")]
        [Input("panelArea", "A min panel area to return warning, default=0.15")]
        [Input("panelEdgeLength", "A minimum panel edge length for valid Panel, smaller value will return warning, default=0.15")]
        [Output("invalidPanels", "A collection of panels which are invalid. Grouped in sublist for each of 8 checks that we perform, see description")]
        public static List<List<Panel>> InvalidPanels(this List<Panel> panels, double panelArea = 0.15, double panelEdgeLength = 0.15)
        {
            List<List<Panel>> listPanels = new List<List<Panel>>();

            for (int i = 0; i < 8; i++)
                listPanels.Add(new List<Panel>());

            foreach (Panel p in panels)
            {
                //check external panels and make sure we have one adj space
                if ((p.Type == PanelType.Roof || p.Type == PanelType.WallExternal || p.Type == PanelType.FloorExposed || p.Type == PanelType.FloorRaised || p.Type == PanelType.SlabOnGrade || p.Type == PanelType.UndergroundSlab || p.Type == PanelType.UndergroundWall) && p.ConnectedSpaces.Count != 1)
                    listPanels[0].Add(p);

                //check if panel shade has no adj spaces
                if ((p.Type.IsShade() || p.Type == PanelType.SolarPanel) && p.ConnectedSpaces.Count != 0)
                    listPanels[1].Add(p);

                //check if internal panles got two adj spaces
                if ((p.Type == PanelType.Ceiling || p.Type == PanelType.WallInternal || p.Type == PanelType.FloorInternal || p.Type == PanelType.UndergroundCeiling) && p.ConnectedSpaces.Count != 2)
                    listPanels[2].Add(p);

                //check for internal to make sure we have two different space
                if (p.ConnectedSpaces.Count == 2 && p.ConnectedSpaces[0] == p.ConnectedSpaces[1])
                    listPanels[3].Add(p);

                //check if geometry area is too small
                if (BH.Engine.Geometry.Query.Area(p.Polyline()) < panelArea)
                    listPanels[4].Add(p);
                else
                {
                    //check if one edge is to short
                    foreach (Edge e in p.ExternalEdges)
                    {
                        if (e.Polyline().Length() < panelEdgeLength)
                        {
                            listPanels[5].Add(p);
                            break;
                        }
                    }
                }

                //check if panel polyline is closed
                if (BH.Engine.Geometry.Query.IsClosed(p.Polyline()) != true)
                    listPanels[6].Add(p);

                //check if self intersect
                if (BH.Engine.Geometry.Query.IIsSelfIntersecting(p.Polyline()) != false)
                    listPanels[7].Add(p);
            }

            if (listPanels[0].Count > 0)
                BH.Engine.Base.Compute.RecordWarning("External panels must have one adjacent space");
            if (listPanels[1].Count > 0)
                BH.Engine.Base.Compute.RecordWarning("Shade must have no adjacent space");
            if (listPanels[2].Count > 0)
                BH.Engine.Base.Compute.RecordWarning("Internal panel must have two different adj spaces");
            if (listPanels[3].Count > 0)
                BH.Engine.Base.Compute.RecordWarning("Internal panels have two adjacent spaces in their data which is the same space name");
            if (listPanels[4].Count > 0)
                BH.Engine.Base.Compute.RecordWarning("Panel area is possibly to small, area is less than panel area");
            if (listPanels[5].Count > 0)
                BH.Engine.Base.Compute.RecordWarning("One of panel edges is less than panel edge length");
            if (listPanels[6].Count > 0)
                BH.Engine.Base.Compute.RecordWarning("Panel polyline is not closed");
            if (listPanels[7].Count > 0)
                BH.Engine.Base.Compute.RecordWarning("Panel polyline is self intersecting");

            return listPanels;
        }
    }
}





