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

        [Description("Returns a collection of BHoM Geometry Points that are in each space")]
        [Input("panelsAsSpaces", "The nested collection of Environment Panels that represent the spaces to points in")]
        [Output("spacePoints", "A collection of points in each space")]
        public static List<Point> PointsInSpaces(this List<List<Panel>> panelsAsSpaces)
        {
            List<Point> spacePnts = new List<Point>();
            foreach (List<Panel> space in panelsAsSpaces)
                spacePnts.Add(space.PointInSpace());

            return spacePnts;
        }

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

