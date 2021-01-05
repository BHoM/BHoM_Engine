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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Structure.Requests;
using BH.oM.Structure.Loads;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a request for extracting Bar results from an adapter.")]
        [InputFromProperty("resultType")]
        [InputFromProperty("axis")]
        [InputFromProperty("cases")]
        [InputFromProperty("modes")]
        [InputFromProperty("objectIds")]
        [Output("request", "The created NodeResultRequest.")]
        public static NodeResultRequest NodeResultRequest(NodeResultType resultType = NodeResultType.NodeReaction, LoadAxis axis = LoadAxis.Global, List<object> cases = null, List<string> modes = null, List<object> objectIds = null)
        {
            return new NodeResultRequest
            {
                ResultType = resultType,
                Axis = axis,
                Cases = cases ?? new List<object>(),
                Modes = modes ?? new List<string>(),
                ObjectIds = objectIds ?? new List<object>()
            };
        }

        /***************************************************/
    }
}

