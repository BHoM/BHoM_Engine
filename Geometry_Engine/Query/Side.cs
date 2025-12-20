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

        [Description("Determines which side of a Plane each Point lies on, returning 1 for positive side, -1 for negative side, and 0 for on the plane.")]
        [Input("plane", "The Plane to test against.")]
        [Input("points", "The collection of Points to test.")]
        [Input("tolerance", "The distance tolerance for determining if a Point is on the plane.", typeof(Length))]
        [Output("sides", "A list of integers indicating which side of the plane each point is on (1, -1, or 0).")]
        public static List<int> Side(this Plane plane, List<Point> points, double tolerance = Tolerance.Distance)
        {
            List<double> result = points.Select(x => Create.Vector(x).DotProduct(plane.Normal)).ToList();
            int[] sameSide = new int[result.Count];

            double d = -plane.Normal.DotProduct(Create.Vector(plane.Origin));

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] + d > tolerance)
                    sameSide[i] = 1;
                else if (result[i] + d < -tolerance)
                    sameSide[i] = -1;
                else
                    sameSide[i] = 0;
            }

            return sameSide.ToList();
        }

        /***************************************************/
    }
}
