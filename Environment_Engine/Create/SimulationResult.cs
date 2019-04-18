/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Results;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Create.SimulationResult => Returns an Environment Simulation Result object")]
        [Input("name", "The name of the simulation result, default empty string")]
        [Input("type", "The type of simulation result from the Simulation Result Type enum, default undefined")]
        [Input("results", "A collection of profile results that make up this simulation result, default null")]
        [Output("An Environment Simulation Result object")]
        public static SimulationResult SimulationResult(string name = "", SimulationResultType type = SimulationResultType.Undefined, List<ProfileResult> results = null)
        {
            results = results ?? new List<ProfileResult>();
            return new SimulationResult
            {
                Name = name,
                SimulationResultType = type,
                SimulationResults = results,
            };
        }
    }
}
