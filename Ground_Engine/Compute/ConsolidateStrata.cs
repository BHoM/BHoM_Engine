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
using System.Transactions;
using System.Reflection;
using ICSharpCode.Decompiler.CSharp.Syntax;
using System.Globalization;
using System.Data;

namespace BH.Engine.Ground
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("A method that takes a Borehole, and consolidates the Strata sequentially by combining them based on a specific property.")]
        [Input("borehole", "The Borehole to consolidate the strata for.")]
        [Input("propertyCompare", "The property of the Strata to consolidate such as ObservedGeology, InterpretedGeology or Legend.")]
        [Input("decimals", "The number of decimals to display the depth ranges.")]
        [Output("b", "The consolidated Borehole.")]
        public static Borehole ConsolidateStrata(Borehole borehole, string propertyCompare, int decimals)
        {
            if (borehole.IsValid())
                return null;


            Borehole consolidatedBorehole = borehole.ShallowClone();

            List<Stratum> strata = consolidatedBorehole.Strata;
            // Add first strata so the strings are formatted the same as the consolidated borehole
            List<Stratum> consolidatedStrata = new List<Stratum>() { strata[0].RangeProperties(null, propertyCompare, true, decimals) };

            for (int i = 1; i < strata.Count; i++)
            {
                Stratum consolidatedStratum = consolidatedStrata.Last();
                Stratum stratum = strata[i];
                // Check whether the strings are equal based on the propertyCompare
                if (string.Equals(Base.Query.PropertyValue(stratum, propertyCompare), Base.Query.PropertyValue(strata[i - 1], propertyCompare)))
                {
                    // Update ConsolidatedStratum to include next stratum
                    consolidatedStratum = RangeProperties(stratum, consolidatedStratum, propertyCompare, false, decimals);
                    consolidatedStrata[consolidatedStrata.Count - 1] = consolidatedStratum;
                }
                else
                {
                    // Add new line
                    consolidatedStrata.Add(strata[i].RangeProperties(null, propertyCompare, true, decimals));
                }
            }

            consolidatedBorehole.SetPropertyValue("Strata", consolidatedStrata);

            return consolidatedBorehole;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Stratum RangeProperties(this Stratum stratum, Stratum consolidatedStratum, string propName, bool initial, int decimals)
        {
            Stratum updatedStratum = stratum.ShallowClone();
            double top = stratum.Top;
            double bottom = stratum.Bottom;

            // Properties to skip over
            List<string> skipProp = new List<string>() { "Id", "Top", "Bottom", "Properties", "BHoM_Guid", "Name", "Fragments", "Tags", "CustomData" };
            skipProp.Add(propName);

            if (!initial)
            {
                updatedStratum.Top = consolidatedStratum.Top;
                updatedStratum.Bottom = stratum.Bottom;
            }

            string topRounded = top.ToString($"N{decimals}", CultureInfo.InvariantCulture);
            string bottomRounded = bottom.ToString($"N{decimals}", CultureInfo.InvariantCulture);

            foreach (PropertyInfo property in typeof(Stratum).GetProperties())
            {
                if (!skipProp.Any(x => x.Equals(property.Name)))
                {
                    string summary = "";
                    if (initial)
                        summary = $"{topRounded}m  - {bottomRounded}m: {stratum.PropertyValue(property.Name)}";
                    else
                        summary = $"{consolidatedStratum.PropertyValue(property.Name)}\n" +
                             $"{topRounded}m  - {bottomRounded}m: {stratum.PropertyValue(property.Name)}";

                    updatedStratum.SetPropertyValue(property.Name, summary);
                }
            }

            if (!updatedStratum.Properties.IsNullOrEmpty())
            {
                List<IStratumProperty> properties = new List<IStratumProperty>();
                for (int i = 0; i < stratum.Properties.Count; i++)
                {
                    IStratumProperty property = updatedStratum.Properties[i];
                    if (property is StratumReference)
                    {
                        StratumReference consolidatedReference = null;
                        if (!initial)
                            consolidatedReference = consolidatedStratum.Properties.OfType<StratumReference>().First();
                        StratumReference updatedReference = (StratumReference)property.ShallowClone();
                        foreach (PropertyInfo prop in typeof(StratumReference).GetProperties())
                        {
                            if (!skipProp.Any(x => x.Equals(prop.Name)))
                            {
                                string summary = "";
                                if (initial)
                                    summary = $"{topRounded}m  - {bottomRounded}m: {updatedReference.PropertyValue(prop.Name)}";
                                else
                                    summary = $"{consolidatedReference.PropertyValue(prop.Name)}\n" +
                                        $"{topRounded}m  - {bottomRounded}m: {updatedReference.PropertyValue(prop.Name)}";

                                updatedReference.SetPropertyValue(prop.Name, summary);
                            }
                        }
                        properties.Add(updatedReference);
                    }
                }

                updatedStratum.Properties = properties;
            }

            return updatedStratum;
        }

        /***************************************************/

    }
}


