/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Query.FloorGeometry => Returns the floor geometry of a space represented by Environment Panels as a BHoM Geometry Polyline")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a closed space")]
        [Output("polyline", "BHoM Geometry Polyline representing the floor of the space")]
        public static Polyline FloorGeometry(this List<Panel> panelsAsSpace)
        {
            Panel floor = null;

            foreach(Panel panel in panelsAsSpace)
            {
                double tilt = panel.Tilt();
                if (tilt == 0 || tilt == 180)
                {
                    if(floor == null)
                        floor = panel;
                    else
                    {
                        //Multiple elements could be a floor - assign the one with the lowest Z
                        if (floor.MinimumLevel() > panel.MinimumLevel()) //TODO: Make this more robust - multiple elements could be a floor with the same z level...
                            floor = panel;
                    }
                }
            }

            if (floor == null) return null;

            Polyline floorGeometry = floor.ToPolyline();

            if (floorGeometry.ControlPoints.Count < 3)
                return null;

            return floorGeometry;
        }
    }
}
