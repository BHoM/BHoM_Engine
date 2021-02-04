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

using BH.Engine.Base;
using BH.oM.Data.Collections;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        public static bool IsPotentialOverlap(this IRequest request, Type requestType)
        {
            if (!typeof(IRequest).IsAssignableFrom(requestType))
            {
                BH.Engine.Reflection.Compute.RecordError($"Type {requestType} does not implement {nameof(IRequest)} interface.");
                return false;
            }

            if (typeof(ILogicalRequest).IsAssignableFrom(requestType))
            {
                BH.Engine.Reflection.Compute.RecordError($"It is not allowed to query for overlaps of types that implement {nameof(ILogicalRequest)} interface.");
                return false;
            }

            return request.IsPotentialOverlap(requestType, RequestTypeExistence.ExistsNot) == RequestTypeExistence.Overlaps;
        }


        /***************************************************/
        /****              Private methods              ****/
        /***************************************************/

        private static RequestTypeExistence IsPotentialOverlap(this IRequest request, Type requestType, RequestTypeExistence rte)
        {
            Type type = request.GetType();
            if (type == requestType)
                return ++rte;

            if (request is LogicalAndRequest)
            {
                foreach (IRequest subRequest in ((LogicalAndRequest)request).Requests)
                {
                    rte = subRequest.IsPotentialOverlap(requestType, rte);
                    if (rte == RequestTypeExistence.Overlaps)
                        return rte;
                }
            }
            else if (request is LogicalOrRequest)
                return ((LogicalOrRequest)request).Requests.Select(x => x.IsPotentialOverlap(requestType, rte)).Max();
            else if (request is LogicalNotRequest)
                return ((LogicalNotRequest)request).Request.IsPotentialOverlap(requestType, rte);

            return rte;
        }


        /***************************************************/
        /****               Private enums               ****/
        /***************************************************/

        private enum RequestTypeExistence : byte
        {
            ExistsNot,
            Exists,
            Overlaps
        }

        /***************************************************/
    }
}


