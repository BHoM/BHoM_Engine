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

        [Description("Maps a set of materials in the GeneralMaterialTakeoff of the provided elements to a set of provided transdiciplinary materials.\n" +
                     "First atempts to match the name of the provided materials to the transdiciplinary material maps.\n" +
                     "If no name match is found, atempts to instead find a material with as many matching MaterialProperties (based on type and name) as possible.\n" +
                     "If a unique match is found based on one of the above matching methods, all Properties from the transdiciplinary material is applied to the material to be matched.")]
        [Input("element", "The elements to fetch GeneralMaterialTakeoff from.")]
        [Input("templateMaterials", "The template materials to match to and assign properties from onto the model materials. Should generally have unique names. Names of material as well as material properties will be used to map to the materials to be modified.")]
        [Input("checkForTakeoffFragment", "If true and the provided element is a BHoMObject, the incoming item is checked if it has a VolumetricMaterialTakeoff fragment attached, and if so, returns that Material composition corresponding to this fragment. If false, the MaterialComposition returned will be calculated, independant of fragment attached.")]
        [Input("prioritiseMap", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("uniquePerNamespace", "If true, the method is checking for similarity of MaterialProperties on the materials of the element and found matching material map based on namespace. If false, this check is instead done on exact type.")]
        [Output("generalMaterialTakeoff", "The GeneralMaterialTakeoff for each element with materials mapped to the provided transdiciplinary materials.")]
        public static GeneralMaterialTakeoff MappedGeneralMaterialTakeoff(this IElementM element, IEnumerable<Material> templateMaterials, bool checkForTakeoffFragment = true, bool prioritiseMap = true, bool uniquePerNamespace = true)
        {
            if (element == null)
                return null;

            return element.IGeneralMaterialTakeoff(checkForTakeoffFragment).AssignTemplate(templateMaterials, prioritiseMap, uniquePerNamespace);
        }

        /***************************************************/
    }
}



