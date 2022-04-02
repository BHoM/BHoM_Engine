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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MethodInfo MakeGenericFromInputs(this MethodInfo method, List<Type> inputTypes)
        {
            if(method == null)
            {
                Compute.RecordError("Cannot make generic from inputs of a null method.");
                return null;
            }

            if(inputTypes == null)
            {
                Compute.RecordWarning("The 'inputTypes' input is null and was replaced by an empty list");
                return null;
            }

            if (!method.IsGenericMethod)
                return method;

            List<Type> paramTypes = method.GetParameters().Select(x => x.ParameterType).ToList();

            // Get where the generic arguments are actually used
            Dictionary<string, Type> dic = new Dictionary<string, Type>();
            for (int i = 0; i < paramTypes.Count; i++)
            {
                Type paramType = paramTypes[i];
                if (paramType.IsGenericType || paramType.IsGenericParameter)
                    MatchGenericParameters(paramTypes[i], inputTypes[i], ref dic);
            }

            // Actually make the generic method
            List<Type> actualTypes = method.GetGenericArguments().Select(x => dic.ContainsKey(x.Name) ? dic[x.Name] : typeof(object)).ToList();
            return method.MakeGenericMethod(actualTypes.ToArray());
        }

        /*****************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private static void MatchGenericParameters(Type genericType, Type targetType, ref Dictionary<string, Type> dic)
        {
            if (genericType.IsGenericParameter)
            {
                if (!dic.ContainsKey(genericType.Name))
                    dic[genericType.Name] = targetType;
            }
            else if (targetType.IsGenericType)
            {
                Type[] targetArguments = targetType.GetGenericArguments();
                Type[] genericArguments = genericType.GetGenericArguments();
                if (targetArguments.Length == genericArguments.Length)
                {
                    for (int i = 0; i < targetArguments.Length; i++)
                        MatchGenericParameters(genericArguments[i], targetArguments[i], ref dic);
                }
            }
            else
            {
                Type[] interfaces = targetType.GetInterfaces();
                foreach (Type inter in targetType.GetInterfaces())
                {
                    if (inter.Name == genericType.Name)
                    {
                        MatchGenericParameters(genericType, inter, ref dic);
                    }
                }

                Type baseType = targetType.BaseType;
                while (baseType != null && baseType != typeof(object))
                {
                    if (baseType.IsGenericType)
                        MatchGenericParameters(genericType, baseType, ref dic);
                    baseType = baseType.BaseType;
                }
            }
        }

        /***************************************************/
    }
}



