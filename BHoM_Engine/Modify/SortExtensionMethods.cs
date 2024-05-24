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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Reflection;
using BH.oM.Base;

namespace BH.Engine.Base
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
            if (methods == null || methods.Count() == 0)
                return new List<MethodInfo>();

            if (type == null)
            {
                Compute.RecordWarning("Cannot sort methods based on a null type. The original list will be returned.");
                return methods.ToList();
            }

            List<List<Type>> hierarchy = type.InheritanceHierarchy();
            IEnumerable<int> levels = methods.Select(x => hierarchy.InheritanceLevel(x.GetParameters()[0].ParameterType));
            return methods.Zip(levels, (m, l) => new { m, l }).Where(x => x.l != -1).OrderBy(x => x.l).Select(x => x.m).ToList();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<List<Type>> InheritanceHierarchy(this Type type)
        {
            List<Type> typeAncestry = new List<Type>();
            Type ancestor = type;
            while (ancestor != null && ancestor != typeof(object))
            {
                typeAncestry.Add(ancestor);
                ancestor = ancestor.BaseType;
            }

            List<List<Type>> result = new List<List<Type>>();

            for (int i = typeAncestry.Count - 1; i >= 0; i--)
            {
                Type child = typeAncestry[i];
                result.Insert(0, new List<Type> { child });

                IEnumerable<Type> alreadyMapped = result.SelectMany(x => x);
                List<List<Type>> interfaces = child.InterfaceHierarchy();
                for (int j = 0; j < interfaces.Count; j++)
                {
                    IEnumerable<Type> notMapped = interfaces[j].Except(alreadyMapped);
                    if (notMapped.Any())
                    {
                        if (j + 1 < result.Count)
                            result[j + 1].AddRange(notMapped);
                        else
                            result.Add(notMapped.ToList());
                    }
                    else
                        break;
                }
            }

            return result;
        }

        /***************************************************/

        private static List<List<Type>> InterfaceHierarchy(this Type type)
        {
            Type[] interfaces = type.GetInterfaces();
            return interfaces.GroupBy(x => interfaces.Count(y => x.IsAssignableFrom(y))).OrderBy(x => x.Key).Select(x => x.ToList()).ToList();
        }

        /***************************************************/

        private static int InheritanceLevel(this List<List<Type>> hierarchy, Type type)
        {
            // If target type of the extension method is object, return max inheritance level (to be treated as last resort)
            if (type == typeof(object))
                return int.MaxValue;

            for (int i = 0; i < hierarchy.Count; i++)
            {
                if (!type.IsGenericType)
                {
                    if (hierarchy[i].Contains(type))
                        return i;
                }
                else if (hierarchy[i].Any(x => x.IsAssignableFromIncludeGenerics(type)))
                    return i;
            }

            return -1;
        }

        /***************************************************/
    }
}





