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

        [Description("Returns whether a property Name is included in the ComparisonConfig exceptions. Useful when Diffing or Hashing.")]
        [Input("comparisonConfig", "The comparisonConfig containing the exceptions to scan.")]
        [Input("objectFullName", "The object Full Name (e.g. `BH.oM.Structure.Elements.Bar.StartNode`). The function will check if this is included in the Exceptions.")]
        public static bool IsInPropertyExceptions(this BaseComparisonConfig comparisonConfig, string objectFullName)
        {
            if (comparisonConfig.PropertyExceptions?.Any() ?? false)
            {
                foreach (string pe in comparisonConfig.PropertyExceptions)
                {
                    if (string.IsNullOrEmpty(pe))
                        continue;

                    // If the specified exception contains a dot, we can assume it's a property full name, otherwise it's just a name. E.g. `BH.oM.Structure.Elements.Bar.StartNode` VS `StartNode`
                    bool isExceptionFullName = pe.Contains(".");

                    if (isExceptionFullName)
                    {
                        // If the exception is a FullName, check if the objectFullName starts with the exception.
                        // E.g. we need to return true if the objectFullName is `BH.oM.Structure.Elements.Bar.StartNode.Position.X` and the exception is `BH.oM.Structure.Elements.Bar.StartNode`.
                        if (objectFullName.StartsWith($"{pe}"))
                            return true;
                    }

                    // If the exception is a simple Name, check if the objectFullName ends with the exception.
                    // E.g. we need to return true if the objectFullName is `BH.oM.Structure.Elements.Bar.StartNode` and the exception is `StartNode`.
                    if (objectFullName.EndsWith($".{pe}"))
                        return true;
                }
            }

            return false;
        }
    }
}
