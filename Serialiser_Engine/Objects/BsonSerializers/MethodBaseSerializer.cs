/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
    public class MethodBaseSerializer : SerializerBase<MethodBase>, IBsonPolymorphicSerializer
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, MethodBase value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteStartDocument();

            var discriminator = m_DiscriminatorConvention.GetDiscriminator(typeof(object), typeof(MethodBase));
            bsonWriter.WriteName(m_DiscriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(context, discriminator);

            bsonWriter.WriteName("TypeName");
            bsonWriter.WriteString(value.DeclaringType.ToJson());

            bsonWriter.WriteName("MethodName");
            bsonWriter.WriteString(value.Name);

            ParameterInfo[] parameters = value.ParametersWithConstraints();
            bsonWriter.WriteName("Parameters");
            bsonWriter.WriteStartArray();
            foreach (ParameterInfo info in parameters)
                bsonWriter.WriteString(info.ParameterType.ToJson());
            bsonWriter.WriteEndArray();

            bsonWriter.WriteEndDocument();
        }

        /*******************************************/

        public override MethodBase Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            bsonReader.ReadStartDocument();

            string text = bsonReader.ReadName();
            if (text == m_DiscriminatorConvention.ElementName)
                bsonReader.SkipValue();

            bsonReader.ReadName();
            string typeName = bsonReader.ReadString();

            bsonReader.ReadName();
            string methodName = bsonReader.ReadString();

            List<string> paramTypesJson = new List<string>();
            bsonReader.ReadName();
            bsonReader.ReadStartArray();
            while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                paramTypesJson.Add(bsonReader.ReadString());
            bsonReader.ReadEndArray();

            context.Reader.ReadEndDocument();

            try
            {
                MethodBase method = GetMethod(methodName, typeName, paramTypesJson);

                if (method == null || method.IsDeprecated())
                {
                    // Try to upgrade through versioning
                    BsonDocument doc = new BsonDocument();
                    doc["_t"] = "System.Reflection.MethodBase";
                    doc["TypeName"] = typeName;
                    doc["MethodName"] = methodName;
                    doc["Parameters"] = new BsonArray(paramTypesJson);
                    BsonDocument newDoc = Versioning.Convert.ToNewVersion(doc);
                    if (newDoc != null && newDoc.Contains("TypeName") && newDoc.Contains("MethodName") && newDoc.Contains("Parameters"))
                        method = GetMethod(newDoc["MethodName"].AsString, newDoc["TypeName"].AsString, newDoc["Parameters"].AsBsonArray.Select(x => x.AsString).ToList());
                }

                if (method == null)
                    Reflection.Compute.RecordError("Method " + methodName + " from " + typeName + " failed to deserialise.");
                return method;
            }
            catch
            {
                Reflection.Compute.RecordError("Method " + methodName + " from " + typeName + " failed to deserialise.");
                return null;
            }
        }

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private MethodBase GetMethod(string methodName, string typeName, List<string> paramTypesJson)
        {
            List<Type> types = paramTypesJson.Select(x => Convert.FromJson(x)).Cast<Type>().ToList();

            MethodBase method = null;
            BsonDocument typeDocument;
            if (BsonDocument.TryParse(typeName, out typeDocument) && typeDocument.Contains("Name"))
            {
                typeName = typeDocument["Name"].AsString;
                foreach (Type type in Reflection.Create.AllTypes(typeName, true))
                {
                    method = Create.MethodBase(type, methodName, types); // type overload
                    if (method != null)
                        return method;
                }
            }

            return method;
        }



        /*******************************************/

        private IDiscriminatorConvention m_DiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
    }
}

