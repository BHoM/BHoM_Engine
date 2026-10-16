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
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a simplified version of the input polyline where midpoints are simplified but endpoints are retained.")]
        [Input("pLine", "A polyline to simplify.")]
        [Output("polyline", "A simplified version of the input polyline where midpoints are simplified but endpoints are retained.")]
        public static Polyline SimplifyMidPoints(this Polyline pLine)
        {
            List<Point> points = pLine.ControlPoints;
            if (points.Count < 2)
                return pLine;

            List<Point> simplified = new List<Point> { points.First() };
            List<Point> midPoints = points.Skip(1).Take(points.Count - 2).ToList();
            List<Point> cleaned = new Polyline { ControlPoints = midPoints }.DiscontinuityPoints(angleTolerance: 1);

            simplified.AddRange(cleaned);
            simplified.Add(points.Last());
            return new Polyline { ControlPoints = simplified };
        }

        /***************************************************/
    }
}
