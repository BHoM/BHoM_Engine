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
using System.Collections;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Groups objects by their property values. Note that not all object properties have groupable values")]
        [Input("objects", "List of objects to be grouped. All objects in the list should be of similar type")]
        [Input("propertyName", "The name of the property to group the list of objects by.")]
        [Output("groupedObjects", "The collection of objects grouped by the given property")]
        public static List<List<T>> GroupBy<T>(this List<T> objects, string propertyName)
        {
            var grouping = objects.GroupBy(x => x.PropertyValue(propertyName));
            List<List<T>> groupedObjects = new List<List<T>>();

            foreach (var group in grouping)
                groupedObjects.Add(group.ToList());
            
            return groupedObjects;
        }

        /***************************************************/

    }
}



