/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Data.Collections;
using BH.oM.Data.Requests;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /****            Interface methods              ****/
        /***************************************************/

        public static List<IRequest> IRequests(this ILogicalRequest request)
        {
            return Requests(request as dynamic);
        }


        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        public static List<IRequest> Requests(this LogicalAndRequest request)
        {
            if(request == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the requests from a null logical and request.");
                return new List<IRequest>();
            }

            return request.Requests;
        }

        /***************************************************/

        public static List<IRequest> Requests(this LogicalOrRequest request)
        {
            if (request == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the requests from a null logical or request.");
                return new List<IRequest>();
            }

            return request.Requests;
        }

        /***************************************************/

        public static List<IRequest> Requests(this LogicalNotRequest request)
        {
            if (request == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the requests from a null logical not request.");
                return new List<IRequest>();
            }

            List<IRequest> result = new List<IRequest>();
            if (request.Request != null)
                result.Add(request.Request);

            return result;
        }

        /***************************************************/
    }
}





