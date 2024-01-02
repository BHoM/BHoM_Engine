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

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels containing the given search point.")]
        [Input("panels", "A collection of Environment Panels.")]
        [Input("searchPoint", "The BHoM Geometry Point to search by.")]
        [Input("acceptOnEdge", "Set whether or not to accept points on the edge of panels, default false.")]
        [Input("tolerance", "Set the distance tolerance for containing a point, default is equal to that given by BH.oM.Geometry.Tolerance.Distance.")]
        [MultiOutput(0, "panelsContainingPoint", "A collection of Environment Panel where the external edges contain the given search point.")]
        [MultiOutput(1, "panelsNotContainingPoint", "A collection of Environment Panel where the external edges do NOT contain the given search point.")]
        public static Output<List<Panel>, List<Panel>> PanelsContainingPoint(this List<Panel> panels, Point searchPoint, bool acceptOnEdge = false, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            var panelsContainingPoint = panels.Where(x => x.Polyline().IsContaining(new List<Point>() { searchPoint }, acceptOnEdge, tolerance)).ToList();
            var guids = panelsContainingPoint.Select(x => x.BHoM_Guid);
            var panelsNotContainingPoint = panels.Where(x => !guids.Contains(x.BHoM_Guid)).ToList();

            return new Output<List<Panel>, List<Panel>>()
            {
                Item1 = panelsContainingPoint,
                Item2 = panelsNotContainingPoint,
            };
        }
    }
}




