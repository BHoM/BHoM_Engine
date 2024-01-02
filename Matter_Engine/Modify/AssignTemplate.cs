/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

        [Description("Maps a set of template materials to a set of model materials.\n" +
                     "First atempts to match the name of the provided materials to the material maps.\n" +
                     "If no name match is found, atempts to instead find a material with as many matching MaterialProperties (based on type and name) as possible.\n" +
                     "If a unique match is found based on one of the above matching methods, all Properties from the template material is applied to the model material matched.")]
        [Input("modelMaterials", "The Materials to Modify, will be evaluated based on their name and properties.")]
        [Input("templateMaterials", "The template materials to match to and assign properties from onto the model materials. Should generally have unique names. Names of material as well as material properties will be used to map to the materials to be modified.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("uniquePerNamespace", "If true, the method is checking for similarity of MaterialProperties on the materials and found matching material map based on namespace. If false, this check is instead done on exact type.")]
        [Output("materials", "Materials with modified list of properties. Materials for which no unique match could be found are unaffected.")]
        public static IEnumerable<Material> AssignTemplate(this IEnumerable<Material> modelMaterials, IEnumerable<Material> templateMaterials, bool prioritiseTemplate = true, bool uniquePerNamespace = true)
        {
            if (modelMaterials.IsNullOrEmpty())
                return null;

            if (templateMaterials == null || !templateMaterials.Any())
            {
                Base.Compute.RecordWarning($"No {nameof(templateMaterials)} provied. Unmapped {nameof(Material)}s returned.");
                return modelMaterials;
            }

            List<Material> materialList = modelMaterials.ToList();
            List<Material> matchedTemplates = materialList.MatchMaterials(templateMaterials.ToList());

            List<Material> results = new List<Material>();

            for (int i = 0; i < materialList.Count; i++)
            {
                if (matchedTemplates[i] != null)
                    results.Add(materialList[i].CombineMaterials(matchedTemplates[i], prioritiseTemplate, uniquePerNamespace));
                else
                    results.Add(materialList[i]);

            }

            return results;
        }

        /***************************************************/

        [Description("Maps a set of materials in the MaterialCompositions to a set of provided transdiciplinary materials.\n" +
                     "First atempts to match the name of the provided materials to the transdiciplinary material maps.\n" +
                     "If no name match is found, atempts to instead find a material with as many matching MaterialProperties (based on type and name) as possible.\n" +
                     "If a unique match is found based on one of the above matching methods, all Properties from the transdiciplinary material is applied to the material to be matched.")]
        [Input("materialComposition", "The MaterialCompositions to Modify. Materials int he MaterialComposition will be evaluated based on the name and properties.")]
        [Input("templateMaterials", "The template materials to match to and assign properties from onto the model materials. Should generally have unique names. Names of material as well as material properties will be used to map to the materials to be modified.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("uniquePerNamespace", "If true, the method is checking for similarity of MaterialProperties on the materials and found matching material map based on namespace. If false, this check is instead done on exact type.")]
        [Output("materialCompositions", "MaterialComposition with Materials with modified list of properties. Materials for which no unique match could be found are unaffected.")]
        public static MaterialComposition AssignTemplate(this MaterialComposition materialComposition, IEnumerable<Material> templateMaterials, bool prioritiseTemplate = true, bool uniquePerNamespace = true)
        {
            if (materialComposition == null)
                return null;

            if (templateMaterials == null || !templateMaterials.Any())
            {
                Base.Compute.RecordWarning($"No {nameof(templateMaterials)} provied. Unmapped {nameof(MaterialComposition)}s returned.");
                return materialComposition;
            }
            return new MaterialComposition(materialComposition.Materials.AssignTemplate(templateMaterials, prioritiseTemplate, uniquePerNamespace), materialComposition.Ratios);
        }

        /***************************************************/

        [Description("Maps a set of materials in the MaterialCompositions to a set of provided transdiciplinary materials.\n" +
                     "First atempts to match the name of the provided materials to the transdiciplinary material maps.\n" +
                     "If no name match is found, atempts to instead find a material with as many matching MaterialProperties (based on type and name) as possible.\n" +
                     "If a unique match is found based on one of the above matching methods, all Properties from the transdiciplinary material is applied to the material to be matched.")]
        [Input("volumetricMaterialTakeoff", "The VolumetricMaterialTakeoff to Modify. Materials int he VolumetricMaterialTakeoff will be evaluated based on the name and properties.")]
        [Input("templateMaterials", "The template materials to match to and assign properties from onto the model materials. Should generally have unique names. Names of material as well as material properties will be used to map to the materials to be modified.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("uniquePerNamespace", "If true, the method is checking for similarity of MaterialProperties on the materials and found matching material map based on namespace. If false, this check is instead done on exact type.")]
        [Output("volumetricMaterialTakeoffs", "MaterialComposition with Materials with modified list of properties. Materials for which no unique match could be found are unaffected.")]
        public static VolumetricMaterialTakeoff AssignTemplate(this VolumetricMaterialTakeoff volumetricMaterialTakeoff, IEnumerable<Material> templateMaterials, bool prioritiseTemplate = true, bool uniquePerNamespace = true)
        {
            if (volumetricMaterialTakeoff == null)
                return null;

            if (templateMaterials == null || !templateMaterials.Any())
            {
                Base.Compute.RecordWarning($"No {nameof(templateMaterials)} provied. Unmapped {nameof(VolumetricMaterialTakeoff)}s returned.");
                return volumetricMaterialTakeoff;
            }

            return new VolumetricMaterialTakeoff(volumetricMaterialTakeoff.Materials.AssignTemplate(templateMaterials, prioritiseTemplate, uniquePerNamespace), volumetricMaterialTakeoff.Volumes);
        }

        /***************************************************/

        [Description("Maps a set of materials in the MaterialCompositions to a set of provided transdiciplinary materials.\n" +
             "First atempts to match the name of the provided materials to the transdiciplinary material maps.\n" +
             "If no name match is found, atempts to instead find a material with as many matching MaterialProperties (based on type and name) as possible.\n" +
             "If a unique match is found based on one of the above matching methods, all Properties from the transdiciplinary material is applied to the material to be matched.")]
        [Input("generalMaterialTakeoff", "The GeneralMaterialTakeoff to Modify. Materials in the GeneralMaterialTakeoff will be evaluated based on the name and properties.")]
        [Input("templateMaterials", "The template materials to match to and assign properties from onto the model materials. Should generally have unique names. Names of material as well as material properties will be used to map to the materials to be modified.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("uniquePerNamespace", "If true, the method is checking for similarity of MaterialProperties on the materials and found matching material map based on namespace. If false, this check is instead done on exact type.")]
        [Output("generalMaterialTakeoff", "MaterialComposition with Materials with modified list of properties. Materials for which no unique match could be found are unaffected.")]
        public static GeneralMaterialTakeoff AssignTemplate(this GeneralMaterialTakeoff generalMaterialTakeoff, IEnumerable<Material> templateMaterials, bool prioritiseTemplate = true, bool uniquePerNamespace = true)
        {
            if (generalMaterialTakeoff == null)
                return null;

            if (templateMaterials == null || !templateMaterials.Any())
            {
                Base.Compute.RecordWarning($"No {nameof(templateMaterials)} provied. Unmapped {nameof(GeneralMaterialTakeoff)}s returned.");
                return generalMaterialTakeoff;
            }

            List<Material> materials = generalMaterialTakeoff.MaterialTakeoffItems.Select(x => x.Material).AssignTemplate(templateMaterials, prioritiseTemplate, uniquePerNamespace).ToList();

            List<TakeoffItem> takeoffItems = new List<TakeoffItem>();

            for (int i = 0; i < materials.Count; i++)
            { 
                TakeoffItem unMapped = generalMaterialTakeoff.MaterialTakeoffItems[i];
                TakeoffItem item = new TakeoffItem
                {
                    Material = materials[i],
                    Volume = unMapped.Volume,
                    Mass = unMapped.Mass,
                    Area = unMapped.Area,
                    Length = unMapped.Length,
                    ElectricCurrent = unMapped.ElectricCurrent,
                    Energy = unMapped.Energy,
                    NumberItem = unMapped.NumberItem,
                    Power = unMapped.Power,
                    VolumetricFlowRate = unMapped.VolumetricFlowRate,
                };
                takeoffItems.Add(item);
            }


            return new GeneralMaterialTakeoff { MaterialTakeoffItems = takeoffItems };
        }

        /***************************************************/
    }
}




