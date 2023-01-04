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
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson;

namespace BH.Engine.Serialiser.Conventions
{
    /// <summary>
    /// Maps a BHoM IImmutable type. 
    /// </summary>
    public class BHoMEnumConvention : ConventionBase, IMemberMapConvention
    {
        /*******************************************/
        /**** Interface Methods                 ****/
        /*******************************************/

        public void Apply(BsonMemberMap memberMap)
        {
            var memberType = memberMap.MemberType;
            var memberTypeInfo = memberType.GetTypeInfo();

            if (memberTypeInfo.IsEnum)
            {
                var serializer = memberMap.GetSerializer();
                var representationConfigurableSerializer = serializer as IRepresentationConfigurable;
                if (representationConfigurableSerializer != null)
                {
                    var reconfiguredSerializer = representationConfigurableSerializer.WithRepresentation(BsonType.String);
                    memberMap.SetSerializer(reconfiguredSerializer);
                }
                return;
            }

            if (IsNullableEnum(memberType))
            {
                var serializer = memberMap.GetSerializer();
                var childSerializerConfigurableSerializer = serializer as IChildSerializerConfigurable;
                if (childSerializerConfigurableSerializer != null)
                {
                    var childSerializer = childSerializerConfigurableSerializer.ChildSerializer;
                    var representationConfigurableChildSerializer = childSerializer as IRepresentationConfigurable;
                    if (representationConfigurableChildSerializer != null)
                    {
                        var reconfiguredChildSerializer = representationConfigurableChildSerializer.WithRepresentation(BsonType.String);
                        var reconfiguredSerializer = childSerializerConfigurableSerializer.WithChildSerializer(reconfiguredChildSerializer);
                        memberMap.SetSerializer(reconfiguredSerializer);
                    }
                }
                return;
            }
        }


        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private bool IsNullableEnum(Type type)
        {
            return
                type.GetTypeInfo().IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                Nullable.GetUnderlyingType(type).GetTypeInfo().IsEnum;
        }

        /*******************************************/
    }
}




