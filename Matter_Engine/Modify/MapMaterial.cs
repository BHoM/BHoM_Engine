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

        [Description("Maps a set of materials to a set of provided transdiciplinary materials.\n" +
                     "First atempts to match the name of the provided materials to the material maps.\n" +
                     "If no name match is found, atempts to instead find a material with as many matching MaterialProperties (based on type and name) as possible.\n" +
                     "If a unique match is found based on one of the above matching methods, all Properties from the transdiciplinary material is applied to the material to be matched.")]
        [Input("materials", "The Materials to Modify, will be evaluated based on their name and properties.")]
        [Input("materialMaps", "The Material maps to match to. Should generally have unique names. Names of material as well as material properties will be used to map to the materials to be modified.")]
        [Output("materials", "Materials with modified list of properties. Materials for which no unique match could be found are unaffected.")]
        public static IEnumerable<Material> MapMaterial(this IEnumerable<Material> materials, IEnumerable<Material> materialMaps)
        {
            ILookup<string, Material> nameLookup = materialMaps.ToLookup(x => x.Name);

            Dictionary<Tuple<Type, string>, List<Material>> propertyMaps = new Dictionary<Tuple<Type, string>, List<Material>>();

            foreach (Material mat in materialMaps)
            {
                foreach (IMaterialProperties property in mat.Properties)
                {
                    Tuple<Type, string> key = new Tuple<Type, string>(property.GetType(), property.Name);
                    if (propertyMaps.ContainsKey(key))
                        propertyMaps[key].Add(mat);
                    else
                        propertyMaps[key] = new List<Material>() { mat };
                }
            }

            List<Material> mappedMaterials = new List<Material>();

            foreach (Material material in materials)
            {
                mappedMaterials.Add(material.MapMaterial(nameLookup, propertyMaps));
            }
            return mappedMaterials;
        }

        /***************************************************/

        [Description("Maps a set of materials in the MaterialCompositions to a set of provided transdiciplinary materials.\n" +
                     "First atempts to match the name of the provided materials to the transdiciplinary material maps.\n" +
                     "If no name match is found, atempts to instead find a material with as many matching MaterialProperties (based on type and name) as possible.\n" +
                     "If a unique match is found based on one of the above matching methods, all Properties from the transdiciplinary material is applied to the material to be matched.")]
        [Input("materialCompositions", "The MaterialCompositions to Modify. Materials int he MaterialCompositions will be evaluated based on the name and properties.")]
        [Input("materialMaps", "The Material maps to match to. Should generally have unique names. Names of material as well as material properties will be used to map to the materials to be modified.")]
        [Output("materialCompositions", "MaterialComposition with Materials with modified list of properties. Materials for which no unique match could be found are unaffected.")]
        public static IEnumerable<MaterialComposition> MapMaterial(this IEnumerable<MaterialComposition> materialCompositions, IEnumerable<Material> materialMaps)
        {
            IEnumerable<Material> allMaterials = materialCompositions.SelectMany(x => x.Materials).GroupBy(x => x.BHoM_Guid).Select(x => x.First());
            Dictionary<Guid, Material> mappedMaterials = allMaterials.MapMaterial(materialMaps).ToDictionary(x => x.BHoM_Guid);
            return materialCompositions.Select(x => new MaterialComposition(x.Materials.Select(mat => mappedMaterials[mat.BHoM_Guid]), x.Ratios)).ToList();
        }

        /***************************************************/

        [Description("Maps a set of materials in the MaterialCompositions to a set of provided transdiciplinary materials.\n" +
                     "First atempts to match the name of the provided materials to the transdiciplinary material maps.\n" +
                     "If no name match is found, atempts to instead find a material with as many matching MaterialProperties (based on type and name) as possible.\n" +
                     "If a unique match is found based on one of the above matching methods, all Properties from the transdiciplinary material is applied to the material to be matched.")]
        [Input("volumetricMaterialTakeoffs", "The VolumetricMaterialTakeoff to Modify. Materials int he VolumetricMaterialTakeoff will be evaluated based on the name and properties.")]
        [Input("materialMaps", "The Material maps to match to. Should generally have unique names. Names of material as well as material properties will be used to map to the materials to be modified.")]
        [Output("volumetricMaterialTakeoffs", "MaterialComposition with Materials with modified list of properties. Materials for which no unique match could be found are unaffected.")]
        public static IEnumerable<VolumetricMaterialTakeoff> MapMaterial(this IEnumerable<VolumetricMaterialTakeoff> materialCompositions, IEnumerable<Material> materialMaps)
        {
            IEnumerable<Material> allMaterials = materialCompositions.SelectMany(x => x.Materials).GroupBy(x => x.BHoM_Guid).Select(x => x.First());
            Dictionary<Guid, Material> mappedMaterials = allMaterials.MapMaterial(materialMaps).ToDictionary(x => x.BHoM_Guid);
            return materialCompositions.Select(x => new VolumetricMaterialTakeoff(x.Materials.Select(mat => mappedMaterials[mat.BHoM_Guid]), x.Volumes)).ToList();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Material MapMaterial(this Material material, ILookup<string, Material> nameLookup, Dictionary<Tuple<Type, string>, List<Material>> propertyMaps)
        {
            List<Material> matches = nameLookup[material.Name].ToList();    //Try match by name

            if (matches.Count == 0) //If no name match found, try match by material proeprties - Type and name
            {
                foreach (IMaterialProperties property in material.Properties)
                {
                    Tuple<Type, string> key = new Tuple<Type, string>(property.GetType(), property.Name);

                    List<Material> propMatches;
                    if (propertyMaps.TryGetValue(key, out propMatches))
                        matches.AddRange(propMatches);
                }

                matches = matches.GroupBy(x => x.Name).Select(x => x.First()).ToList();
                matches = matches.GroupBy(x => x.MatchScore(material))  //Get and group by score
                                 .Where(x => x.Key > 0)                 //Only keep materials with score > 0, as negative score indicates at least one mismatch
                                 .OrderByDescending(x => x.Key)         //Order by score - Higher score first
                                 .FirstOrDefault()?.ToList() ?? new List<Material>();
            }

            if (matches.Count == 1) //Exactly one best match. Success!
            {
                return material.MergeProperties(matches[0]);
            }
            else if (matches.Count == 0)    //No matches, record warning
            {
                Base.Compute.RecordWarning($"No map found for material named {material.Name}");
                return material;
            }
            else  //More than one match. Record warning
            {
                Base.Compute.RecordWarning($"Material named {material.Name} has ambiguous matches to {string.Join(", ", matches.Select(x => x.Name))}. No mapping preformed.\nEnsure that the transdiciplinaryMaterialMaps have unique names and that your Material has a name matching the transdiciplinary material you want to match.");
                return material;
            }
        }

        /***************************************************/

        [Description("Merges the Properties of the target and source by adding all properties on the source to the target. For duplicate types the Property on the Source is prioritised.")]
        private static Material MergeProperties(this Material target, Material source)
        {
            Material targetClone = target.ShallowClone();
            targetClone.Properties = new List<IMaterialProperties>(target.Properties);
            List<Type> targetTypes = target.Properties.Select(x => x.GetType()).ToList();
            foreach (IMaterialProperties property in source.Properties)
            {
                if (targetTypes.Contains(property.GetType()))
                {
                    targetClone.Properties.RemoveAll(x => x.GetType() == property.GetType());
                }
                targetClone.Properties.Add(property);
            }
            return targetClone;
        }

        /***************************************************/

        [Description("Matches two materials based on how well the properties align based on name. For each aligning property the score is increased by 1. If a single misaligned property is found a score of -1 is returned.")]
        private static int MatchScore(this Material mat1, Material mat2)
        {
            //Method matches two materials based on their properties.
            //For each proeprty of the same type and same name, the score is increased by 1.
            //If a property of the same type but a different name is found the score is set to -1, as that means a mismatch of properties.
            int score = 0;
            ILookup<Type, IMaterialProperties> matLookup = mat2.Properties.ToLookup(x => x.GetType());
            foreach (IMaterialProperties prop in mat1.Properties)
            {
                string name = prop.Name;
                Type type = prop.GetType();

                IEnumerable<IMaterialProperties> prop2 = matLookup[type];
                if (prop2.Any())
                {
                    if (prop2.Any(x => x.Name == name))
                        score++;
                    else
                        return -1;
                }

            }
            return score;
        }

        /***************************************************/

    }
}


