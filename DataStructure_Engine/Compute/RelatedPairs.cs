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
using System;
using System.Collections.Generic;

namespace BH.Engine.DataStructure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Tuple<LocalData<T>, LocalData<T>>> RelatedPairs<T>(this PointMatrix<T> matrix, double minDist, double maxDist)
        {
            int range = (int)Math.Ceiling(maxDist / matrix.CellSize);
            double minSqrDist = minDist * minDist;
            double maxSqrDist = maxDist * maxDist;

            List<Tuple<LocalData<T>, LocalData<T>>> result = new List<Tuple<LocalData<T>, LocalData<T>>>();
            foreach (KeyValuePair<DiscreetPoint, List<LocalData<T>>> kvp in matrix.Data)
            {
                DiscreetPoint k = kvp.Key;
                List<LocalData<T>> closePoints = matrix.SubMatrixData<T>(
                    new DiscreetPoint { X = k.X - range, Y = k.Y - range, Z = k.Z - range },
                    new DiscreetPoint { X = k.X + range, Y = k.Y + range, Z = k.Z + range });

                foreach (LocalData<T> value in kvp.Value)
                {
                    foreach (LocalData<T> other in closePoints)
                    {
                        double sqrDist = value.Position.SquareDistance(other.Position);
                        if (sqrDist < maxSqrDist && sqrDist > minSqrDist)
                            result.Add(new Tuple<LocalData<T>, LocalData<T>>(value, other));
                    }
                }
            }

            return result;
        }

        /***************************************************/
    }
}
