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
using BH.Engine.Base.Objects;
using BH.oM.Base;
using BH.Engine.Diffing;
using BH.Engine.Base;
using BH.oM.Diffing;

namespace BH.Engine.Matter
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates an aggregate MaterialComposition from a collection of elements.")]
        [Input("elements", "The elements to iterate over in generation of the MaterialCombination.")]
        [Output("materialComposition", "A MaterialComposition containing the unique materials across all elements and their relative proportions.")]
        public static MaterialComposition AggregateMaterialComposition(IEnumerable<IElementM> elements)
        {
            return AggregateMaterialComposition(elements.Select(x => x.IMaterialComposition()), elements.Select(x => x.ISolidVolume()));
        }

        /***************************************************/

        [Description("Calculates an aggregate MaterialComposition from a collection individual MaterialCompositions and their relative ratios.")]
        [Input("materialCompositions", "The individual MaterialCompositions to aggregate together.")]
        [Input("ratios", "The relative volumetric based ratios of each MaterialComposition. The number of ratios must match the number of MaterialCompositions.", typeof(Ratio))]
        [Output("materialComposition", "A MaterialComposition incorporating the provided materials from each individual MaterialComposition and newly calculated ratios, factoring both the input ratio values and the individual Material ratios in the existing MaterialCompositions.")]
        public static MaterialComposition AggregateMaterialComposition(IEnumerable<MaterialComposition> materialCompositions, IEnumerable<double> ratios)
        {
            List<Material> allMaterials = new List<Material>();
            List<double> allRatios = new List<double>();

            List<MaterialComposition> localMatComps = materialCompositions.ToList();
            List<double> localRatios = ratios.ToList();

            if (localMatComps.Count != localRatios.Count)
                return null;

            for (int j = 0; j < localMatComps.Count; j++)
            {
                for (int i = 0; i < localMatComps[j].Materials.Count; i++)
                {
                    Material mat = localMatComps[j].Materials[i];
                    mat = BH.Engine.Diffing.Modify.SetRevisionFragment(mat);

                    bool existed = false;
                    for (int k = 0; k < allMaterials.Count; k++)
                    {
                        if (allMaterials[k].FindFragment<RevisionFragment>().Hash == mat.FindFragment<RevisionFragment>().Hash)
                        {
                            allRatios[k] += localMatComps[j].Ratios[i] * localRatios[j];
                            existed = true;
                            break;
                        }
                    }
                    if (!existed)
                    {
                        allMaterials.Add(mat);
                        allRatios.Add(localMatComps[j].Ratios[i] * localRatios[j]);
                    }
                }
            }

            foreach (Material mat in allMaterials)
            {
                mat.RemoveFragment(typeof(RevisionFragment));
            }

            double factor = 1 / allRatios.Sum();
            return new MaterialComposition(allMaterials, allRatios.Select(x => x * factor).ToList());
        }

        /***************************************************/

    }
}



