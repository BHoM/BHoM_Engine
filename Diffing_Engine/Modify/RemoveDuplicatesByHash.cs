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

using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Base;

namespace BH.Engine.Diffing
{
    public static partial class Modify
    {
        [Description("Removes duplicates from a collection of objects. The comparison is made using their Hash. If hash is missing, it is computed.")]
        [Input("objects", "Collection of objects whose duplicates have to be removed. If they don't already have an Hash assigned, it will be calculated.")]
        [Input("comparisonConfig", "Settings to determine the uniqueness of an Object.")]
        [Input("useExistingHash", "If true, if objects already have a HashFragment, use that. If false, recompute the hash for all objects.")]
        public static IEnumerable<T> RemoveDuplicatesByHash<T>(this IEnumerable<T> objects, ComparisonConfig comparisonConfig = null, bool useExistingHash = true) where T : IBHoMObject
        {
            return objects?.GroupBy(obj =>
                 obj.Hash(comparisonConfig, useExistingHash)
            ).Select(gr => gr.First());
        }
    }
}






