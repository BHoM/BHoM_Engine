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

using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Queries a set of objects for items matching given identifiers." +
                     "\nReturns a collection of equal length as the input identifiers, with nulls in case an object with given identifier is not found.")]
        [Input("objects", "Objects to pick from.")]
        [Input("identifiers", "Identifiers to find the matching objects for.")]
        [Output("matching", "Collection of objects matching each of the input identifiers." +
                "\nIf matching object is not found for a given identifier, null item is added to the output collection.")]
        public static List<object> ObjectsByIdentifiers(this IList<object> objects, IEnumerable<IComparable> identifiers)
        {
            List<object> result = new List<object>();
            Dictionary<IComparable, object> cache = new Dictionary<IComparable, object>();
            foreach (IComparable id in identifiers)
            {
                if (cache.ContainsKey(id))
                {
                    result.Add(cache[id]);
                    continue;
                }

                while (cache.Count != objects.Count)
                {
                    object obj = objects[cache.Count];
                    IComparable idToCache = obj.IIdentifier();
                    cache[idToCache] = obj;

                    if (idToCache == id)
                        break;
                }

                if (cache.ContainsKey(id))
                    result.Add(cache[id]);
                else
                    result.Add(null);
            }

            return result;
        }

        /***************************************************/
    }
}

