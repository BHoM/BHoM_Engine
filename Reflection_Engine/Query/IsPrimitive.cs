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
using System.Reflection;
using System.Linq;
using Mono.Cecil;
using Mono.Reflection;
using BH.Engine.Base;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether a type is of a primitive C# type.")]
        [Input("type", "Type to be determined.")]
        [Input("includeStrings", "(Optional, defaults to true) Whether strings should count as primitive types.")]
        [Input("includeValueTypes", "(Optional, defaults to true) Whether value types should count as primitive types.")]
        public static bool IsPrimitive(this Type type, bool includeStrings = true, bool includeValueTypes = true)
        {
            if (type == null)
                return false;

            bool result = type.IsPrimitive;

            if (includeStrings)
                result |= type == typeof(string);

            if (includeValueTypes)
                result |= type.IsValueType;

            return result;
        }

        /***************************************************/
    }
}



