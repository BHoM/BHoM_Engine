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

        [Description("BH.Engine.Environment.Query.Vertices => Returns a collection of vertices for an Environment Panel")]
        [Input("panel", "An Environment Panel")]
        [Output("points", "A collection of BHoM Geometry Points which are the vertices of the panel")]
        public static List<Point> Vertices(this Panel panel)
        {
            return panel.ToPolyline().IControlPoints();
        }

        [Description("BH.Engine.Environment.Query.Vertices => Returns a collection of vertices for a collection of Environment Panels representing a space")]
        [Input("panelsAsSpace", "A collection of Environment Panels representing a space")]
        [Output("points", "A collection of BHoM Geometry Points which are the vertices of the space")]
        public static List<Point> Vertices(this List<Panel> panelsAsSpace)
        {
            List<Point> vertexPts = new List<Point>();

            foreach (Panel be in panelsAsSpace)
                vertexPts.AddRange(be.Vertices());

            return vertexPts;
        }
    }
}
