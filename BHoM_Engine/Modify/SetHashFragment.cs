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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.oM.Base;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        [Description("Computes the hash of the input BHoMObjects and stores it in a HashFragment for each of them." +
            "\nIf the hashFragment already existed, it is replaced.")]
        public static List<T> SetHashFragment<T>(this List<T> objs, ComparisonConfig comparisonConfig = null) where T : IBHoMObject
        {
            // Each object will be cloned to avoid modification by reference.
            List<T> objs_cloned = new List<T>();

            // Calculate and set the object hashes
            foreach (var obj in objs)
                objs_cloned.Add(SetHashFragment(obj, comparisonConfig));

            return objs_cloned;
        }

        [Description("Computes the hash of the BHoMObject and stores it in a HashFragment." +
            "\nIf the hashFragment already existed, it is replaced.")]
        public static T SetHashFragment<T>(this T obj, ComparisonConfig comparisonConfig = null) where T : IBHoMObject
        {
            // Calculate and set the object hashes
            string hash = obj.Hash(comparisonConfig);
            
            return SetHashFragment<T>(obj, hash);
        }

        [Description("Clones the IBHoMObject, computes its hash and stores it in a HashFragment." +
            "\nIf the hashFragment already existed, it is replaced.")]
        public static T SetHashFragment<T>(this T obj, string hash) where T : IBHoMObject
        {
            if (obj == null)
                return default(T);

            // Clone the current object to avoid modification by reference.
            T obj_cloned = BH.Engine.Base.Query.DeepClone(obj);

            obj_cloned.Fragments.AddOrReplace(new HashFragment() { Hash = hash });

            return obj_cloned;
        }
    }
}






