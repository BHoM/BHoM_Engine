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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Filters a list of objects by a type. Any objects that is assignable from the provided type will be returned.")]
        [Input("list", "The list of elements to filter")]
        [Input("type", "Type to filter by")]
        [Output("List", "Filtered list containing only objects assignable from the provided type")]
        public static List<object> FilterByType(this IEnumerable<object> list, Type type)
        {
            return list.Where(x => x.GetType().IsAssignableFrom(type)).ToList();
        }

        /***************************************************/

        [Description("Returns a collection of objects which are of the provided object type")]
        [Input("objects", "A collection of generic BHoM objects")]
        [Input("type", "The type of object to be queried and returned")]
        [Output("bhomObjects", "A collection of generic BHoM objects matching the provided type")]
        public static List<IBHoMObject> ObjectsByType(this List<IBHoMObject> objects, Type type)
        {
            return objects.Where(x => x.GetType() == type).ToList();
        }
    }
}
