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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base.Objects;
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
            Material material = new Material
            {
                Name = materialProperty.Name,
                Properties = new List<IMaterialProperties>() { materialProperty },
            };

            return (MaterialComposition)material;
        }

        /***************************************************/

    }
}
