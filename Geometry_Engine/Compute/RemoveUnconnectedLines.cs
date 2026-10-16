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
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Removes lines that are not connected at both ends.")]
        [Input("lines", "List of lines.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 3.")]
        [Output("connectedLines", "List of lines which all have connections at both ends.")]
        public static List<Line> RemoveUnconnectedLines(this List<Line> lines, int decimalPlaces = 3)
        {
            if (lines == null)
            {
                Base.Compute.RecordError("Input of lines to clean up cannot be null.");
                return new List<Line>();
            }

            List<Point> endPoints = lines.Select(x => x.EndPoint().RoundCoordinates(decimalPlaces)).ToList();
            List<Point> startPoints = lines.Select(x => x.StartPoint().RoundCoordinates(decimalPlaces)).ToList();
            endPoints.AddRange(startPoints);
            List<Point> uniquePoints = endPoints.Where(x => endPoints.Where(y => y == x).Count() == 1).ToList();
            return lines.Where(x => !uniquePoints.Contains(x.EndPoint().RoundCoordinates(decimalPlaces)) && !uniquePoints.Contains(x.StartPoint().RoundCoordinates(decimalPlaces))).ToList();
        }

        /***************************************************/
    }
}
