/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.Engine.Base;
using BH.Engine.Spatial;


namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Calculates the ReinforcementDensity of a BarRequiredArea from a given Bar and Material.")]
        [Input("barRequiredArea", "The BarRequiredArea containing the reinforced areas and ids for the Bar and Material.")]
        [Input("bars", "The Bars to search for the relevant Bar associated with the BarRequiredArea.")]
        [Input("materials", "The Materials to search for the relevant Material associated with the BarRequiredArea.")]
        [Output("reinforcementDensity", "The ReinforcementDensity calculated using the inputs provided.")]
        public static ReinforcementDensity ReinforcementDensity(BarRequiredArea barRequiredArea, List<Bar> bars, List<IMaterialFragment> materials)
        {
            //Add to IsNull when PR is merged
            if(barRequiredArea == null)
            {
                Reflection.Compute.RecordError("The BarRequiredArea is null and therefore the ReinforcementDensity cannot be calculated.");
                return null;
            }
            else if(materials == null || materials.All(x => x == null))
            {
                Reflection.Compute.RecordError("The Materials are null, these are required to calculate the density of the reinforcing objects.");
                return null;
            }
            else if (bars == null || bars.All(x => x == null))
            {
                Reflection.Compute.RecordError("The Materials are null, these are required to calculate the density of the reinforcing objects.");
                return null;
            }

            Dictionary<string, IMaterialFragment> materialsDict = materials.ToDictionary(x => x.Name.ToString());

            //Work around until Results_Engine PR is merged which have similar methods
            Type fragmentType = bars.FirstOrDefault().Fragments.FirstOrDefault(fr => fr is IAdapterId)?.GetType();
            List<string> ids = new List<string>();

            foreach(Bar bar in bars)
            {
                IFragment id;
                bar.Fragments.TryGetValue(fragmentType, out id);
                ids.Add((((IAdapterId)id).Id.ToString()));
            }

            if(ids.Count == 0)
            {
                Reflection.Compute.RecordError("No ids were found within the Bars, therefore the ReinforcementDensity cannot be evaluated.");
                return null;
            }

            Dictionary<string, Bar> barsDict = ids.Zip(bars, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            IMaterialFragment material;
            Bar resultBar;
            ReinforcementDensity reinforcementDensity;

            if(materialsDict.TryGetValue(barRequiredArea.MaterialName, out material) && barsDict.TryGetValue(barRequiredArea.ObjectId.ToString(), out resultBar))
            {
                //Calculate the volume of the Bar
                if(resultBar.SectionProperty == null)
                {
                    Reflection.Compute.RecordError("The Bar defined in the BarRequiredArea does not have a SectionProperty. Therefore, the ReinforcementDensity cannot be evaluated.");
                    return null;
                }
                double elementArea = resultBar.SectionProperty.Area;
                double reinforcedArea = barRequiredArea.SumRequiredArea();
                double density = material.Density;
                double rebarDensity = reinforcedArea * density / elementArea;
                reinforcementDensity = Create.ReinforcementDensity(rebarDensity, material);
            }
            else
            {
                Reflection.Compute.RecordError("The Bar and/or Material could not be found in the lists provided corresponding to the ids provided for in the BarRequiredArea.");
                return null;
            }

            return reinforcementDensity;
        }

        /***************************************************/

    }
}

