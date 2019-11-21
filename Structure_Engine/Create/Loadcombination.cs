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

using BH.oM.Structure.Loads;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LoadCombination LoadCombination(string name, int number, IEnumerable<Tuple<double, ICase>> factoredCases)
        {
            return new LoadCombination { LoadCases = factoredCases.ToList(), Name = name, Number = number };
        }

        /***************************************************/

        public static LoadCombination LoadCombination(string name, int number, List<Loadcase> cases, List<double> factors, bool excludeZeroFactorCases = true)
        {
            if (cases.Count != factors.Count)
                throw new ArgumentException("Loadcombinations require the same number of cases and factors");

            List<Tuple<double, ICase>> factoredCases = new List<Tuple<double, ICase>>();

            for (int i = 0; i < cases.Count; i++)
            {
                if (cases[i] != null && (!excludeZeroFactorCases || factors[i] > 0))
                    factoredCases.Add(new Tuple<double, ICase>(factors[i], cases[i]));
            }

            return LoadCombination(name, number, factoredCases);
        }

        /***************************************************/
    }
}
