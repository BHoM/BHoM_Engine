/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using System.Reflection;

namespace BH.Engine.Serialiser
{
    public static partial class Create
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static MethodBase MethodBase(Type type, string methodName, List<string> paramTypesJson)
        {
            List<MethodBase> methods;
            if (methodName == ".ctor")
                methods = type.GetConstructors().ToList<MethodBase>();
            else
                methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).ToList<MethodBase>();

            for (int k = 0; k < methods.Count; k++)
            {
                MethodBase method = methods[k];

                if (method.Name == methodName)
                {
                    ParameterInfo[] parameters = method.ParametersWithConstraints();
                    if (parameters.Length == paramTypesJson.Count)
                    {
                        bool matching = true;
                        List<string> names = parameters.Select(x => x.ParameterType.Name).ToList();
                        for (int i = 0; i < paramTypesJson.Count; i++)
                            matching &= names[i] == paramTypesJson[i];

                        if (matching)
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }

        /*******************************************/

        public static MethodBase MethodBase(Type type, string methodName, List<Type> paramTypes)
        {
            if (methodName == ".ctor")
                return type.GetConstructor(paramTypes.ToArray());
            else
                return type.GetMethod(methodName, paramTypes.ToArray()); 
        }

        /*******************************************/
    }
}
