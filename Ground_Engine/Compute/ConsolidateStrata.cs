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
using System.ComponentModel;
using BH.Engine.Base;
using BH.oM.Base.Attributes;
using BH.oM.Ground;

namespace BH.Engine.Ground
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description(".")]
        [Input("borehole", "The Borehole to consolidate the strata for.")]
        [Output("b", "The consolidated borehole.")]
        public static Borehole ConsolidateStrata(Borehole borehole, string propertyCompare)
        {
            if (!borehole.IsValid())
                return null;

            Borehole consolidatedBorehole = borehole;

            List<Stratum> strata = consolidatedBorehole.Strata;
            List<Stratum> consolidatedStrata = new List<Stratum>() { strata[0] };

            for (int i = 1; i < strata.Count; i++)
            {
                Stratum consolidatedStratum = consolidatedStrata.Last();
                Stratum stratum = strata[i];
                if (Base.Query.PropertyValue(strata[i], propertyCompare) == Base.Query.PropertyValue(consolidatedStratum, propertyCompare))
                {
                    consolidatedStratum.Bottom = stratum.Bottom;
                    consolidatedStratum.LogDescription = $"{consolidatedStratum.LogDescription} \n {stratum.Top}m  - {stratum.Bottom}m: {stratum.LogDescription}";
                    consolidatedStratum.Legend = $"{consolidatedStratum.Legend} \n {stratum.Top}m  - {stratum.Bottom}m: {stratum.Legend}";
                    consolidatedStratum.ObservedGeology = $"{consolidatedStratum.ObservedGeology} \n {stratum.Top}m  - {stratum.Bottom}m: {stratum.ObservedGeology}";
                    consolidatedStratum.InterpretedGeology = $"{consolidatedStratum.InterpretedGeology} \n {stratum.Top}m  - {stratum.Bottom}m: {stratum.InterpretedGeology}";

                    if(!stratum.Properties.IsNullOrEmpty())
                    {
                        List<IStratumProperty> properties = new List<IStratumProperty>();
                        for(int j = 0; i < stratum.Properties.Count; j++)
                        {
                            IStratumProperty property = stratum.Properties[j];
                            if(property is StratumReference)
                            {
                                StratumReference consolidatedReference = consolidatedStratum.Properties.OfType<StratumReference>().ToList()[0];
                                StratumReference reference = (StratumReference)property;
                                consolidatedReference.Remarks = $"{consolidatedReference.Remarks} \n {stratum.Top}m  - {stratum.Bottom}m: {reference.Remarks}";
                                consolidatedReference.LexiconCode = $"{consolidatedReference.LexiconCode} \n {stratum.Top}m  - {stratum.Bottom}m: {reference.LexiconCode}";
                                properties.Add(consolidatedReference);
                            }
                        }

                        consolidatedStratum.Properties = properties;
                    }
                }
                else
                {
                    consolidatedStrata.Add(strata[i]);
                }
            }

            consolidatedBorehole.SetPropertyValue("Strata", consolidatedStrata);

            return consolidatedBorehole;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        /***************************************************/

    }
}


