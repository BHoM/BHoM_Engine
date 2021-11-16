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

using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        [Description("Extract the smallest (most precise) numeric tolerance from the ComparisonConfig," +
            "which is the smallest value amongst all CustomTolerances (irrespective of the properties they were paired with) and the global numeric tolerance.")]
        [Input("comparisonConfig", "Comparison Config from where tolerance information should be extracted.")]
        public static double SmallestToleranceFromConfig(this BaseComparisonConfig comparisonConfig)
        {
            return ToleranceFromConfig(comparisonConfig, "", true); // the `propertyFullName` input is ignored when `getGlobalSmallest` is set to true.
        }
    }
}