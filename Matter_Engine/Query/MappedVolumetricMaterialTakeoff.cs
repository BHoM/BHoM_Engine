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
using BH.oM.Dimensional;
using BH.Engine.Base;
using BH.oM.Physical.Materials;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Maps a set of materials in the VolumetricMaterialTakeoff of the provided elements to a set of provided transdiciplinary materials.\n" +
                     "First atempts to match the name of the provided materials to the transdiciplinary material maps.\n" +
                     "If no name match is found, atempts to instead find a material with as many matching MaterialProperties (based on type and name) as possible.\n" +
                     "If a unique match is found based on one of the above matching methods, all Properties from the transdiciplinary material is applied to the material to be matched.")]
        [Input("elements", "The elements to fetch VolumetricMaterialTakeoff from.")]
        [Input("materialMaps", "The Material maps to match to. Should generally have unique names. Names of material as well as material properties will be used to map to the materials to be modified.")]
        [Input("checkForTakeoffFragment", "If true and the provided element is a BHoMObject, the incoming item is checked if it has a VolumetricMaterialTakeoff fragment attached, and if so, returns that Material composition corresponding to this fragment. If false, the MaterialComposition returned will be calculated, independant of fragment attached.")]
        [Output("materialComposition", "The material compositions for each element with materials mapped to the provided transdiciplinary materials.")]
        public static List<VolumetricMaterialTakeoff> MappedVolumetricMaterialTakeoff(this IEnumerable<IElementM> elements, IEnumerable<Material> materialMaps, bool checkForTakeoffFragment = true)
        {
            return elements.Select(x => x.IVolumetricMaterialTakeoff(checkForTakeoffFragment)).MapMaterial(materialMaps).ToList();
        }

        /***************************************************/
    }
}
