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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Collections;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Unpacks the contents of the input IContainer. The contents are flattened into a list of objects." + 
            "The flattening supports properties of IContainer that are Lists, List of Lists, Dictionaries (the values are flattened) and Dictionaries with a Value that is a list (the list is flattened)." + 
            "Any other nested datastructure has its elements returned as-is.")]
        [Input("container", "The IContainer to be unpacked.")]
        [Output("objs", "Objects unpacked from the container.")]
        public static IEnumerable<object> Unpack(this BH.oM.Base.IContainer container)
        {
            var result = new List<object>();

            var propValues = container.GetType().GetProperties()
                .Where(p => p.CanRead && p.GetMethod.GetParameters().Length == 0)
                .Select(p => p.GetValue(container, null));

            foreach (var propValue in propValues)
            {
                if (propValue is IEnumerable enumerable && !(propValue is string))
                {
                    if (propValue is IDictionary)
                        enumerable = ((IDictionary)propValue).Values;

                    foreach (var enumItem in enumerable)
                    {
                        if (enumItem is IEnumerable nestedEnum && !(enumItem is string))
                        {
                            foreach (var enumOfEnumItem in nestedEnum)
                            {
                                result.Add(enumOfEnumItem);
                            }
                        }
                        else
                            result.Add(enumItem);
                    }
                }
                else
                    result.Add(propValue);
            }

            return result;
        }

        /***************************************************/
    }
}