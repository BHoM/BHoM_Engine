/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Physical.Constructions;

using BH.oM.Diffing;
using BH.oM.Base;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        [Description("Returns a collection of unique constructions from a list of construction objects")]
        [Input("constructions", "A collection of Constructions")]
        [Input("includeConstructionName", "Flag to determine whether or not to use the construction name as a parameter of uniqueness. Default false")]
        [Output("uniqueConstructions", "A collection of unique Construction objects")]
        public static List<Construction> UniqueConstructions(this List<Construction> constructions, bool includeConstructionName = false)
        {
            ComparisonConfig cc = new ComparisonConfig()
            {
                PropertyExceptions = new List<string>
                {
                    "CustomData"
                },
                NumericTolerance = BH.oM.Geometry.Tolerance.Distance
            };

            if (!includeConstructionName)
                cc.PropertyExceptions.Add("Name");

            List<Construction> allConstructions = constructions.Where(x => x != null).ToList();
            List<Construction> uniqueConstructions = BH.Engine.Diffing.Modify.RemoveDuplicatesByHash<Construction>(allConstructions, cc).ToList();

            return uniqueConstructions;
        }
    }
}





