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
using BH.Engine.Base;

namespace BH.Engine.Diffing
{
    public static partial class Modify
    {
        [Description("Clones the IBHoMObjects, computes their hash and stores it in a RevisionFragment. " +
            "If the object already has a RevisionFragment, it computes the current one and keeps the old one in the `previousHash` of the RevisionFragment.")]
        public static List<T> SetRevisionFragment<T>(this IEnumerable<T> objs, DiffingConfig diffingConfig = null) where T : IBHoMObject
        {
            if (!objs?.Any() ?? true)
                return null;

            // Clone the current objects to preserve immutability
            List<T> objs_cloned = new List<T>();

            // Set configurations if DiffingConfig is null
            diffingConfig = diffingConfig == null ? new DiffingConfig() : diffingConfig;

            // Calculate and set the object hashes
            foreach (var obj in objs)
            {
                objs_cloned.Add(SetRevisionFragment(obj, diffingConfig));
            }

            return objs_cloned;
        }

        [Description("Clones the IBHoMObject, computes their hash and stores it in a RevisionFragment. " +
            "If the object already has a RevisionFragment, it computes the current one and keeps the old one in the `previousHash` of the RevisionFragment.")]
        public static T SetRevisionFragment<T>(this T obj, DiffingConfig diffingConfig = null) where T : IBHoMObject
        {
            if (obj == null)
                return default(T);

            // Clone the current object to preserve immutability
            T obj_cloned = BH.Engine.Base.Query.DeepClone(obj);

            // Set configurations if DiffingConfig is null
            diffingConfig = diffingConfig == null ? new DiffingConfig() : diffingConfig;

            // Calculate and set the object hashes
            string hash = obj_cloned.Hash(diffingConfig.ComparisonConfig);

            RevisionFragment existingFragm = obj_cloned.RevisionFragment();

            obj_cloned.Fragments.AddOrReplace(new RevisionFragment(hash, existingFragm?.Hash));

            return obj_cloned;
        }

        [Description("Clones the IBHoMObject, computes their hash and stores it in a RevisionFragment. " +
            "If the object already has a RevisionFragment, it computes the current one and keeps the old one in the `previousHash` of the RevisionFragment.")]
        public static T SetRevisionFragment<T>(this T obj, string hash) where T : IBHoMObject
        {
            if (obj == null)
                return default(T);

            // Clone the current object to preserve immutability
            T obj_cloned = BH.Engine.Base.Query.DeepClone(obj);

            RevisionFragment existingFragm = obj_cloned.RevisionFragment();

            obj_cloned.Fragments.AddOrReplace(new RevisionFragment(hash, existingFragm?.Hash));

            return obj_cloned;
        }
    }
}




