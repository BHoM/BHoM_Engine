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

using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Removes duplicate lines from the collection")]
        [Input("lines", "The nested collection of lines to cull duplicates from")]
        [Output("lines", "A collection of lines with no duplicates")]
        public static List<Line> CullDuplicateLines(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            List<Line> result = lines.Select(l => l).ToList();
            
            for (int i = lines.Count - 2; i >= 0; i--)
            {
                for (int j = lines.Count - 1; j > i; j--)
                {
                    Line l1 = lines[i];
                    Line l2 = lines[j];
                    if ((l1.Start.SquareDistance(l2.Start) <= sqTol && l1.End.SquareDistance(l2.End) <= sqTol) || (l1.Start.SquareDistance(l2.End) <= sqTol && l1.End.SquareDistance(l2.Start) <= sqTol))
                        lines.RemoveAt(j);
                }
            }

            return lines;
        }
    }
}




