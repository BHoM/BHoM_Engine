/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Debugging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MethodInfo MakeGenericFromInputs(MethodInfo method, List<Type> inputTypes)
        {
            if (!method.IsGenericMethod)
                return method;

            Type[] genericArguments = method.GetGenericArguments();
            List<Type> paramTypes = method.GetParameters().Select(x => x.ParameterType).ToList();

            // Get where the generic arguments are actually used
            Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>(); 
            for (int i = 0; i < paramTypes.Count; i++)
            {
                Type type = paramTypes[i];
                if (type.IsGenericParameter)
                    dic[type.Name] = new List<int> { i };
                else if (type.ContainsGenericParameters)
                {
                    Type[] types = type.GetGenericArguments();
                    for (int j = 0; j < types.Length; j++)
                    {
                        if (!dic.ContainsKey(types[j].Name))
                            dic[types[j].Name] = new List<int> { i, j };
                    }
                }
            }

            // Now look at the same positions inside the inputTypes to find the matching generic types
            List<Type> actualTypes = new List<Type>();
            foreach (Type argument in genericArguments)
            {
                if (!dic.ContainsKey(argument.Name))
                {
                    actualTypes.Add(null);
                    continue;
                }

                List<int> indices = dic[argument.Name];
                if (indices == null || indices.Count == 0)
                    continue;

                int index = indices[0];
                Type paramType = paramTypes[index];
                Type type = inputTypes[index];

                if (type.Name != paramType.Name)
                {
                    foreach (Type inter in type.GetInterfaces())
                    {
                        if (inter.Name == paramType.Name)
                        {
                            type = inter;
                            break;
                        }
                    }
                }

                if (type.Name != paramType.Name)
                {
                    actualTypes.Add(null);
                    continue;
                }

                for (int i = 1; i < indices.Count; i++)
                {
                    index = indices[i];
                    Type[] types = type.GetGenericArguments();
                    if (types.Length > index)
                        type = types[index];
                }

                actualTypes.Add(type);
            }

            // Actually make the generic method
            return method.MakeGenericMethod(actualTypes.ToArray());
        }

        /***************************************************/
    }
}

