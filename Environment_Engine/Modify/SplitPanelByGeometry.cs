/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;

using BH.oM.Environment.Elements;

using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Split an Environment Panel by assigning new geometry with the original core data. Returns one panel per geometry provided")]
        [Input("panel", "An Environment Panel to split")]
        [Input("polylines", "Geometry polylines to split the panel by - one panel per polyline will be returned")]
        [Output("panels", "A collection of Environment Panels split into the geometry parts provided")]
        public static List<Panel> SplitPanelByGeometry(this Panel panel, List<Polyline> polylines)
        {
            List<Panel> panels = new List<Panel>();

            foreach (Polyline p in polylines)
            {
                Panel pan = panel.ShallowClone();
                pan.ExternalEdges = p.ToEdges();
                panels.Add(pan);
            }

            return panels;
        }
    }
}



