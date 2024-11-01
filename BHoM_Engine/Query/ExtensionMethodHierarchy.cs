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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Builds a hierarchy of methods based on distance of each method's input parameter to the correspondent input type, first to first, second to second etc." +
            "\nThree levels of hierarchy are returned:" +
            "\n- first level of hierarchy groups the output by order of inputs, i.e. first sublist corresponds to first input type and first method parameter, second to second etc." +
            "\n- second level of hierarchy groups methods by the distance in inheritance hierarchy between their input parameter and the correspondent input type" +
            "\n- third level of hierarchy is an unordered set of methods with same distance in inheritance hierarchy between their input parameter and the correspondent input type.")]
        [Input("methods", "The list of extension methods to sort. Will assume the input parameters of the methods to be of a type assignable from the provided types.")]
        [Input("types", "Types to check closeness to. First provided type is assumed to match first input parameter of the methods, others to follow respectively.")]
        [Output("hierarchy", "Hierarchy of input methods based on distance of each method's parameters to the input types.")]
        public static List<List<List<MethodInfo>>> ExtensionMethodHierarchy(this IEnumerable<MethodInfo> methods, IEnumerable<Type> types)
        {
            if (methods == null || types == null || !types.Any())
                return null;

            if (methods.Count() == 0)
                return new List<List<List<MethodInfo>>>();

            // Build inheritance hierarchy for the input types
            // Each item of the top list represents hierarchy for each input type
            List<List<List<MethodInfo>>> result = new List<List<List<MethodInfo>>>();
            int i = 0;
            foreach(Type type in types)
            {
                if (type == null)
                {
                    // If null input, all methods taking nullable types would be as suitable
                    List<MethodInfo> applicableMethods = methods.Where(x => x.GetParameters()[i].ParameterType.IsNullable()).ToList();
                    result.Add(new List<List<MethodInfo>> { applicableMethods });
                }
                else
                {
                    // Build inheritance hierarchy for the input type
                    List<List<Type>> inheritanceHierarchy = type.InheritanceHierarchy();

                    // Organise methods into hierarchy based on hierarchy of types they extend
                    Dictionary<int, List<MethodInfo>> methodHierarchy = new Dictionary<int, List<MethodInfo>>();
                    foreach (MethodInfo method in methods)
                    {
                        int hierarchyLevel = inheritanceHierarchy.InheritanceLevel(method.GetParameters()[i].ParameterType);
                        if (hierarchyLevel != -1)
                        {
                            if (!methodHierarchy.ContainsKey(hierarchyLevel))
                                methodHierarchy[hierarchyLevel] = new List<MethodInfo>();

                            methodHierarchy[hierarchyLevel].Add(method);
                        }
                    }

                    result.Add(methodHierarchy.OrderBy(x => x.Key).Select(x => x.Value).ToList());
                }

                i++;
            }

            return result;
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
