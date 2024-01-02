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

using BH.Engine.Base;
using BH.oM.Data.Requests;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Compute
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Decomposes a tree created by a set of nested ILogicalRequests with multiple items of given type into a set of trees with max. one item of that type each, which in total represents the same request as the original tree.")]
        [Input("request", "A tree of nested ILogicalRequests with multiple items of given type to be split into a set of trees with only one item of that type each.")]
        [Input("splittingType", "Type of IRequest that is not allowed to exist more than once within a tree.")]
        [Output("splitRequests", "Collection of trees with max. one item of the splitting type each, which in total represent the same request as the original tree.")]
        public static List<IRequest> SplitRequestTreeByType(this IRequest request, Type splittingType)
        {
            if (!typeof(IRequest).IsAssignableFrom(splittingType))
            {
                BH.Engine.Base.Compute.RecordError($"Type {splittingType} does not implement {nameof(IRequest)} interface.");
                return null;
            }

            if (typeof(ILogicalRequest).IsAssignableFrom(splittingType))
            {
                BH.Engine.Base.Compute.RecordError($"It is not allowed to split by types that implement {nameof(ILogicalRequest)} interface.");
                return null;
            }

            if (request.IsPotentialOverlap(splittingType))
            {
                BH.Engine.Base.Compute.RecordError($"The request could not be split by type {splittingType} because there is a potential logical AND overlap between two requests of the given type.");
                return null;
            }

            List<IRequest> extracted = new List<IRequest>();
            IRequest simplified = request.SimplifyRequestTree();
            if (simplified is ILogicalRequest)
            {
                simplified.ExtractTrees(splittingType, extracted, new List<IRequest>());
                extracted = extracted.Select(x => x.SimplifyRequestTree()).ToList();

                simplified = simplified.SimplifyRequestTree();
                if (simplified != null)
                    extracted.Add(simplified);
            }
            else
                extracted.Add(simplified);

            return extracted.Where(x => x != null).ToList();
        }


        /***************************************************/
        /****              Private methods              ****/
        /***************************************************/

        private static void ExtractTrees(this IRequest request, Type typeToExtract, List<IRequest> extracted, List<IRequest> history)
        {
            List<IRequest> newHistory = new List<IRequest>(history);

            Type type = request.GetType();
            if (type == typeToExtract)
            {
                extracted.Add(request.Extract(newHistory));

                IRequest last = request;
                for (int i = history.Count - 1; i >= 0; i--)
                {
                    ILogicalRequest current = (ILogicalRequest)history[i];
                    if (current is LogicalNotRequest)
                    {
                        if (i == 0)
                        {
                            ((LogicalNotRequest)current).Request = null;
                            return;
                        }
                        else
                        {
                            last = current;
                            continue;
                        }
                    }
                    else
                    {
                        current.IRequests().Remove(last);
                        return;
                    }
                }
            }

            newHistory.Add(request);

            if (request is LogicalAndRequest)
            {
                List<IRequest> subRequests = ((LogicalAndRequest)request).Requests;
                IRequest found = subRequests.FirstOrDefault(x => x.GetType() == typeToExtract);
                if (found != null)
                {
                    extracted.Add(found.Extract(newHistory));

                    IRequest last = request;
                    for (int i = history.Count - 1; i >= 0; i--)
                    {
                        ILogicalRequest current = (ILogicalRequest)history[i];
                        if (current is LogicalNotRequest)
                        {
                            if (i == 0)
                                ((LogicalNotRequest)current).Request = null;
                            else
                            {
                                last = current;
                                continue;
                            }
                        }
                        else
                        {
                            current.IRequests().Remove(last);
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = subRequests.Count - 1; i >= 0; i--)
                    {
                        subRequests[i].ExtractTrees(typeToExtract, extracted, newHistory);
                    }
                }
            }
            else if (request is LogicalOrRequest)
            {
                List<IRequest> subRequests = ((LogicalOrRequest)request).Requests;
                for (int i = subRequests.Count - 1; i >= 0; i--)
                {
                    subRequests[i].ExtractTrees(typeToExtract, extracted, newHistory);
                }
            }
            else if (request is LogicalNotRequest)
                ((LogicalNotRequest)request).Request.ExtractTrees(typeToExtract, extracted, newHistory);
        }

        /***************************************************/

        private static IRequest Extract(this IRequest request, List<IRequest> history)
        {
            history.Add(request);
            List<int> nexts = new List<int>();
            for (int i = 0; i < history.Count - 1; i++)
            {
                nexts.Add(((ILogicalRequest)history[i]).IRequests().IndexOf(history[i + 1]));
            }

            IRequest result = history[0].DeepClone();
            IRequest current = result;
            foreach (int n in nexts)
            {
                List<IRequest> subRequests = ((ILogicalRequest)current).IRequests();
                IRequest next = subRequests[n];
                if (current is LogicalOrRequest)
                {
                    subRequests.Clear();
                    subRequests.Add(next);
                }

                current = next;
            }

            return result;
        }

        /***************************************************/
    }
}




