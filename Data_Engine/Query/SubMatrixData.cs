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

using BH.oM.Data.Collections;
using System.Collections.Generic;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<LocalData<T>> SubMatrixData<T>(this PointMatrix<T> matrix, DiscretePoint minCell, DiscretePoint maxCell)
        {
            Dictionary<DiscretePoint, List<LocalData<T>>> data = matrix.Data;

            List<LocalData<T>> inCells = new List<LocalData<T>>();
            for (int x = minCell.X; x <= maxCell.X; x++)
            {
                for (int y = minCell.Y; y <= maxCell.Y; y++)
                {
                    for (int z = minCell.Z; z <= maxCell.Z; z++)
                    {
                        DiscretePoint key = new DiscretePoint { X = x, Y = y, Z = z };
                        if (data.ContainsKey(key))
                        {
                            foreach (LocalData<T> comp in data[key])
                                inCells.Add(comp);
                        }
                    }
                }
            }

            return inCells;
        }

        /***************************************************/
    }
}




