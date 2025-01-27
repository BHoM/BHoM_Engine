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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Spatial.SettingOut;

using BH.Engine.Base;

using BH.oM.Physical.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels queried from a collection of Physical Objects (walls, floors, etc.)")]
        [Input("physicalObjects", "A collection of Physical Objects to query Environment Panels from")]
        [Output("panels", "A collection of Environment Panels from Physical Objects")]
        public static List<Panel> PanelsFromPhysical(this List<BH.oM.Physical.Elements.ISurface> physicalObjects)
        {
            List<Panel> panels = new List<Panel>();

            foreach (BH.oM.Physical.Elements.ISurface srf in physicalObjects)
            {
                Panel p = new Panel();
                p.Name = srf.Name;
                p.Construction = srf.Construction;
                p.ExternalEdges = srf.Location.IExternalEdges().ToEdges();
                p.Openings = srf.Openings.OpeningsFromPhysical();
                if (p.Openings == null) p.Openings = new List<Opening>();
                p.Type = (srf is BH.oM.Physical.Elements.Wall ? PanelType.Wall : (srf is BH.oM.Physical.Elements.Roof ? PanelType.Roof : PanelType.Floor));
                panels.Add(p);
            }

            return panels;
        }
    }
}





