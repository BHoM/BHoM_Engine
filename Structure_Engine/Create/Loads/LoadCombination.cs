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

using BH.oM.Structure.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a LoadCombination from two equal length lists of cases and factors. The cases can be Loadcases, other LoadCombinations, or a mix of both, so a combination can be built from combinations.")]
        [Input("name", "The name of the load combination. This is required for various structural packages to create the object.")]
        [InputFromProperty("number")]
        [Input("cases", "The Loadcases and/or LoadCombinations to be assigned to the LoadCombination. This list should have the exact same length as the factors.")]
        [Input("factors", "The factor to scale each of the cases with. This list should have the exact same length as the cases.")]
        [Input("excludeZeroFactorCases", "If set to true, any case that has a corresponding factor of 0 will NOT be added to the combination.")]
        [Output("comb", "The created LoadCombination.")]
        public static LoadCombination LoadCombination(string name, int number, IReadOnlyList<ICase> cases, IReadOnlyList<double> factors, bool excludeZeroFactorCases = true)
        {
            if (cases == null || factors == null)
            {
                Base.Compute.RecordError("Cannot create a LoadCombination from null cases or factors.");
                return null;
            }

            if (cases.Count != factors.Count)
            {
                Base.Compute.RecordError("LoadCombinations require the same number of cases and factors.");
                return null;
            }

            List<Tuple<double, ICase>> factoredCases = new List<Tuple<double, ICase>>();
            bool nullCaseFound = false;

            for (int i = 0; i < cases.Count; i++)
            {
                if (cases[i] == null)
                    nullCaseFound = true;
                else if (!excludeZeroFactorCases || factors[i] != 0)
                    factoredCases.Add(new Tuple<double, ICase>(factors[i], cases[i]));
            }

            if (nullCaseFound)
                Base.Compute.RecordWarning("Null cases were ignored when creating the LoadCombination.");

            return new LoadCombination { LoadCases = factoredCases, Name = name, Number = number };
        }
    }
}







