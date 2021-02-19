/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Reflection;
using BH.oM.Base;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sorts methods based on methods first parameters closeness to the type. First implementation will only rate exact type matching over non exact")]
        [Input("methods", "The list of extentionmethods to sort. Will assume the first inputparameter of the methods to be of a type assignable from the provided Type")]
        [Input("type", "Type to check closeness to. Assumed to match first input parameter of the methods")]
        [Output("metods", "Sorted methods")]
        public static List<MethodInfo> SortExtensionMethods(this IEnumerable<MethodInfo> methods, Type type)
        {
            return methods.OrderBy(x => methods.Count(y => x.GetParameters()[0].ParameterType.IsAssignableFrom(y.GetParameters()[0].ParameterType))).ToList();
        }

        /***************************************************/
    }
}


