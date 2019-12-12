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

using MongoDB.Bson.Serialization.Conventions;
using System;
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
                    if (property.DeclaringType == classType)
                        classMap.MapMember(property);
                    else if (classMap is BH.Engine.Serialiser.Conventions.BHoMBsonClassMap)
                    {
                        BH.Engine.Serialiser.Conventions.BHoMBsonClassMap bhClassMap =classMap as BH.Engine.Serialiser.Conventions.BHoMBsonClassMap;
                        bhClassMap.MapImmutableMember(property);
                    }

                }
            }
        }
    }
}
