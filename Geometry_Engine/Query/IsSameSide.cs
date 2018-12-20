/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsSameSide(this Point p1, Plane plane, Point p2, double tolerance = Tolerance.Distance)
        {
            double d1 = plane.Normal.DotProduct(p1 - plane.Origin);
            double d2 = plane.Normal.DotProduct(p2 - plane.Origin);

            return ((d1 < -tolerance && d2 < -tolerance) || (d1 > tolerance && d2 > tolerance));
        }

        /***************************************************/

        public static bool IsSameSide(this IList<Point> points, Plane plane, double tolerance = Tolerance.Distance)
        {
            if (points.Count() < 2)
                return true;

            double d1 = plane.Normal.DotProduct(points[0] - plane.Origin);
            if (d1 >= -tolerance && d1 <= tolerance)
                return false;

            for (int i = 1; i < points.Count; i++)
            {
                double d = plane.Normal.DotProduct(points[i] - plane.Origin);
                if (d * d1 < 0 || (d >= -tolerance && d <= tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/
    }
}
