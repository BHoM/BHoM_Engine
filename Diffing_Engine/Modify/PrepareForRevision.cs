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

namespace BH.Engine.Diffing
{
    public static partial class Modify
    {
        [Description("Prepare a list of objects to be included in a Revision: sets the revision fragment with the object's hash, and removes any duplicates by hash.")]
        [Input("objects", "Objects to be prepared for revision.")]
        [Input("diffConfig", "(Optional) Additional options for the Diffing that will be used by the Revision. The DiffingConfig.ComparisonConfig will be used to compute the object's hash which will then be stored in the RevisionFragment.")]
        [Output("The distinct (duplicates removed by hash) objects with a RevisionFragment assigned.")]
        public static IEnumerable<T> PrepareForRevision<T>(this IEnumerable<T> objects, DiffingConfig diffConfig = null) where T : IBHoMObject
        {
            if (!objects?.Any() ?? true)
                return null;

            // Clone the current objects to preserve immutability; calculate and set the hash fragment
            IEnumerable<T> objs_cloned = Modify.SetRevisionFragment(objects, diffConfig);

            // Remove duplicates by hash
            objs_cloned = Modify.RemoveDuplicatesByHash(objs_cloned);

            if (objs_cloned.Count() != objects.Count())
                Base.Compute.RecordWarning("Some Objects were duplicates (same hash) and therefore have been discarded.");

            return objs_cloned;
        }
    }
}




