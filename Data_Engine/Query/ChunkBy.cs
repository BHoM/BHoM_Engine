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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Partition an enumerable collection into sublists based on the number of target sub-lists")]
        [Input("collection", "An enumerable list of variable data types")]
        [Input("nChunks", "The number of \"chunks\" into which the collection should be split")]
        [Output("chunks", "The chunked list")]
        public static IEnumerable<List<T>> ChunkByNumber<T>(IEnumerable<T> collection, int nChunks)
        {
            int i = 0;
            var splits = from item in collection
                         group item by i++ % nChunks into part
                         select part.AsEnumerable().ToList();
            return splits;
        }

        [Description("Chunk an enumerable collection into sublists based on a maximum size of each sub-list")]
        [Input("collection", "An enumerable list of variable data types")]
        [Input("chunkSize", "The size of each \"chunk\" into which the collection should be split")]
        [Output("chunks", "The chunked list")]
        public static IEnumerable<List<T>> ChunkBySize<T>(List<T> collection, int chunkSize = 1)
        {
            for (int i = 0; i < collection.Count; i += chunkSize)
            {
                yield return collection.GetRange(i, Math.Min(chunkSize, collection.Count - i));
            }
        }

        /***************************************************/
    }
}




