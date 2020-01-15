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

        [Description("Returns an Environment Profile Result object")]
        [Input("name", "The name of the profile result, default empty string")]
        [Input("type", "The type of profile result from the Profile Result Type enum, default undefined")]
        [Input("unit", "The unit measurement of the profile result from the Profile Result Unit enum, default undefined")]
        [Input("results", "A collection of the results of this profile, default null")]
        [Output("profileResult", "An Environment Profile Result object that can be added to a Simulation Result")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static ProfileResult ProfileResult(string name = "", ProfileResultType type = ProfileResultType.Undefined, ProfileResultUnit unit = ProfileResultUnit.Undefined, List<double> results = null)
        {
            results = results ?? new List<double>();
            return new ProfileResult
            {
                Name = name,
                Unit = unit,
                Type = type,
                Results = results,
            };
        }
    }
}
