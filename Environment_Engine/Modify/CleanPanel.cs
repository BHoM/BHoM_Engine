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

using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Environment;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Returns a list of panels that has been cleaned from short segments and insignificant vertices")]
        [Input("panels", "A list of panels that will be cleaned")]
        [Input("angleTolerance", "The tolerance of the angle that defines a straight line. Default is set to the value defined by BH.oM.Geometry.Tolerance.Angle")]
        [Input("minimumSegmentLength", "The length of the smallest allowed segment. Segments smaller than this will be removed. Default is set to the value defined by BH.oM.Geometry.Tolerance.Distance")]
        [Output("cleanedPanels", "A list of panels that has been cleaned")]
        public static List<Panel> CleanPanel(this List<Panel> panels, double angleTolerance = Tolerance.Angle, double minimumSegmentLength = Tolerance.Distance)
        {
            List<Panel> clonedPanels = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());

            foreach (Panel p in clonedPanels)
                p.ExternalEdges = p.Polyline().CleanPolyline(angleTolerance, minimumSegmentLength).ToEdges();

            return clonedPanels;
        }
    }
}






