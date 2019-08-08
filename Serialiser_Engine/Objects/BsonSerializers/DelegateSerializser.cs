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
    public class DelegateSerializer : SerializerBase<Delegate>, IBsonPolymorphicSerializer
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Delegate value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteStartDocument();

            var discriminator = m_DiscriminatorConvention.GetDiscriminator(typeof(object), typeof(Delegate));
            bsonWriter.WriteName(m_DiscriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(context, discriminator);

            bsonWriter.WriteName("Method");
            bsonWriter.WriteString(value.Method.ToJson());

            bsonWriter.WriteName("Target");
                bsonWriter.WriteString(value.Target?.ToJson() ?? "");

            bsonWriter.WriteEndDocument();
        }

        /*******************************************/

        public override Delegate Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            bsonReader.ReadStartDocument();

            string text = bsonReader.ReadName();
            if (text == m_DiscriminatorConvention.ElementName)
                bsonReader.SkipValue();

            bsonReader.ReadName();
            string methodName = bsonReader.ReadString();

            bsonReader.ReadName();
            string targetName = bsonReader.ReadString();

            context.Reader.ReadEndDocument();

            try
            {
                Delegate @delegate = Create.Delegate((MethodInfo)Convert.FromJson(methodName), (Type)Convert.FromJson(targetName));
                if (@delegate == null)
                    Reflection.Compute.RecordError("Delegate " + methodName + " with target " + targetName + " failed to deserialise.");
                return @delegate;
            }
            catch
            {
                Reflection.Compute.RecordError("Delegate " + methodName + " with target " + targetName + " failed to deserialise.");
                return null;
            }
        }

        /*******************************************/

        private IDiscriminatorConvention m_DiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
    }
}
