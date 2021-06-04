/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a rectangular 2D array from a 2D matrix")]
        [Input("matrix", "A 2D matrix")]
        [Output("array2D", "A rectangular 2D array")]
        public static IEnumerable<IEnumerable<double>> ToArray2D(this double[,] matrix)
        {
            return matrix.Cast<double>()
                .Select((x, i) => new { x, index = i / matrix.GetLength(1) })
                .GroupBy(x => x.index)
                .Select(x => x.Select(s => s.x).ToList())
                .ToList();
        }

        [Description("Returns a rectangular 2D array from a 2D matrix")]
        [Input("matrix", "A 2D matrix")]
        [Output("array2D", "A rectangular 2D array")]
        public static IEnumerable<IEnumerable<int>> ToNestedList(this int[,] matrix)
        {
            return matrix.Cast<int>()
                .Select((x, i) => new { x, index = i / matrix.GetLength(1) })
                .GroupBy(x => x.index)
                .Select(x => x.Select(s => s.x).ToList())
                .ToList();
        }

        [Description("Returns a rectangular 2D array from a 2D matrix")]
        [Input("matrix", "A 2D matrix")]
        [Output("array2D", "A rectangular 2D array")]
        public static IEnumerable<IEnumerable<bool>> ToNestedList(this bool[,] matrix)
        {
            return matrix.Cast<bool>()
                .Select((x, i) => new { x, index = i / matrix.GetLength(1) })
                .GroupBy(x => x.index)
                .Select(x => x.Select(s => s.x).ToList())
                .ToList();
        }
    }
}

