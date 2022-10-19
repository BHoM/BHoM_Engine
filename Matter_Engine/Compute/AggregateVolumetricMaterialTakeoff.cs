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

        [Description("Calculates an aggregate VolumetricMaterialTakeoff from a collection of elements.")]
        [Input("elements", "The elements to iterate over in generation of the VolumetricMaterialTakeoff.")]
        [Input("checkForTakeoffFragment", "If true and the provided element is a BHoMObject, the incoming item is checked if it has a VolumetricMaterialTakeoff fragment attached, and if so, returns it. If false, the VolumetricMaterialTakeoff returned will be calculated, independant of fragment attached.")]
        [Output("volumetricMaterialTakeoff", "A VolumetricMaterialTakeoff containing the unique materials across all elements.")]
        public static VolumetricMaterialTakeoff AggregateVolumetricMaterialTakeoff(IEnumerable<IElementM> elements, bool checkForTakeoffFragment = false)
        {
            return AggregateVolumetricMaterialTakeoff(elements.Select(x => x.IVolumetricMaterialTakeoff(checkForTakeoffFragment)));
        }

        /***************************************************/

        [Description("Calculates an aggregate VolumetricMaterialTakeoff from a collection individual VolumetricMaterialTakeoffs.")]
        [Input("volumetricMaterialTakeoffs", "The individual VolumetricMaterialTakeoffs to aggregate together.")]
        [Output("volumetricMaterialTakeoff", "A VolumetricMaterialTakeoff incorporating the provided materials and volumes from each individual VolumetricMaterialTakeoff.")]
        public static VolumetricMaterialTakeoff AggregateVolumetricMaterialTakeoff(IEnumerable<VolumetricMaterialTakeoff> volumetricMaterialTakeoffs)
        {
            List<VolumetricMaterialTakeoff> localMatTakeoffs = volumetricMaterialTakeoffs.ToList();

            Dictionary<string, Tuple<Material, double>> hashedMaterialVolumeTuples = new Dictionary<string, Tuple<Material, double>>();

            for (int j = 0; j < localMatTakeoffs.Count; j++)
            {
                for (int i = 0; i < localMatTakeoffs[j].Materials.Count; i++)
                {
                    Material mat = localMatTakeoffs[j].Materials[i];
                    double volume = localMatTakeoffs[j].Volumes[i];
                    string hash = mat.Hash();
                    Tuple<Material, double> matVolumePair;
                    if (hashedMaterialVolumeTuples.TryGetValue(hash, out matVolumePair))
                        matVolumePair = new Tuple<Material, double>(matVolumePair.Item1, matVolumePair.Item2 + volume);
                    else
                        matVolumePair = new Tuple<Material, double>(mat, volume);

                    hashedMaterialVolumeTuples[hash] = matVolumePair;
                }
            }


            return new VolumetricMaterialTakeoff(hashedMaterialVolumeTuples.Values.Select(x => x.Item1), hashedMaterialVolumeTuples.Values.Select(x => x.Item2));
        }

        /***************************************************/

    }
}


