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

using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Shift items in a list forward and move overflowing items at the end back to the start of the list.")]
        [Input("list", "A list containing items to shift. For example, control points of a polyline which we want to traverse from a particular index.")]
        [Input("offset", "The number of items to move from the start to the end of the input list.")]
        [Output("list", "A list with items in the input list .")]
        public static List<T> ShiftList<T>(this List<T> list, int offset)
        {
            int steps = (offset / list.Count);
            if (offset < 0)
                steps--;
           
            offset -= steps * list.Count;
            return list.Skip(offset).Concat(list.Take(offset)).ToList();
        }

        /***************************************************/
    }
}

