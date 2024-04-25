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

        [Description("Calculates an aggregate GeneralMaterialTakeoff from a collection of elements.")]
        [Input("elements", "The elements to iterate over in generation of the VolumetricMaterialTakeoff.")]
        [Input("checkForTakeoffFragment", "If true and the provided element is a BHoMObject, the incoming item is checked if it has a VolumetricMaterialTakeoff fragment attached, and if so, returns uses it as the basis for the GeneralMaterialTakeoff. If false, the GeneralMaterialTakeoff returned will be calculated, independant of fragment attached.")]
        [Input("comparisonConfig", "Optional comparison config to be used for check equality of two Materials. Defaults to checking the full Material object.")]
        [Output("generalMaterialTakeoff", "A GeneralMaterialTakeoff containing the unique materials across all elements.")]
        public static GeneralMaterialTakeoff AggregateGeneralMaterialTakeoff(IEnumerable<IElementM> elements, bool checkForTakeoffFragment = true, BaseComparisonConfig comparisonConfig = null)
        {
            if (elements == null)
            {
                Base.Compute.RecordError($"Provided List of {nameof(IElementM)}s is null, cannot compute aggregate takeoff.");
                return null;
            }

            if (elements.Any(x => x == null))
            {
                Base.Compute.RecordWarning($"At least one of the provided {nameof(IElementM)}s is null and will be filtered out when computing the {nameof(AggregateVolumetricMaterialTakeoff)}.");
                elements = elements.Where(x => x != null);
            }

            if (!elements.Any())
            {
                Base.Compute.RecordWarning($"No non-null {nameof(IElementM)}s provided. Empty {nameof(GeneralMaterialTakeoff)} returned.");
                return new GeneralMaterialTakeoff();
            }

            return AggregateGeneralMaterialTakeoff(elements.Select(x => x.IGeneralMaterialTakeoff(checkForTakeoffFragment)), comparisonConfig);
        }

        /***************************************************/

        [Description("Calculates an aggregate GeneralMaterialTakeoff from a collection individual GeneralMaterialTakeoff.")]
        [Input("generalMaterialTakeoffs", "The individual GeneralMaterialTakeoff to aggregate together.")]
        [Input("comparisonConfig", "Optional comparison config to be used for check equality of two Materials. Defaults to checking the full Material object.")]
        [Output("generalMaterialTakeoff", "A GeneralMaterialTakeoff incorporating the provided materials and qunatities from each individual GeneralMaterialTakeoff.")]
        public static GeneralMaterialTakeoff AggregateGeneralMaterialTakeoff(IEnumerable<GeneralMaterialTakeoff> generalMaterialTakeoffs, BaseComparisonConfig comparisonConfig = null)
        {
            if (generalMaterialTakeoffs == null)
            {
                Base.Compute.RecordError($"Provided List of {nameof(GeneralMaterialTakeoff)}s is null, cannot compute aggregate takeoff.");
                return null;
            }

            if (generalMaterialTakeoffs.Any(x => x == null))
            {
                Base.Compute.RecordWarning($"At least one of the provided  {nameof(GeneralMaterialTakeoff)}s is null and will be filtered out when computing the {nameof(AggregateGeneralMaterialTakeoff)}.");
                generalMaterialTakeoffs = generalMaterialTakeoffs.Where(x => x != null);
            }

            if (!generalMaterialTakeoffs.Any())
            {
                Base.Compute.RecordWarning($"No non-null {nameof(GeneralMaterialTakeoff)}s provided. Empty {nameof(GeneralMaterialTakeoff)} returned.");
                return new GeneralMaterialTakeoff();
            }

            List<GeneralMaterialTakeoff> localMatTakeoffs = generalMaterialTakeoffs.ToList();
            Dictionary<string, TakeoffItem> hashedTakeoffItems = new Dictionary<string, TakeoffItem>();

            for (int j = 0; j < localMatTakeoffs.Count; j++)
            {
                for (int i = 0; i < localMatTakeoffs[j].MaterialTakeoffItems.Count; i++)
                {
                    TakeoffItem current = localMatTakeoffs[j].MaterialTakeoffItems[i];
                    string hash = current.Material.Hash(comparisonConfig);
                    TakeoffItem takeoffItem;
                    if (hashedTakeoffItems.TryGetValue(hash, out takeoffItem))
                    {
                        takeoffItem.Mass += current.Mass;
                        takeoffItem.Volume += current.Volume;
                        takeoffItem.Area += current.Area;
                        takeoffItem.Length += current.Length;
                        takeoffItem.NumberItem += current.NumberItem;
                        takeoffItem.ElectricCurrent += current.ElectricCurrent;
                        takeoffItem.Energy += current.Energy;
                        takeoffItem.Power += current.Power;
                        takeoffItem.VolumetricFlowRate += current.VolumetricFlowRate;
                    }
                    else
                        takeoffItem = current.ShallowClone();

                    hashedTakeoffItems[hash] = takeoffItem;
                }
            }


            return new GeneralMaterialTakeoff { MaterialTakeoffItems = hashedTakeoffItems.Values.ToList() };
        }

        /***************************************************/

    }
}




