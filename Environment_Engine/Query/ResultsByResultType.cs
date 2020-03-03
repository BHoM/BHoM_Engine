/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Environment.Results;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Simulation Results by Profile Result Type")]
        [Input("results", "A collection of Simulation Results")]
        [Input("resultType", "The Profile Result Type filter")]
        [Output("simulationResults", "A collection of filtered simulation results")]
        public static List<SimulationResult> ResultsByResultType(this List<SimulationResult> results, ProfileResultType resultType)
        {
            List<SimulationResult> resultList = new List<SimulationResult>();

            foreach (SimulationResult sr in results)
            {
                List<ProfileResult> pResults = sr.SimulationResults.Where(x => x.Type == resultType).ToList();
                if (pResults.Count > 0)
                    resultList.Add(Create.SimulationResult(sr.Name, sr.SimulationResultType, pResults));
            }

            return resultList;
        }
    }
}

