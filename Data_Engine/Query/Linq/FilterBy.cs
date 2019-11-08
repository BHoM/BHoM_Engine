/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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


using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BHRE = BH.Engine.Reflection;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Filters a list of objects to output only those with a property value matching that input")]
        [Input("objects", "List of objects to be filtered. All objects in the list should be of similar type")]
        [Input("propertyName", "The name of the property to filter the objects by")]
        [Input("value", "The value of the selected property to filter the objects by")]
        public static List<T> FilterBy<T>(this List<T> objects, string propertyName = null, object value = null, bool ignoreStringCase = false, bool exactStringMatch = false, double rounding = 0.0)
        {
            if (objects == null || objects.Count == 0) return new List<T>();
            if (propertyName == null) return new List<T>();
            if (value == null) return new List<T>();
            System.Reflection.PropertyInfo prop = objects.First(x => x != null).GetType().GetProperty(propertyName);
            int groupedObjects = objects.GroupBy(x => x.GetType()).Count();
            if (groupedObjects != 1)
            {
                BHRE.Compute.RecordError("All objects in the list to be sorted should be of similiar type.");
                return null;
            }
            if (!propertyName.Contains("."))
            {
                if (prop != null)
                {
                    if (value.GetType() == typeof(System.String))
                    {
                        if (ignoreStringCase)
                            return objects.Where(x => string.Equals((string)prop.GetValue(x), (string)value, System.StringComparison.CurrentCultureIgnoreCase)).ToList();
                        if (!exactStringMatch)
                            return objects.Where(x => (prop.GetValue(x) as string).ToUpper().Contains((value as string).ToUpper())).ToList();
                        return objects.Where(x => prop.GetValue(x).Equals(value)).ToList();
                    }
                return objects.Where(x => prop.GetValue(x) == value).ToList();
                }
            }
            BHRE.Compute.RecordNote("CustomData or nested property is used as the sorting property (using 'Object.Property.Property...') which is slower than a base property.");
            return objects.Where(x => BHRE.Query.PropertyValue(x, propertyName).Equals(value)).ToList();
        }
        /***************************************************/

    }
}