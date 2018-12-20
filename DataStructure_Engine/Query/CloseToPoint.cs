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

using BH.Engine.Geometry;
using BH.oM.DataStructure;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.DataStructure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<LocalData<T>> CloseToPoint<T>(this PointMatrix<T> matrix, Point refPt, double maxDist)
        {
            // Collect all the points within cells in range
            Vector range = new Vector { X = maxDist, Y = maxDist, Z = maxDist };
            List<LocalData<T>> inCells = matrix.SubMatrixData<T>(Create.DiscreetPoint(refPt - range, matrix.CellSize), Create.DiscreetPoint(refPt + range, matrix.CellSize));

            // Keep only points within maxDist distance of refPt
            double maxSqrDist = maxDist * maxDist;
            List<LocalData<T>> result = new List<LocalData<T>>();
            foreach (LocalData<T> tuple in inCells)
            {
                if (tuple.Position.SquareDistance(refPt) < maxSqrDist)
                    result.Add(tuple);
            }

            // Return final result
            return result;
        }

        /***************************************************/
    }
}
