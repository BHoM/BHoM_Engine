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
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Checks whether a tree created by a set of nested ILogicalRequests contains two or more IRequests of given type that can conflict (fall into a logical and statement).")]
        [Input("request", "A tree created by a set of nested ILogicalRequests to be checked for potential conflicts between IRequests of the same type.")]
        [Input("requestType", "Type of IRequest to be checked for potential conflicts.")]
        [Output("overlap", "True if the input tree created by a set of nested ILogicalRequests contains two or more IRequests of given type that can conflict (fall into a logical and statement).")]
        public static bool IsPotentialOverlap(this IRequest request, Type requestType)
        {
            if (!typeof(IRequest).IsAssignableFrom(requestType))
            {
                BH.Engine.Base.Compute.RecordError($"Type {requestType} does not implement {nameof(IRequest)} interface.");
                return false;
            }

            if (typeof(ILogicalRequest).IsAssignableFrom(requestType))
            {
                BH.Engine.Base.Compute.RecordError($"It is not allowed to query for overlaps of types that implement {nameof(ILogicalRequest)} interface.");
                return false;
            }

            return request.IsPotentialOverlap(requestType, 0) == 2;
        }


        /***************************************************/
        /****              Private methods              ****/
        /***************************************************/

        private static int IsPotentialOverlap(this IRequest request, Type requestType, int occurences)
        {
            Type type = request.GetType();
            if (type == requestType)
                return ++occurences;

            if (request is LogicalAndRequest)
            {
                foreach (IRequest subRequest in ((LogicalAndRequest)request).Requests)
                {
                    occurences = subRequest.IsPotentialOverlap(requestType, occurences);
                    if (occurences == 2)
                        return occurences;
                }
            }
            else if (request is LogicalOrRequest)
                return ((LogicalOrRequest)request).Requests.Select(x => x.IsPotentialOverlap(requestType, occurences)).Max();
            else if (request is LogicalNotRequest)
                return ((LogicalNotRequest)request).Request.IsPotentialOverlap(requestType, occurences);

            return occurences;
        }

        /***************************************************/
    }
}






