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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BH.oM.Environment.Elements;
using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {        
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of Environment Panel with panel type set by intersecting lines.")]
        [Input("panels", "A collection of Environment Panels to set the type for.")]
        [Input("intersectingLines", "A collection of lines intersecting the outline of panels.")]
        [Input("panelType", "The panel type to set.")]
        [Input("minTilt", "The minimum tilt to filter the collection of panels by.")]
        [Input("maxTilt", "The maximum tilt to filter the collection of panels by.")]
        [Input("distanceTolerance", "Distance tolerance for calculating discontinuity points, default is set to BH.oM.Geometry.Tolerance.Distance.")]
        [Input("angleTolerance", "Angle tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Angle.")]
        [Output("panels", "A collection of modified Environment Panels with the type set by intsersecting lines.")]
        public static void SetPanelTypeByIntersectingLines(this List<Panel> panels, List<Line> intersectingLines, PanelType panelType, double minTilt = 88, double maxTilt = 92, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            if (panels == null || intersectingLines == null)
                return;

            foreach (Line l in intersectingLines)
            {
                for (int i = 0; i < panels.Count; i++)
                {
                    double tilt = panels[i].Tilt(distanceTolerance, angleTolerance);
                    if (tilt >= minTilt && tilt <= maxTilt && panels[i].Polyline().LineIntersections(l).Count > 0)
                    {
                        panels[i].Type = panelType;
                    }
                }
            }
        }
    }
}


