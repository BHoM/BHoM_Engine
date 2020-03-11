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

        [Description("Creates the combined MaterialComposition for a collection of IElementMs. Two Materials are considered equal if their names are equal.")]
        [Input("elements", "The elements to get the combined materialCombination of.")]
        [Output("materialComposition", "A material composition which contain which materials were used in the elements and in which proportions.")]
        public static MaterialComposition CombinedMaterialComposition(IEnumerable<IElementM> elements)
        {
            return MaterialComposition(elements.Select(x => x.IMaterialComposition()), elements.Select(x => x.ISolidVolume()));
        }

        /***************************************************/

        [Description("Creates a MaterialComposition for a collection of Materials given their ratios.")]
        [Input("materials", "The Materials the MaterialCombination is composed of.")]
        [Input("ratios", "The ratios of each material, the number of ratios must match the number of materials." +
                         "If the ratios sum does not equal 1 they will be factored to do so.", typeof(Ratio))]
        [Output("materialComposition", "A material composition composed of the provided materials and ratios scaled so that their sum equals 1.")]
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

        [Description("Creates a MaterialComposition for a collection of MaterialCompositions given their ratios. Two Materials are considered equal if their names are equal.")]
        [Input("materials", "The MaterialCompositions the MaterialCombination is composed of.")]
        [Input("ratios", "The ratios of each material, the number of ratios must match the number of MaterialCompositions.", typeof(Ratio))]
        [Output("materialComposition", "A material composition composed of the provided MaterialComposition materials and ratios factoring both the inputted ones and the ones in the existing MaterialCompositions.")]
        public static MaterialComposition MaterialComposition(IEnumerable<MaterialComposition> materialCompositions, IEnumerable<double> ratios)
        {
            List<Material> allMaterials = new List<Material>();
            List<double> allRatios = new List<double>();

            List<MaterialComposition> localMatComps = materialCompositions.ToList();
            List<double> localRatios = ratios.ToList();

            if (localMatComps.Count != localRatios.Count)
                return null;

            BHoMObjectNameComparer equalityComparer = new BHoMObjectNameComparer();

            for (int j = 0; j < localMatComps.Count; j++)
            {
                for (int i = 0; i < localMatComps[j].Materials.Count; i++)
                {
                    bool existed = false;
                    for (int k = 0; k < allMaterials.Count; k++)
                    {
                        if (equalityComparer.Equals(allMaterials[k], localMatComps[j].Materials[i]))
                        {
                            allRatios[k] += localMatComps[j].Ratios[i] * localRatios[j];
                            existed = true;
                            break;
                        }
                    }
                    if (!existed)
                    {
                        allMaterials.Add(localMatComps[j].Materials[i]);
                        allRatios.Add(localMatComps[j].Ratios[i] * localRatios[j]);
                    }
                }
            }
            
            double factor = 1 / allRatios.Sum();
            return new MaterialComposition(allMaterials, allRatios.Select(x => x * factor).ToList());
        }

        /***************************************************/

        [Description("Creates a MaterialComposition from a Material, equivellant to casting to MaterialComposition.")]
        [Input("material", "This material will be the only Material in the Composition.")]
        [Output("materialComposition", "A MaterialComposition whos only Material is the provided one with a ratio of one.")]
        public static MaterialComposition MaterialComposition(Material material)
        {
            return (MaterialComposition)material;
        }

        /***************************************************/

    }
}

