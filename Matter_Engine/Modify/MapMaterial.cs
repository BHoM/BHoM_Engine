/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
 
using BH.Engine.Base;
using BH.oM.Physical.Materials;

namespace BH.Engine.Matter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Adds a IMaterialFragment to a material based on the mapping defined by the keys and materialFragments. \n" +
                     "i.e. The materialFragment on index 3 will be added to the Material with the same name as the key at index 3.")]
        [Input("materials", "The Materials to Modify, will be evaluated based on their name.")]
        [Input("keys", "The key is the name of the Material to be affected. The keys index in this list relates to the index of a materialFragment to add in the other list. \n" +
                       "Empty keys means that its related materialFragment will be disgarded.")]
        [Input("materialFragments", "The materialFragments to add to the Materials, the order of which relates to the keys.")]
        [Output("materials", "Materials with modified list of properties. Materials whos names did not appear among the keys are unaffected.")]
        public static IEnumerable<Material> MapMaterial(this IEnumerable<Material> materials, List<string> keys, List<IMaterialProperties> materialFragments)
        {
            if (keys.Count > materialFragments.Count)
            {
                Engine.Base.Compute.RecordError("Can't have more keys than materialFragments.");
                return null;
            }

            // Copy the lists
            List<string> culledKeys = new List<string>(keys);
            culledKeys.AddRange(new string[materialFragments.Count - keys.Count]);
            List<IMaterialProperties> culledMaterialFragments = new List<IMaterialProperties>(materialFragments);

            // Cull based on empty keys
            for (int i = culledKeys.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(culledKeys[i]))
                {
                    culledKeys.RemoveAt(i);
                    culledMaterialFragments.RemoveAt(i);
                }
            }

            // Ensure distinct
            if (culledKeys.Count != culledKeys.Distinct().Count())
            {
                Engine.Base.Compute.RecordError("Non-empty keys must be distinct.");
                return null;
            }

            // Add the materialFragment to the material and return
            return materials.Select((x) =>
           {
               for (int i = 0; i < culledKeys.Count; i++)
               {
                   if (x.Name == culledKeys[i])
                   {
                       Material mat = x.DeepClone();
                       mat.Properties.Add(culledMaterialFragments[i]);
                       return mat;
                   }
               }
               return x;
           }).ToList();
        }

        /***************************************************/

    }
}


