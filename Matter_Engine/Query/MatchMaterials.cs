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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        [Description("Maps a set of model materials to a set of template materials by name and returns the found tempaltes, i.e. finds a material in the list of template materials that has the same name as the material in the model material and returns it. Returns null if nothing is found. The list order of the returned materials correspond to the list order of the input materials.")]
        [Input("modelMaterials", "The materials to find a matching template to.")]
        [Input("tempalteMaterials", "The template materials to scan.")]
        [Output("matchedMaterials", "The a list of template materials matched to the model materials by name. Order corresponds to the model materials. Method returns null for cases where no match is found.")]
        public static List<Material> MatchMaterials(this List<Material> modelMaterials, List<Material> templateMaterials)
        {
            if (modelMaterials.IsNullOrEmpty())
                return null;

            if (templateMaterials == null || !templateMaterials.Any())
            {
                Base.Compute.RecordWarning($"No {nameof(templateMaterials)} provied. List of nulls returned.");
                return Enumerable.Repeat<Material>(null, modelMaterials.Count).ToList();
            }

            ILookup<string, Material> nameLookup = templateMaterials.ToLookup(x => x.Name);

            Dictionary<Tuple<Type, string>, List<Material>> propertyMaps = new Dictionary<Tuple<Type, string>, List<Material>>();

            foreach (Material mat in templateMaterials)
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
            return modelMaterials.Select(x => x.MatchMaterial(nameLookup, propertyMaps)).ToList();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Method preforming the mapping work. Matches the Material first by name, secondly by name of properties using the provided lookup and dictionary.")]
        private static Material MatchMaterial(this Material material, ILookup<string, Material> nameLookup, Dictionary<Tuple<Type, string>, List<Material>> propertyMaps)
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
                return matches[0];
            }
            else if (matches.Count == 0)    //No matches, record warning
            {
                Base.Compute.RecordWarning($"No map found for material named {material.Name}");
                return null;
            }
            else  //More than one match. Record warning
            {
                Base.Compute.RecordWarning($"Material named {material.Name} has ambiguous matches to {string.Join(", ", matches.Select(x => x.Name))}. No mapping preformed.\nEnsure that the transdiciplinaryMaterialMaps have unique names and that your Material has a name matching the transdiciplinary material you want to match.");
                return null;
            }
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
