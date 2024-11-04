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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks whether the two objects are the same within the scope defined by the ComparisonConfig.")]
        [Input("obj1", "First object to check for equality.")]
        [Input("obj2", "Second object to check for equality.")]
        [Input("comparisonConfig", "Comparison configuration to be used for the comparison.")]
        [Output("equal", "True if the two objects are the same within the scope defined by the provided ComparisonConfig, otherwise false.")]
        public static bool IsEqual(this object obj1, object obj2, BaseComparisonConfig comparisonConfig = null)
        {
            if (obj1 == obj2)
                return true;

            if (obj1 == null || obj2 == null)
                return false;

            if (obj1.GetType() != obj2.GetType())
                return false;

            return !obj1.Differences(obj2, comparisonConfig).Any();
        }

        /***************************************************/
    }
}
