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
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return the list of inherited types that are limited to one per collection.")]
        [Input("type", "The type of object you want to apply the fragment to.")]
        public static List<Type> UniquenessRestrictions(this Type type)
        {
            if (type == null)
                return new List<Type>();

            // Do not cover value types
            if (type.IsValueType)
                return new List<Type>();

            // Consider all interface types
            List<Type> types = type.GetInterfaces().ToList();

            // Consider all inherited types
            Type baseType = type.BaseType;
            while (baseType != typeof(object))
            {
                types.Add(baseType);
                baseType = baseType.BaseType;
            }

            // Consider the type itself
            types.Add(type);

            // Only keep the types that need to be unique
            return types.Where(x => x.IsMarkedAsUnique()).ToList();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool IsMarkedAsUnique(this Type type)
        {
            return type.GetCustomAttributes(typeof(UniqueAttribute), false).OfType<UniqueAttribute>().Count() > 0;
        }

        /***************************************************/
    }
}




