/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Data.Requests;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an IRequest that combines a couple of requests into a logical AND statement.")]
        [Input("request1", "First request in logical statement.")]
        [Input("request2", "Second request in logical statement.")]
        [Output("request", "Created request.")]
        public static LogicalAndRequest LogicalAndRequest(IRequest request1, IRequest request2)
        {
            return new LogicalAndRequest { Requests = new List<IRequest> { request1, request2 } };
        }

        /***************************************************/

        [Description("Creates an IRequest that combines a group of requests into a logical AND statement.")]
        [InputFromProperty("requests")]
        [Output("request", "Created request.")]
        public static LogicalAndRequest LogicalAndRequest(List<IRequest> requests)
        {
            return new LogicalAndRequest { Requests = requests };
        }

        /***************************************************/
    }
}



