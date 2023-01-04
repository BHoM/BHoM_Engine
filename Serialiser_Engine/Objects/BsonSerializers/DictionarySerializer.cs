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

using BH.Engine.Reflection;
using BH.Engine.Versioning;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class DictionarySerializer : SerializerBase<IDictionary>, IBsonPolymorphicSerializer
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, IDictionary value)
        {
            var bsonWriter = context.Writer;

            var discriminator = m_DiscriminatorConvention.GetDiscriminator(typeof(object), value.GetType());
            bsonWriter.WriteStartDocument();
            bsonWriter.WriteName(m_DiscriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(context, discriminator);
            bsonWriter.WriteName("_v");
            bsonWriter.WriteStartArray();
            foreach (DictionaryEntry kvp in value as IDictionary)
            {
                bsonWriter.WriteStartDocument();
                bsonWriter.WriteName("k");
                BsonSerializer.Serialize(bsonWriter, kvp.Key);
                bsonWriter.WriteName("v");
                BsonSerializer.Serialize(bsonWriter, kvp.Value);
                bsonWriter.WriteEndDocument();
            }
            bsonWriter.WriteEndArray();
            bsonWriter.WriteEndDocument();

        }

        /*******************************************/

        public override IDictionary Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            bsonReader.ReadStartDocument();

            string text = bsonReader.ReadName();
            Type type = null;
            if (text == m_DiscriminatorConvention.ElementName)
            {
                string typeName = bsonReader.ReadString();
                type = BH.Engine.Base.Create.Type(typeName);
            }

            if (type == null)
                return null;

            IDictionary dic = Activator.CreateInstance(type) as IDictionary;
            if (dic == null)
                return null;

            bsonReader.ReadName();
            bsonReader.ReadStartArray();
            while (bsonReader.State != BsonReaderState.EndOfArray && bsonReader.ReadBsonType() != BsonType.EndOfDocument)
            {
                bsonReader.ReadStartDocument();
                bsonReader.ReadName("k");
                object key = BsonSerializer.Deserialize(bsonReader, typeof(object));
                bsonReader.ReadName("v");
                object value = BsonSerializer.Deserialize(bsonReader, typeof(object));
                bsonReader.ReadEndDocument();
                dic.Add(key, value);
            }
            bsonReader.ReadEndArray();

            string version = "";
            if (bsonReader.FindElement("_bhomVersion"))
                version = bsonReader.ReadString();

            bsonReader.ReadEndDocument();

            return dic;
        }


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private IDiscriminatorConvention m_DiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));

        /*******************************************/
    }
}




