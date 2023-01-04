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
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using System.Diagnostics;

namespace BH.Engine.Serialiser.MemberMapConventions
{
    /// <summary>
    /// Maps a fully immutable type that is not a IImmutable type. 
    /// </summary>
    public class ImmutableTypeClassMapConventionFixed : ConventionBase, IClassMapConvention
    {
        /// <inheritdoc />
        public void Apply(BsonClassMap classMap)
        {
            var typeInfo = classMap.ClassType.GetTypeInfo();
            if (typeInfo.IsAbstract)
            {
                return;
            }

            if (typeInfo.GetInterface("IImmutable") != null)
            {
                return; // only applies to classes that do not inherit from IImutable
            }

            if (typeInfo.GetConstructor(Type.EmptyTypes) != null)
            {
                return;
            }

            var properties = typeInfo.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Any(p => p.CanWrite))
            {
                return; // a type that has any writable properties is not immutable
            }

            var anyConstructorsWereMapped = false;
            foreach (var ctor in typeInfo.GetConstructors())
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length != properties.Length)
                {
                    continue; // only consider constructors that have sufficient parameters to initialize all properties
                }

                var matches = parameters
                    .GroupJoin(properties,
                        parameter => parameter.Name,
                        property => property.Name,
                        (parameter, props) => new { Parameter = parameter, Properties = props },
                        StringComparer.OrdinalIgnoreCase);

                if (matches.Any(m => m.Properties.Count() != 1))
                {
                    continue;
                }

                classMap.MapConstructor(ctor);

                anyConstructorsWereMapped = true;
            }

            if (anyConstructorsWereMapped)
            {
                // if any constructors were mapped by this convention then map all the properties also
                foreach (var property in properties)
                {
                    try
                    {
                        var memberMap = classMap.MapMember(property);
                        if (classMap.IsAnonymous)
                        {
                            var defaultValue = memberMap.DefaultValue;
                            memberMap.SetDefaultValue(defaultValue);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}




