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
using BH.oM.Environment;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return a collection of panels which have invalid connected spaces")]
        [Input("panels", "A collection of environment panels")]
        [Output("invalidPanels", "A collection of panels which are invalid")]
        public static List<Panel> InvalidPanels(this List<Panel> panels)
        {
            List<Panel> rtn = new List<Panel>();

            foreach (Panel p in panels)
            {
                //check for external
                if ((p.Type == PanelType.Roof || p.Type == PanelType.WallExternal || p.Type == PanelType.FloorExposed || p.Type == PanelType.FloorRaised || p.Type == PanelType.SlabOnGrade || p.Type == PanelType.UndergroundSlab || p.Type == PanelType.UndergroundWall) && p.ConnectedSpaces.Count != 1)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("external panels must have 1 adj space");
                    rtn.Add(p);
                }

                //check for shade
                if ((p.Type == PanelType.Shade || p.Type == PanelType.SolarPanel) && p.ConnectedSpaces.Count != 0)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("shade  must have 0 adj space");
                    rtn.Add(p);
                }

                //check for internal
                if ((p.Type == PanelType.Ceiling || p.Type == PanelType.WallInternal || p.Type == PanelType.FloorInternal || p.Type == PanelType.UndergroundCeiling) && p.ConnectedSpaces.Count != 2 )
                {
                   
                    BH.Engine.Reflection.Compute.RecordWarning("internal panel must have 2 different adj space");
                    rtn.Add(p);

                }

                //check for internal to make sure we have two different space
                if ( p.ConnectedSpaces.Count == 2 && p.ConnectedSpaces[0] == p.ConnectedSpaces[1])
                {

                    BH.Engine.Reflection.Compute.RecordWarning("two the same spaces on both side");
                    rtn.Add(p);

                }

                //check if geometry area is too small
                if (p.Area() < 0.15)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Panel area is possibly to small, area is less than 0.15");
                    rtn.Add(p);
                }

                //check if on dimension is to short
                foreach (Edge e in p.ExternalEdges)
                {
                    if (e.Polyline().Length() < 0.15)
                    {
                        BH.Engine.Reflection.Compute.RecordWarning("One of panel poluline edge is less than 0.15");
                        rtn.Add(p);
                        break;
                    }
                }

                //check if panel polyline is closed
                if (Geometry.Query.IsClosed(p.Polyline()) == false)
                    BH.Engine.Reflection.Compute.RecordWarning("Panel polyline is not closed");
                rtn.Add(p);

                //check if self intesect
                if (Geometry.Query.IsClosed(p.Polyline()) == false)
                    BH.Engine.Reflection.Compute.RecordWarning("Panel polyline intersect");
                rtn.Add(p);
            }

            return rtn;
        }
    }
}
