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

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class EnumSerializer : SerializerBase<Enum>, IBsonPolymorphicSerializer
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Enum value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteStartDocument();

            var discriminator = m_DiscriminatorConvention.GetDiscriminator(typeof(object), typeof(Enum));
            bsonWriter.WriteName(m_DiscriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(context, discriminator);

            bsonWriter.WriteName("TypeName");
            BsonSerializer.Serialize(bsonWriter, typeof(Type), value.GetType());

            bsonWriter.WriteName("Value");
            bsonWriter.WriteString(Enum.GetName(value.GetType(), value)); 

            bsonWriter.WriteEndDocument();
        }

        /*******************************************/

        public override Enum Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            bsonReader.ReadStartDocument();

            string text = bsonReader.ReadName();
            if (text == m_DiscriminatorConvention.ElementName)
                bsonReader.SkipValue();

            bsonReader.ReadName();
            Type type = BsonSerializer.Deserialize(bsonReader, typeof(Type)) as Type;

            bsonReader.ReadName();
            string valueName = bsonReader.ReadString();

            string version = "";
            if (bsonReader.FindElement("_bhomVersion"))
                version = bsonReader.ReadString();

            context.Reader.ReadEndDocument();

            try
            {
                Enum value = Enum.Parse(type, valueName) as Enum;
                return value;
            }
            catch
            {
                Base.Compute.RecordError("Enum " + valueName + " failed to deserialise.");
                return null;
            }
        }


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private IDiscriminatorConvention m_DiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
    }
}




