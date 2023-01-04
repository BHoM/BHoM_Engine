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

namespace BH.Engine.Serialiser.Conventions
{
    /// <summary>
    /// Maps a BHoM IImmutable type. 
    /// </summary>
    public class BHoMDictionaryConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            Type memberType = memberMap.MemberType;
            TypeInfo typeInfo = memberType.GetTypeInfo();

            if (typeInfo.Name == "Dictionary`2" || typeInfo.Name == "ReadOnlyDictionary`2")
            {
                Type keyType = typeInfo.GenericTypeArguments[0];

                DictionaryRepresentation representation = DictionaryRepresentation.Document;
                if (keyType != typeof(string))
                    representation = DictionaryRepresentation.ArrayOfDocuments;

                var serializer = memberMap.GetSerializer();
                var dictionaryRepresentationConfigurable = serializer as IDictionaryRepresentationConfigurable;
                if (dictionaryRepresentationConfigurable != null && dictionaryRepresentationConfigurable.DictionaryRepresentation != representation)
                {
                    var reconfiguredSerializer = dictionaryRepresentationConfigurable.WithDictionaryRepresentation(representation);
                    memberMap.SetSerializer(reconfiguredSerializer);
                }
                return;
            }
        }
    }
}




