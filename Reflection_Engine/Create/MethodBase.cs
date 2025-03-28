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

using BH.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        [Description("Extracts a method from the provided Type with the specified name and matching parameter types.")]
        public static MethodBase MethodBase(Type type, string methodName, List<string> paramTypeNames)
        {
            if(type == null)
            {
                Base.Compute.RecordError("Cannot create a method base from a null type.");
                return null;
            }
            
            if(methodName == null)
            {
                Base.Compute.RecordError("Cannot create a method base from a null method name.");
                return null;
            }

            if(paramTypeNames == null)
            {
                Base.Compute.RecordWarning("The 'paramTypeNames' input is null and was replaced by an empty list");
                paramTypeNames = new List<string>();
            }

            List<MethodBase> methods;
            if (methodName == ".ctor")
                methods = type.GetConstructors().ToList<MethodBase>();
            else
                methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .Where(x => x.Name == methodName)
                    .ToList<MethodBase>();

            foreach (MethodBase method in methods)
            {
                try
                {
                    ParameterInfo[] parameters = method.ParametersWithConstraints();
                    if (parameters.Length == paramTypeNames.Count)
                    {
                        bool matching = true;
                        List<string> names = parameters.Select(x => x.ParameterType.ToText(true)).ToList();
                        for (int i = 0; i < paramTypeNames.Count; i++)
                            matching &= names[i] == paramTypeNames[i];

                        if (matching)
                        {
                            return method;
                        }
                    }
                }
                catch { }
            }

            // If failed, look for a generic method that can satisfy the constraints
            methods = methods.Where(x => x.ContainsGenericParameters).ToList();
            foreach (MethodBase method in methods)
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == paramTypeNames.Count)
                {
                    List<Type> types = paramTypeNames.Select(x => Base.Create.Type(x)).ToList();

                    if (types.Any(x => x == null))
                        continue;

                    bool matching = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        Type paramType = parameters[i].ParameterType;
                        if (paramType.IsGenericType)
                        {
                            if (paramType.Name == types[i].Name)
                            {
                                try
                                {
                                    Type t = paramType.GetGenericTypeDefinition().MakeGenericType(types[i].GenericTypeArguments);
                                    matching &= t != null;
                                }
                                catch
                                {
                                    matching = false;
                                }
                            }
                            else
                            {
                                matching &= false;
                            }
                        }
                        else if (paramType.IsGenericParameter)
                        {
                            matching &= paramType.GenericTypeConstraint().IsAssignableFrom(types[i]);
                        }
                        else
                            matching &= parameters[i].ParameterType.ToText(true) == paramTypeNames[i];
                    }

                    if (matching)
                    {
                        if (method.IsGenericMethodDefinition && method is MethodInfo)
                        {
                            try
                            {
                                MethodInfo result = Base.Compute.MakeGenericFromInputs(method as MethodInfo, types);
                                if (result != null)
                                    return result;
                            }
                            catch { }
                        }
                        return method;
                    }
                }
            }

            return null;
        }

        /*******************************************/

        [Description("Extracts a method from the provided Type with the specified name and matching parameter types.")]
        public static MethodBase MethodBase(Type type, string methodName, List<Type> paramTypes)
        {
            if (type == null)
            {
                Base.Compute.RecordError("Cannot create a method base from a null type.");
                return null;
            }

            if (methodName == null)
            {
                Base.Compute.RecordError("Cannot create a method base from a null method name.");
                return null;
            }

            if (paramTypes == null)
            {
                Base.Compute.RecordWarning("The 'paramTypes' input is null and was replaced by an empty list");
                paramTypes = new List<Type>();
            }

            MethodBase method = null;
            try
            {
                if (methodName == ".ctor")
                    method = type.GetConstructor(paramTypes.ToArray());
                else
                    method = type.GetMethod(methodName, paramTypes.ToArray());
            }
            catch { }

            // the above will return null if the type is a generic type
            // that is because we serialise a generic method with its constraints
            // rather than serialising the generics
            if (method != null)
                return method;

            // So, let's try the other overload
            return MethodBase(type, methodName, paramTypes.Select(x => x.ToText(true)).ToList());
        }

        /*******************************************/
    }
}






