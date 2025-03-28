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

using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Removes list items at given indexes, then returns the remaining objects as non-empty sublists of consecutive items.")]
        [Input("items", "A list of items to split at one or more indexes.")]
        [Input("indexes", "Indexes of items to remove.")]
        [Output("lists", "Sublists of consecutive items that remain after items at input indexes have been removed.")]
        public static List<List<T>> RemoveAndSplitAtIndexes<T>(this List<T> items, List<int> indexes)
        {
            int startIndex = 0;
            indexes.Add(items.Count);
            List<List<T>> result = new List<List<T>>();

            foreach (int i in indexes)
            {
                if (i < 0 || i > items.Count)
                    continue;

                List<T> subList = items.Skip(startIndex).Take(i - startIndex).ToList();

                if (subList.Any())
                    result.Add(subList);

                startIndex = i + 1;
            }

            return result;
        }

        /***************************************************/
    }
}

