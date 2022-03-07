/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Diffing;
using BH.oM.Base;
using kellerman = KellermanSoftware.CompareNetObjects;
using System.Reflection;
using BH.Engine.Base;

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        [Description("Checks two BHoMObjects property by property and returns the differences.")]
        [Input("obj1", "First object to compare for differences.")]
        [Input("obj2", "Second object to compare for differences.")]
        [Input("comparisonConfig", "Additional configurations to be used for the comparison.")]
        [Output("Dictionary whose Key is the Full Name of the property that is different among the two objects, " +
            "and whose Value is a Tuple: Item1 is the value of the property in obj1, Item2 the value in obj2." +
            "\nThis dictionary can be 'exploded' in the UI by using the `ListDifferentProperties()` method." +
            "\nIf no difference was found, returns null.")]
        public static Dictionary<string, Tuple<object,object>> DifferentProperties(this object obj1, object obj2, ComparisonConfig comparisonConfig = null)
        {
            // Call Query.ObjectDifferences(). 
            // This method returns an object `ObjectDifferences`, which stores differences in a "temporal" manner (e.g. pastValue, followingValue, etc.),
            // because it is used for change control purposes.
            // Instead, the purpose of this current method `DifferentProperties()` is to return a similar result, but without the "temporal" qualification.
            ObjectDifferences objectDifferences = Query.ObjectDifferences(obj1, obj2, comparisonConfig);

            if (objectDifferences == null)
                return null;

            // Group the `ObjectDifferences` in a Dictionary.
            Dictionary<string, Tuple<object, object>> result = objectDifferences.Differences.GroupBy(d => d.FullName)
                .ToDictionary(
                g => g.Key,
                g => 
                {
                    // By grouping by FullName, only one propertyDifference will be found per group.
                    IPropertyDifference firstPropertyDifference = g.ToList().FirstOrDefault();

                    return new Tuple<object, object>(firstPropertyDifference.PastValue, firstPropertyDifference.FollowingValue);
                });

            return result;
        }
    }
}



