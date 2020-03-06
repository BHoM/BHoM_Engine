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

namespace BH.Engine.Matter
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MaterialComposition CombinedMaterialComposition(IEnumerable<IElementM> elements)
        {
            return MaterialComposition(elements.Select(x => x.IMaterialComposition()), elements.Select(x => x.ISolidVolume()));
        }

        /***************************************************/

        public static MaterialComposition MaterialComposition(IEnumerable<Material> materials, IEnumerable<double> ratios)
        {
            if (Math.Abs(1 - ratios.Sum()) > Tolerance.Distance)
            {
                Engine.Reflection.Compute.RecordWarning("The ratios sum did not equal 1, the values have been factored to do so");
                double factor = 1 / ratios.Sum();
                return new MaterialComposition(materials, ratios.Select(x => x * factor));
            }

            return new MaterialComposition(materials, ratios);
        }

        /***************************************************/

        public static MaterialComposition MaterialComposition(IEnumerable<MaterialComposition> materialCompositions, IEnumerable<double> ratios)
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
                    bool existed = false;
                    for (int k = 0; k < allMaterials.Count; k++)
                    {
                        if (allMaterials[k].Equals(localMatComps[j].Materials[i]))
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

        public static MaterialComposition MaterialComposition(Material material)
        {
            return (MaterialComposition)material;
        }

        /***************************************************/

    }
}

