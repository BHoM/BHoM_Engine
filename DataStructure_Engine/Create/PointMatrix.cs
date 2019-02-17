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

using BH.oM.DataStructure;
using System.Collections.Generic;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PointMatrix<T> PointMatrix<T>(double cellSize)
        {
            return new PointMatrix<T> { CellSize = cellSize };
        }

        /***************************************************/

        public static PointMatrix<T> PointMatrix<T>(List<LocalData<T>> data, double cellSize = 1.0)
        {
            PointMatrix<T> matrix = new PointMatrix<T> { CellSize = cellSize };
            Dictionary<DiscretePoint, List<LocalData<T>>> cells = matrix.Data;

            foreach (LocalData<T> d in data)
            {
                DiscretePoint key = Create.DiscretePoint(d.Position, cellSize);
                if (!cells.ContainsKey(key))
                    cells[key] = new List<LocalData<T>>();
                cells[key].Add(d);
            }

            return matrix;
        }

        /***************************************************/
    }
}
