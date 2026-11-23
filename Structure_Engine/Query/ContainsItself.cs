/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Loads;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a LoadCombination contains itself, directly or through nested LoadCombinations.")]
        [Input("combination", "The LoadCombination to check.")]
        [Output("containsItself", "True if the LoadCombination contains itself.")]
        public static bool ContainsItself(this LoadCombination combination)
        {
            if (combination.IsNull())
                return false;

            // Compare by Number, as packages identify cases by number. Unnumbered combinations are compared by reference.
            bool byNumber = combination.Number > 0;

            // Only loops back to this combination are found. Each combination is visited once, so other loops still end.
            HashSet<LoadCombination> visited = new HashSet<LoadCombination> { combination };
            Stack<LoadCombination> toVisit = new Stack<LoadCombination>();
            toVisit.Push(combination);

            while (toVisit.Count > 0)
            {
                LoadCombination current = toVisit.Pop();
                if (current.LoadCases == null)
                    continue;

                IEnumerable<LoadCombination> nestedCombinations = current.LoadCases
                    .Select(item => item?.Item2)
                    .OfType<LoadCombination>();
                foreach (LoadCombination nested in nestedCombinations)
                {
                    if (byNumber ? nested.Number == combination.Number : ReferenceEquals(nested, combination))
                        return true;

                    if (visited.Add(nested))
                        toVisit.Push(nested);
                }
            }

            return false;
        }

        /***************************************************/
    }
}

