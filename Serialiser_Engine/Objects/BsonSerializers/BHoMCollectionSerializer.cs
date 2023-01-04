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
    public class BHoMCollectionSerializer<T, Q> : SerializerBase<T>, IBsonPolymorphicSerializer where T: ICollection<Q>
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            var bsonWriter = context.Writer;

            if (value == null)
            {
                bsonWriter.WriteNull();
                return;
            }

            if (bsonWriter.SerializationDepth == 0)
            {
                bsonWriter.WriteStartDocument();

                var discriminator = m_DiscriminatorConvention.GetDiscriminator(typeof(object), value.GetType());
                bsonWriter.WriteName(m_DiscriminatorConvention.ElementName);
                BsonValueSerializer.Instance.Serialize(context, discriminator);

                bsonWriter.WriteName("_Items");
            }
            
            bsonWriter.WriteStartArray();
            foreach (object item in value)
                BsonSerializer.Serialize(bsonWriter, item);
            bsonWriter.WriteEndArray();


            if (bsonWriter.SerializationDepth == 0)
            {
                bsonWriter.AddVersion();
                bsonWriter.WriteEndDocument();
            }

        }

        /*******************************************/

        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            BsonType bsonType = bsonReader.GetCurrentBsonType();

            if (bsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return default(T);
            }

            // Get type name
            string typeName = "";
            if (bsonType == BsonType.Document)
            {
                bsonReader.ReadStartDocument();
                string text = bsonReader.ReadName();
                if (text == m_DiscriminatorConvention.ElementName)
                    typeName = bsonReader.ReadString();

                bsonReader.ReadName();
            }

            // Get items
            List<object> items = new List<object>();
            bsonReader.ReadStartArray();
            while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                items.Add(BsonSerializer.Deserialize(bsonReader, typeof(object)));
            bsonReader.ReadEndArray();

            // Get version
            string version = "";
            if (bsonType == BsonType.Document)
            {
                if (bsonReader.FindElement("_bhomVersion"))
                    version = bsonReader.ReadString();

                context.Reader.ReadEndDocument();
            }

            T result = Activator.CreateInstance<T>();
            foreach (Q item in items.OfType<Q>())
                result.Add(item);

            return result;
        }


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private IDiscriminatorConvention m_DiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));

        /*******************************************/
    }
}




