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
using System.ComponentModel;
using System.Linq;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Fragments;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Results;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using BH.Engine.Spatial;


namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Calculates the ReinforcementDensity of a BarRequiredArea from a given Bar and Material. \n " +
            "The input output relation is one to one, so it is recommended that some enveloping methods are used prior to this method.")]
        [Input("barRequiredAreas", "The BarRequiredArea containing the reinforced areas and ids for the Bar and Material.")]
        [Input("bars", "The Bars to search for the relevant Bar associated with the BarRequiredArea.")]
        [Input("materials", "The Materials to search for the relevant Material associated with the BarRequiredArea.")]
        [Output("reinforcementDensity", "The ReinforcementDensity calculated using the inputs provided.")]
        public static List<ReinforcementDensity> ReinforcementDensity(List<BarRequiredArea> barRequiredAreas, List<Bar> bars, List<IMaterialFragment> materials, Type adapterIdType = null)
        {
            if (barRequiredAreas.IsNullOrEmpty() || barRequiredAreas.Any(x => x.IsNull()) || materials.IsNullOrEmpty() || materials.Any(x => x.IsNull()) || bars.IsNullOrEmpty() || bars.Any(x => x.IsNull()))
                return null;

            Dictionary<string, IMaterialFragment> materialsDict = materials.ToDictionary(x => x.Name);

            adapterIdType = bars.First().FindIdentifier(adapterIdType);

            if (adapterIdType == null)
                return null;

            Dictionary<string, Bar> barsDict = bars.ToDictionary(x => ((IAdapterId)x.Fragments[adapterIdType]).Id.ToString());

            IMaterialFragment material;
            Bar resultBar;
            List<ReinforcementDensity> reinforcementDensities = new List<ReinforcementDensity>();

            foreach (BarRequiredArea barRequiredArea in barRequiredAreas)
            {
                if (materialsDict.TryGetValue(barRequiredArea.MaterialName, out material) && barsDict.TryGetValue(barRequiredArea.ObjectId.ToString(), out resultBar))
                {
                    if (resultBar.SectionProperty.IsNull())
                        return null;

                    double elementArea = resultBar.SectionProperty.Area;
                    double reinforcedArea = barRequiredArea.SumRequiredArea();
                    double density = material.Density;
                    double rebarDensity = reinforcedArea * density / elementArea;
                    reinforcementDensities.Add(new ReinforcementDensity() { Density = rebarDensity, Material = material });
                }
                else
                {
                    Base.Compute.RecordError("The Bar and/or Material could not be found in the lists provided corresponding to the ids provided for in the BarRequiredArea.");
                    return null;
                }
            }

            return reinforcementDensities;
        }

        /***************************************************/

        [NotImplemented]
        [Description("Calculates the ReinforcementDensity of a MeshRequiredArea from a given Bar and Material.")]
        [Input("meshRequiredArea", "The BarRequiredArea containing the reinforced areas and ids for the Bar and Material.")]
        [Input("elements", "The elements to search for the relevant AreaElement associated with the MeshRequiredArea.")]
        [Input("materials", "The Materials to search for the relevant Material associated with the MeshRequiredArea.")]
        [Output("reinforcementDensity", "The ReinforcementDensity calculated using the inputs provided.")]
        public static ReinforcementDensity ReinforcementDensity(MeshRequiredArea meshRequiredArea, List<IAreaElement> elements, List<IMaterialFragment> materials)
        {
            return null;
        }

        /***************************************************/

    }
}



