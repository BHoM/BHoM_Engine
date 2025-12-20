/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the Line segment of a Polyline that the specified Point lies on.")]
        [Input("pline", "The Polyline to search for the containing segment.")]
        [Input("pt", "The Point to find the containing segment for.")]
        [Output("segment", "The Line segment that contains the Point, or an empty Line if no segment contains the Point.")]
        public static Line GetLineSegment(this Polyline pline, Point pt)
        {
            List<Line> pLineSegments = pline.SubParts();
            Line line = new Line();

            foreach (Line segment in pLineSegments)
            {
                if (BH.Engine.Geometry.Query.IsOnCurve(segment, pt))
                    line = segment;
            }

            return line;
        }

        /***************************************************/
    }
}
