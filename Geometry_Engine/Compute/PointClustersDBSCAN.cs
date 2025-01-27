/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Data;
using BH.oM.Geometry;
using BH.oM.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/
        
        public static List<List<Point>> PointClustersDBSCAN(this List<Point> points, double maxDist, int minPointCount = 1)
        {
            double sqDist = maxDist * maxDist;
            Func<Point, DomainBox> toDomainBox = a => new DomainBox()
            {
                Domains = new Domain[] {
                    new Domain(a.X, a.X),
                    new Domain(a.Y, a.Y),
                    new Domain(a.Z, a.Z),
                }
            };
            Func<DomainBox, DomainBox, bool> treeFunction = (a, b) => a.SquareDistance(b) < sqDist;
            Func<Point, Point, bool> itemFunction = (a, b) => true;  // The distance between the boxes is enough to determine if a Point is in range
            return Data.Compute.DomainTreeClusters<Point>(points, toDomainBox, treeFunction, itemFunction, minPointCount);
        }

        /***************************************************/
    }
}






