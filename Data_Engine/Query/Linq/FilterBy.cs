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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Base;

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
        [Input("ignoreStringCase", "Ignore upper/lower case letters if the property filtered by is a text/string.")]
        [Input("exactStringMatch", "If true checks if the property filtered by contains the text value (filtering must be a string/text)")]
        [Output("filteredObjects", "The collection of objects that match the given filter conditions")]
        public static List<T> FilterBy<T>(this List<T> objects, string propertyName, object value, bool ignoreStringCase = false, bool exactStringMatch = true)
        {
            if (objects == null || objects.Count == 0)
            {
                BH.Engine.Base.Compute.RecordWarning("No objects submitted to filter");
                return new List<T>();
            }

            objects = objects.Where(x => x != null).ToList();

            if (objects.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError("All objects in the list to filter are null, please try with valid objects");
                return new List<T>();
            }

            if (propertyName == null)
            {
                BH.Engine.Base.Compute.RecordError("propertyName cannot be null in order to filter the objects");
                return new List<T>();
            }

            if (value == null)
            {
                BH.Engine.Base.Compute.RecordError("value cannot be null to filter the objects");
                return new List<T>();
            }

            object firstObject = objects.First();
            System.Type type = firstObject.PropertyValue(propertyName)?.GetType();
            
            if(type == null)
            {
                BH.Engine.Base.Compute.RecordError("That property name could not be resolved to a specific object type. Please check the property name and try again");
                return new List<T>();
            }

            List<T> filteredObjects = new List<T>();

            if (type == typeof(System.String))
            {
                if (!exactStringMatch)
                {
                    if(ignoreStringCase) //Convert property and value to lower case and check if it's contained
                        filteredObjects.AddRange(objects.Where(x => x.PropertyValue(propertyName) != null && x.PropertyValue(propertyName).ToString().ToLower().Contains(value.ToString().ToLower())).ToList());
                    else
                        filteredObjects.AddRange(objects.Where(x => x.PropertyValue(propertyName) != null && x.PropertyValue(propertyName).ToString().Contains(value.ToString())).ToList());
                }
                else
                {
                    if (ignoreStringCase)
                        filteredObjects.AddRange(objects.Where(x => x.PropertyValue(propertyName) != null && x.PropertyValue(propertyName).ToString().Equals(value.ToString(), System.StringComparison.CurrentCultureIgnoreCase)).ToList());
                    else
                        filteredObjects.AddRange(objects.Where(x => x.PropertyValue(propertyName) != null && x.PropertyValue(propertyName).ToString().Equals(value.ToString())).ToList());
                }
            }
            else
            {
                dynamic val = System.Convert.ChangeType(value, type);
                filteredObjects = objects.Where(x => (System.Convert.ChangeType(x.PropertyValue(propertyName), type) as dynamic) == val).ToList();
            }

            return filteredObjects.Distinct().ToList();
        }

        /***************************************************/

    }
}



