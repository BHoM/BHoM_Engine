/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

/* Copyright 2010-2016 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MongoDB.Bson.Serialization.Conventions
{
    /// <summary>
    /// A convention that uses the names of the creator parameters to find the matching members.
    /// </summary>
    public class ImmutableBHoMCreatorMapConvention : ConventionBase, ICreatorMapConvention
    {
        // public methods
        /// <summary>
        /// Applies a modification to the creator map.
        /// </summary>
        /// <param name="creatorMap">The creator map.</param>
        public void Apply(BsonCreatorMap creatorMap)
        {
            if (creatorMap.Arguments == null)
            {
                if (creatorMap.MemberInfo != null)
                {
                    var parameters = GetParameters(creatorMap.MemberInfo);
                    if (parameters != null)
                    {
                        var arguments = new List<MemberInfo>();

                        foreach (var parameter in parameters)
                        {
                            var argument = FindMatchingArgument(creatorMap.ClassMap.ClassType, parameter);
                            if (argument == null)
                            {
                                return;
                            }
                            arguments.Add(argument);
                        }

                        creatorMap.SetArguments(arguments);
                    }
                }
            }
        }

        // private methods
        private MemberInfo FindMatchingArgument(Type classType, ParameterInfo parameter)
        {
            MemberInfo argument;
            if ((argument = Match(classType, MemberTypes.Property, BindingFlags.Public, parameter)) != null)
            {
                return argument;
            }
            if ((argument = Match(classType, MemberTypes.Field, BindingFlags.Public, parameter)) != null)
            {
                return argument;
            }
            if ((argument = Match(classType, MemberTypes.Property, BindingFlags.NonPublic, parameter)) != null)
            {
                return argument;
            }
            if ((argument = Match(classType, MemberTypes.Field, BindingFlags.NonPublic, parameter)) != null)
            {
                return argument;
            }
            return null;
        }

        private Type GetMemberType(MemberInfo memberInfo)
        {
            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            // should never happen
            throw new BsonInternalException();
        }

        private IEnumerable<ParameterInfo> GetParameters(MemberInfo memberInfo)
        {
            var constructorInfo = memberInfo as ConstructorInfo;
            if (constructorInfo != null)
            {
                return constructorInfo.GetParameters();
            }

            var methodInfo = memberInfo as MethodInfo;
            if (methodInfo != null)
            {
                return methodInfo.GetParameters();
            }

            return null;
        }

        private MemberInfo Match(Type classType, MemberTypes memberType, BindingFlags visibility, ParameterInfo parameter)
        {
            var classTypeInfo = classType.GetTypeInfo();
            var bindingAttr = BindingFlags.IgnoreCase | BindingFlags.Instance;
            var memberInfos = classTypeInfo.GetMember(parameter.Name, memberType, bindingAttr | visibility);
            if (memberInfos.Length == 1 /*&& GetMemberType(memberInfos[0]) == parameter.ParameterType*/)
            {
                return memberInfos[0];
            }
            else
            {
                List<PropertyInfo> propertiesOfType = classType.GetProperties().Where(x => x.PropertyType == parameter.ParameterType).ToList();
                if (propertiesOfType.Count == 1)
                    return propertiesOfType[0];
            }
            return null;
        }
    }
}



