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

using BH.Engine.Base;
using BH.oM.Data.Requests;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Data
{
    public static partial class Modify
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Simplifies a tree created by a set of nested ILogicalRequests by merging LogicalAndRequests embedded into LogicalAndRequests of the same type as well as squashing the ones with only one item.")]
        [Input("request", "A tree created by a set of nested ILogicalRequests to be simplified.")]
        [Output("simplified", "Simplified tree of ILogicalRequests.")]
        public static IRequest SimplifyRequestTree(this IRequest request)
        {
            IRequest clone = request.DeepClone();
            clone.Flatten();
            if (clone is ILogicalRequest)
            {
                List<IRequest> subRequests = ((ILogicalRequest)clone).IRequests();
                if (subRequests.Count == 0)
                    return null;
                else if (subRequests.Count == 1 && !(clone is LogicalNotRequest))
                    return subRequests[0];
            }

            return clone;
        }


        /***************************************************/
        /****              Private methods              ****/
        /***************************************************/

        private static void Flatten(this IRequest request)
        {
            if (request is ILogicalRequest)
            {
                List<IRequest> subRequests = ((ILogicalRequest)request).IRequests();
                Type type = request.GetType();
                bool flattened = false;
                for (int i = subRequests.Count - 1; i >= 0; i--)
                {
                    if (subRequests[i]?.GetType() == type && type != typeof(LogicalNotRequest))
                    {
                        ILogicalRequest toRemove = (ILogicalRequest)subRequests[i];
                        subRequests.RemoveAt(i);
                        subRequests.InsertRange(i, toRemove.IRequests());
                        flattened = true;
                    }
                }

                if (flattened)
                    request.Flatten();
                else
                {
                    for (int i = subRequests.Count - 1; i >= 0; i--)
                    {
                        IRequest subRequest = subRequests[i];
                        subRequest.Flatten();
                        ILogicalRequest logical = subRequest as ILogicalRequest;
                        if (logical != null)
                        {
                            List<IRequest> subSub = logical.IRequests();
                            if (subSub.Count == 0)
                                subRequests.RemoveAt(i);
                            else if (subSub.Count == 1 && !(logical is LogicalNotRequest))
                            {
                                if (request is LogicalNotRequest)
                                    ((LogicalNotRequest)request).Request = subSub[0];
                                else
                                {
                                    subRequests.RemoveAt(i);

                                    if (subSub[0].GetType() == request.GetType())
                                        subRequests.InsertRange(i, ((ILogicalRequest)subSub[0]).IRequests());
                                    else
                                        subRequests.Insert(i, subSub[0]);
                                }
                            }
                        }
                    }
                }
            }
        }

        /***************************************************/
    }
}






