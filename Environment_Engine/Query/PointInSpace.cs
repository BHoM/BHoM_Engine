/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Elements;
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

        [Description("Returns a BHoM Geometry Point that is in the provided space")]
        [Input("panelsAsSpace", "The collection of Environment Panels that represent a single space to get the point in")]
        [Output("spacePoint", "A point in the space")]
        public static Point PointInSpace(this List<Panel> panelsAsSpace)
        {
            Polyline floor = panelsAsSpace.FloorGeometry();
            Point spacePnt = floor.PointInRegion();

            double height = 0.0;
            foreach (Panel p in panelsAsSpace)
                height = Math.Max(height, p.Height());

            spacePnt.Z += height / 2;

            return spacePnt;
        }
    }
}




