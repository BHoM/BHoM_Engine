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

using BH.oM.Base.Attributes;
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

        [Description("Orders a list of objects by a property of the object - e.g. input 'Name' to sort objects by their names. Note that not all object properties have sortable values")]
        [Input("objects", "List of objects to be sorted. All objects in the list should be of similar type")]
        [Input("propertyName", "The name of the property to sort the list of objects by. Note that the property must be able to be sorted - e.g. numbers/text")]
        [Output("orderedObjects", "The collection of objects ordered by the given parameters")]
        public static List<T> OrderBy<T>(this List<T> objects, string propertyName)
        {
            if (objects == null || objects.Count == 0)
            {
                BH.Engine.Base.Compute.RecordWarning("No objects submitted to order");
                return new List<T>();
            }

            objects = objects.Where(x => x != null).ToList();

            if(objects.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError("All objects in the list to order are null, please try with valid objects");
                return new List<T>();
            }

            int groupedObjects = objects.GroupBy(x => x.GetType()).Count();
            if (groupedObjects != 1)
            {
                BH.Engine.Base.Compute.RecordError("All objects in the list to be sorted should be of the exact same type.");
                return null;
            }
            try
            {
                if (!propertyName.Contains("."))
                {
                    System.Reflection.PropertyInfo prop = objects.First().GetType().GetProperty(propertyName);
                    if (prop != null)
                        return objects.OrderBy(x => prop.GetValue(x)).ToList();
                }
                BH.Engine.Base.Compute.RecordNote("CustomData or nested property is used as the sorting property (using 'Object.Property.Property...') which is slower than a base property.");
                return objects.OrderBy(x => BH.Engine.Base.Query.PropertyValue(x, propertyName)).ToList();
            }
            catch
            {
                BH.Engine.Base.Compute.RecordWarning("The sorting property does not have a sort function.");
                return null;
            }
        }

        /***************************************************/
    }
}



