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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the direction of the dominant edge of a polyline.")]
        [Input("polyline", "Polyline whose edge directions are evaluated.")]
        [Input("tolerance", "Minimum edge length.")]
        [Input("angleTolerance", "Angular tolerance used when comparing edge directions for parallelism.")]
        [Output("direction", "Direction vector of the dominant edge.")]
        public static Vector DominantEdgeDirection(this Polyline polyline, double tolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            List<Line> edges = polyline.SubParts().Where(x => x != null && x.Length() > tolerance).ToList();
            Dictionary<Vector, double> dirLen = new Dictionary<Vector, double>();
            foreach (Line edge in edges)
            {
                Vector direction = edge.Direction();
                Vector existDir = dirLen.Keys.FirstOrDefault(x => x.IsParallel(direction, angleTolerance) != 0);

                if (existDir != null)
                    dirLen[existDir] += edge.Length();
                else
                    dirLen[direction] = edge.Length();
            }

            return dirLen.OrderByDescending(x => x.Value).First().Key;
        }
    }
}
