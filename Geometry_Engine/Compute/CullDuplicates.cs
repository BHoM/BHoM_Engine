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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Culls all duplicate points in the list by grouping all points that are within the maximum provided distance from one another and returning the average point in each group. A DBScan algorithm is used to determine if considered duplicates. This means that chains of close points will be determined the same, even if two individual points in this chain can have a distance between them that is larger than the tolerance, as long as they both are within tolerance of another point in the same group.")]
        [Input("points", "The collection of points to cull duplicates from.")]
        [Input("maxDist", "The maximum allowable distance between two points for them to be deemed the same point.", typeof(Length))]
        [Output("points", "The collection of points with all duplicates removed. For cases when points have been deemed duplicates of each other, average point per each group of duplicates will be returned.")]
        public static List<Point> CullDuplicates(this List<Point> points, double maxDist = Tolerance.Distance)
        {
            int count = points.Count;

            if (count <= 1)
                return points;
            if (count == 2)
            {
                if (points[0].SquareDistance(points[1]) < maxDist * maxDist)
                    return new List<Point> { (points[0] + points[1]) / 2.0 };
                else
                    return points;
            }
            List<List<Point>> clusteredPoints = points.PointClustersDBSCAN(maxDist);
            return clusteredPoints.Select(x => x.Average()).ToList();
        }

        /***************************************************/
    }
}




