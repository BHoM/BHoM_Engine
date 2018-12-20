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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BH.Engine.Serialiser.Objects.MemberMapConventions
{
    public class GenericDiscriminatorConvention : IDiscriminatorConvention
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string ElementName { get { return "_t"; } }


        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            // the BsonReader is sitting at the value whose actual type needs to be found
            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType == BsonType.Document)
            {
                // ensure KnownTypes of nominalType are registered (so IsTypeDiscriminated returns correct answer)
                //BsonSerializer.EnsureKnownTypesAreRegistered(nominalType);

                // we can skip looking for a discriminator if nominalType has no discriminated sub types
                if (BsonSerializer.IsTypeDiscriminated(nominalType))
                {
                    var bookmark = bsonReader.GetBookmark();
                    bsonReader.ReadStartDocument();
                    var actualType = nominalType;
                    if (bsonReader.FindElement(ElementName))
                    {
                        var context = BsonDeserializationContext.CreateRoot(bsonReader);
                        var discriminator = BsonValueSerializer.Instance.Deserialize(context);
                        if (discriminator.IsBsonArray)
                        {
                            discriminator = discriminator.AsBsonArray.Last(); // last item is leaf class discriminator
                        }
                        actualType = BsonSerializer.LookupActualType(nominalType, discriminator);
                    }
                    bsonReader.ReturnToBookmark(bookmark);
                    return actualType;
                }
            }

            return nominalType;
        }

        /***************************************************/

        public BsonValue GetDiscriminator(Type nominalType, Type actualType)
        {
            if (actualType.IsGenericType)
                return actualType.AssemblyQualifiedName;
            else
                return actualType.FullName;
        }

        /***************************************************/
    }
}
