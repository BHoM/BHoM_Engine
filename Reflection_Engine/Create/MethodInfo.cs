/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        [Description("Creates a method declared by the provided type, with the specified name and matching parameter types. For generic methods, the generic parameters are replaced by types satisfying their constraints before the parameter types are compared.")]
        [Input("declaringType", "The type declaring the method. Inherited methods are not considered.")]
        [Input("methodName", "The name of the method to extract.")]
        [Input("paramTypes", "The types of the parameters of the method, in order. A null entry matches any type for that parameter. If null, the method is expected to have no parameters.")]
        [Output("methodInfo", "The first method matching the provided name and parameter types, or null if none was found.")]
        public static MethodInfo MethodInfo(this Type declaringType, string methodName, List<Type> paramTypes)
        {
            if (declaringType == null)
            {
                Base.Compute.RecordError("Cannot create a method info from a null type.");
                return null;
            }

            if (methodName == null)
            {
                Base.Compute.RecordError("Cannot create a method info from a null method name.");
                return null;
            }

            if (paramTypes == null)
            {
                Base.Compute.RecordWarning("The 'paramTypes' input is null and was replaced by an empty list");
                paramTypes = new List<Type>();
            }

            MethodInfo foundMethod = null;
            List<MethodInfo> methods = declaringType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).ToList();

            for (int k = 0; k < methods.Count; k++)
            {
                MethodInfo method = methods[k];

                if (method.Name == methodName)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == paramTypes.Count)
                    {
                        if (method.ContainsGenericParameters)
                        {
                            method = method.MakeFromGeneric();   // Falls back to the generic method if the constraints cannot be satisfied
                            parameters = method.GetParameters();
                        }

                        bool matching = true;
                        for (int i = 0; i < paramTypes.Count; i++)
                        {
                            matching &= (paramTypes[i] == null || parameters[i].ParameterType == paramTypes[i]);
                        }
                        if (matching)
                        {
                            foundMethod = method;
                            break;
                        }
                    }
                }
            }

            return foundMethod;
        }

        /*************************************/
    }
}







