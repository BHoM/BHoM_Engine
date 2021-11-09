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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns whether an object/property Name is included in the ComparisonConfig propertiesToConsider." +
            "\nIf a propertyToConsider was specified only with its `Name` (as opposed to a `FullName`), and a match with the input objectFullName is found, the objectFullName is added to the propertiesToConsider," +
            "so future sub-properties of the input objectFullName will be evaluated correctly." +
            "\nUseful when Diffing or Hashing.")]
        [Input("comparisonConfig", "The comparisonConfig containing the exceptions to scan. This may get updated with additional PropertiesToConsider, see description.")]
        [Input("objectFullName", "The object Full Name (e.g. `BH.oM.Structure.Elements.Bar.StartNode`). The function will check if this is included in the Exceptions.")]
        [Output("bool", "True if the given objectFullName is amongst the PropertiesToConsider, false otherwise.")]
        public static bool ScanPropertiesToConsider(this BaseComparisonConfig comparisonConfig, string objectFullName)
        {
            bool validInclusionFound = false;

            foreach (string ptc in comparisonConfig.PropertiesToConsider)
            {
                if (string.IsNullOrEmpty(ptc))
                    continue;

                validInclusionFound = true;

                // If the specified propertyToConsider contains a dot, we can assume it's a property full name, otherwise it's just a name. E.g. `BH.oM.Structure.Elements.Bar.StartNode` VS `StartNode`
                bool containsDots = ptc.Contains(".");

                if (containsDots && ptc.StartsWith("BH."))
                {
                    // If the propertyToConsider is a FullName, check if the objectFullName starts with the propertyToConsider.
                    // E.g. we need to return true if the objectFullName is `BH.oM.Structure.Elements.Bar.StartNode.Position.X` and the propertyToConsider is `BH.oM.Structure.Elements.Bar.StartNode`.
                    if (objectFullName.StartsWith($"{ptc}"))
                        return true;
                } 
                else if (containsDots)
                {
                    var parts = ptc.Replace("*","").Split('.').Where(x => !string.IsNullOrEmpty(x)).ToList();

                    // If the propertyToConsider is at least a partial FullName, check if the objectFullName starts with the propertyToConsider.
                    // E.g. we need to return true if the objectFullName is `BH.oM.Structure.Elements.Bar.StartNode.Position.X` and the propertyToConsider is `BH.oM.Structure.Elements.Bar.StartNode`.
                    if (objectFullName.Contains($"{ptc}") || objectFullName.EndsWith(parts[0]))
                        return true;
                }

                // If the propertyToConsider is a simple Name, check if the objectFullName ends with the propertyToConsider.
                // E.g. we need to return true if the objectFullName is `BH.oM.Structure.Elements.Bar.StartNode` and the propertyToConsider is `StartNode`.
                if (objectFullName.EndsWith($".{ptc}"))
                {
                    // Before returning, we need to add the objectFullName to the inclusions.
                    // This is needed to allow future sub-properties of the current object to be included.
                    comparisonConfig.PropertiesToConsider.Add(objectFullName);
                    return true;
                }
            }

            // If no valid inclusion found, just assume the objectFullName is to be considered.
            if (!validInclusionFound)
                return true;

            // If the objectFullName was not found among the inclusions, it must be skipped. Return false.
            return false;
        }
    }
}
