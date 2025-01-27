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

using BH.oM.Data.Requests;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Finds all instances of IRequests of a given type in a tree created by a set of nested ILogicalRequests.")]
        [Input("request", "A tree created by a set of nested ILogicalRequests to be queried for instances of a given type.")]
        [Input("requestType", "Type of IRequest to be sought for in the tree.")]
        [Output("instances", "All instances of IRequests of a given type in a tree created by a set of nested ILogicalRequests.")]
        public static List<IRequest> AllRequestsOfType(this IRequest request, Type requestType)
        {
            if (!typeof(IRequest).IsAssignableFrom(requestType))
            {
                BH.Engine.Base.Compute.RecordError($"Type {requestType} does not implement {nameof(IRequest)} interface.");
                return null;
            }

            List<IRequest> instances = new List<IRequest>();
            request.RequestsOfType(requestType, instances);
            return instances;
        }


        /***************************************************/
        /****              Private methods              ****/
        /***************************************************/

        private static void RequestsOfType(this IRequest request, Type requestType, List<IRequest> instances)
        {
            Type type = request?.GetType();
            if (type == requestType)
                instances.Add(request);

            if (request is ILogicalRequest)
            {
                foreach (IRequest subRequest in ((ILogicalRequest)request).IRequests())
                {
                    subRequest.RequestsOfType(requestType, instances);
                }
            }
        }

        /***************************************************/
    }
}






