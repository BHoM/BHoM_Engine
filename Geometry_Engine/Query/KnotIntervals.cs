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

        [Description("Extracts the unique knot values that define the parametric intervals of a NURBS curve or surface, excluding repeated knots.")]
        [Input("knots", "The knot vector.")]
        [Input("degree", "The degree of the curve or surface.")]
        [Output("intervals", "The unique knot values defining the parametric intervals.")]
        public static List<double> KnotIntervals(this IList<double> knots, int degree)
        {
            List<double> knotIntervals = new List<double>();
            for (int i = degree - 1; i <= knots.Count - degree; i++)
            {
                double current = knots[i];
                if (knotIntervals.Count == 0)
                    knotIntervals.Add(current);
                else if (knotIntervals.Last() != current)
                    knotIntervals.Add(current);
            }

            return knotIntervals;
        }

        /***************************************************/
    }
}
