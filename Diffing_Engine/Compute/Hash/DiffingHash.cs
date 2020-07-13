/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/ 

        [Description("Computes the hash code required for the Diffing.")]
        [Input("obj", "Objects the hash code should be calculated for")]
        [Input("diffConfig", "Sets configs for the hash calculation, such as properties to be ignored.")]
        public static string DiffingHash(this object obj, DiffConfig diffConfig = null)
        {
            diffConfig = diffConfig == null ? new DiffConfig() : (DiffConfig)diffConfig.GetShallowClone();

            // The following is to consider only the PropertiesToInclude specified in the diffConfig.
            // Since the SHA hash algorithm can only consider "exceptions", we need to retrieve all the top level properties,
            // intersect them with the set of PropertiesToInclude, and treat all the properties that remain out as "exceptions" (not to be considered).
            if (diffConfig.PropertiesToConsider.Any())
            {
                IEnumerable<string> exceptions = BH.Engine.Reflection.Query.PropertyNames(obj).Except(diffConfig.PropertiesToConsider);
                diffConfig.PropertiesToIgnore.AddRange(exceptions);
            }

            // The current Hash must not be considered when computing the hash. Remove HashFragment if present. 
            IBHoMObject bhomobj = obj as IBHoMObject;
            if (bhomobj != null)
            {
                bhomobj = BH.Engine.Base.Query.DeepClone(obj) as IBHoMObject;
                bhomobj.Fragments.Remove(typeof(HashFragment));
                return Compute.SHA256Hash(bhomobj, diffConfig.PropertiesToIgnore);
            }

            return Compute.SHA256Hash(obj, diffConfig.PropertiesToIgnore);
        }
    }
}

