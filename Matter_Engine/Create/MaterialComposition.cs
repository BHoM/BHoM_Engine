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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;
using BH.oM.Base;

namespace BH.Engine.Matter
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a MaterialComposition for a collection of Materials given their ratios.")]
        [Input("materials", "The discrete Materials the MaterialComposition is comprised of.")]
        [Input("ratios", "The ratios of each material based on their relative volumes. The number of ratios must match the number of materials." +
                         "The ratio sum will be normalised as required, to ensure the total equates to one.", typeof(Ratio))]
        [Output("materialComposition", "A material composition composed of the provided materials and ratios scaled so that their sum equals one.")]
        public static MaterialComposition MaterialComposition(IEnumerable<Material> materials, IEnumerable<double> ratios)
        {
            if (materials.IsNullOrEmpty() || ratios.IsNullOrEmpty())
                return null;

            if (materials.Count() != ratios.Count())
            {
                Base.Compute.RecordError("Requires the same number of materials as ratios to create a MaterialComposition.");
                return null;
            }

            if (Math.Abs(1 - ratios.Sum()) > Tolerance.Distance)
            {
                double factor = 1 / ratios.Sum();
                return new MaterialComposition(materials, ratios.Select(x => x * factor));
            }

            return new MaterialComposition(materials, ratios);
        }

        /***************************************************/

        [Description("Creates a homogeneous MaterialComposition from a single Material.")]
        [Input("material", "This material will be the only Material in the Composition.")]
        [Output("materialComposition", "A single Material MaterialComposition with a ratio of one.")]
        public static MaterialComposition MaterialComposition(Material material)
        {
            return (MaterialComposition)material;
        }

        /***************************************************/

        [Description("Creates a homogeneous MaterialComposition from a single IMaterialProperties. Sets the Material's name as the IMaterialProperties' name.")]
        [Input("materialProperty", "This material will be the only Material in the Composition.")]
        [Output("materialComposition", "A single Material MaterialComposition with a ratio of one.")]
        public static MaterialComposition MaterialComposition(IMaterialProperties materialProperty)
        {
            if(materialProperty == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a material composition from a null set of material properties.");
                return null;
            }

            Material material = new Material
            {
                Name = materialProperty.Name,
                Properties = new List<IMaterialProperties>() { materialProperty },
            };

            return (MaterialComposition)material;
        }

        /***************************************************/

        [Description("Creates a MaterialComposition based on the volumes normalised to 1 and materials in the provided Material.")]
        [Input("volumetricMaterialTakeoff", "The VolumetricMaterialTakeoff to be used to create the MaterialComposition. Materials from the VolumetricMaterialTakeoff will be used with corresponing normalised volumes, ensuring the total of all ratios equates to 1.")]
        [Output("materialComposition", "A MaterialComposition composed of the Materials in the provided VolumetricMaterialTakeoff and ratios as its normalised volumes.")]
        public static MaterialComposition MaterialComposition(VolumetricMaterialTakeoff volumetricMaterialTakeoff)
        {
            if (volumetricMaterialTakeoff == null)
            {
                Base.Compute.RecordError($"Cannot create a {nameof(MaterialComposition)} from a null {nameof(VolumetricMaterialTakeoff)}.");
                return null;
            }
            if (volumetricMaterialTakeoff.Volumes == null || volumetricMaterialTakeoff.Materials == null)
            {
                Base.Compute.RecordError($"Cannot create a {nameof(MaterialComposition)} from a {nameof(VolumetricMaterialTakeoff)} with null {nameof(volumetricMaterialTakeoff.Volumes)} or {nameof(volumetricMaterialTakeoff.Materials)}.");
                return null;
            }

            double totalVolume = volumetricMaterialTakeoff.Volumes.Sum();

            return new MaterialComposition(volumetricMaterialTakeoff.Materials, volumetricMaterialTakeoff.Volumes.Select(x => x / totalVolume));
        }

        /***************************************************/

    }
}



