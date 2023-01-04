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

using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;

namespace BH.Engine.Serialiser.MemberMapConventions
{
    /// <summary>
    /// Maps a BHoM IImmutable type. 
    /// </summary>
    public class ImmutableBHoMClassMapConvention : ConventionBase, IClassMapConvention
    {
        public void Apply(BsonClassMap classMap)
        {
            var typeInfo = classMap.ClassType.GetTypeInfo();
            if (typeInfo.IsAbstract)
            {
                return;
            }

            if (typeInfo.GetInterface("IImmutable") == null)
            {
                return; // only applies to classes that inherit from IImutable
            }

            var properties = typeInfo.GetProperties();

            var anyConstructorsWereMapped = false;
            ConstructorInfo[] constructors = typeInfo.GetConstructors();
            if (constructors.Length > 0)
            {
                var ctor = typeInfo.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();

                var parameters = ctor.GetParameters();

                var matches = parameters
                    .GroupJoin(properties,
                        parameter => parameter.Name,
                        property => property.Name,
                        (parameter, props) => new { Parameter = parameter, Properties = props },
                        StringComparer.OrdinalIgnoreCase);

                if (matches.Any(m => m.Properties.Count() != 1))
                {
                    //continue;
                }

                classMap.MapConstructor(ctor);

                anyConstructorsWereMapped = true;
            }

            if (anyConstructorsWereMapped)
            {
                var classType = classMap.ClassType;
                
                foreach (var property in properties)
                {
                    if (property.DeclaringType == classType && !IsOverridden(property))
                        classMap.MapMember(property);
                    else if(!property.CanWrite && classType.BaseType != null && classType.BaseType.IsAbstract)
                    {
                        //Forcing immutable properties from base class to be added via reflection.
                        //This is due to BsonClassMap refusing to add members from base class to the class map which is needed
                        //for IImmutable members withg an abstract immutable base class.
                        var memberMap = classMap.DeclaredMemberMaps.ToList().Find(m => m.MemberInfo == property);
                        if (memberMap == null)
                        {
                            memberMap = new BsonMemberMap(classMap, property);

                            FieldInfo info = typeof(BsonClassMap).GetField("_declaredMemberMaps", BindingFlags.NonPublic | BindingFlags.Instance);
                            var declaredMemberMaps = info.GetValue(classMap) as List<BsonMemberMap>;

                            declaredMemberMaps.Add(memberMap);
                        }

                    }

                }
            }
        }

        /***************************************************/

        private bool IsOverridden(PropertyInfo property)
        {
            var getMethod = property.GetGetMethod(false);
            if (getMethod != null)
            {
                return getMethod.GetBaseDefinition() != getMethod;
            }
            else
            {
                //This case should not happen for compliant BHoMObjects, as all properties should have getters, but adding to be safe.
                var setMethod = property.GetSetMethod(false);
                return setMethod.GetBaseDefinition() != setMethod;
            }
        }

        /***************************************************/
    }
}




