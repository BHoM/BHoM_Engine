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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class TypeSerializer : SerializerBase<Type>, IBsonPolymorphicSerializer
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Type value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteStartDocument();

            var discriminator = m_DiscriminatorConvention.GetDiscriminator(typeof(object), typeof(Type));
            bsonWriter.WriteName(m_DiscriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(context, discriminator);

            if (value == null)
            {
                //Using context.Writer.WriteNull() leads to problem in the deserialisation. 
                //We think that BSON think that the types will always be types to be deserialised rather than properties of objects.
                //If that type is null bson throws an exception believing that it wont be able to deserialise an object of type null, while for this case it is ment to be used as a property.
                bsonWriter.WriteName("Name");
                bsonWriter.WriteString("");
            }
            else
            {
                // Handle the case of generic types
                Type[] generics = new Type[] { };
                if (value.IsGenericType)
                {
                    generics = value.GetGenericArguments();
                    value = value.GetGenericTypeDefinition();
                }

                // Write the name of the type
                bsonWriter.WriteName("Name");
                if (value.IsGenericParameter)
                {
                    bsonWriter.WriteString("T");

                    Type[] constraints = value.GetGenericParameterConstraints();
                    if (constraints.Length > 0)
                    {
                        bsonWriter.WriteName("Constraints");
                        bsonWriter.WriteStartArray();
                        foreach (Type constraint in constraints)
                            BsonSerializer.Serialize(bsonWriter, constraint);
                        bsonWriter.WriteEndArray();
                    }
                }
                else if (value.Namespace.StartsWith("BH.oM"))
                    bsonWriter.WriteString(value.FullName);
                else if (value.AssemblyQualifiedName != null)
                    bsonWriter.WriteString(value.AssemblyQualifiedName);
                else
                    bsonWriter.WriteString(""); //TODO: is that even possible?


                // Add additional information for generic types
                if (generics.Length > 0)
                {
                    bsonWriter.WriteName("GenericArguments");
                    bsonWriter.WriteStartArray();
                    foreach (Type arg in generics)
                        BsonSerializer.Serialize(bsonWriter, arg);
                    bsonWriter.WriteEndArray();
                }
            }

            bsonWriter.WriteEndDocument();
        }

        /*******************************************/

        public override Type Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            bsonReader.ReadStartDocument();

            string text = bsonReader.ReadName();
            if (text == m_DiscriminatorConvention.ElementName)
                bsonReader.SkipValue();

            bsonReader.ReadName();
            var fullName = context.Reader.ReadString();

            List<Type> genericTypes = new List<Type>();
            if (context.Reader.FindElement("GenericArguments"))
            {
                bsonReader.ReadStartArray();
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                    genericTypes.Add(BsonSerializer.Deserialize(bsonReader, typeof(Type)) as Type);
                bsonReader.ReadEndArray();
            }

            context.Reader.ReadEndDocument();

            try
            {
                Type type = GetTypeFromName(fullName);

                if (type == null)
                {
                    // Try to upgrade through versioning
                    BsonDocument doc = new BsonDocument();
                    doc["_t"] = "System.Type";
                    doc["Name"] = fullName;
                    BsonDocument newDoc = Versioning.Convert.ToNewVersion(doc);
                    if (newDoc != null && newDoc.Contains("Name"))
                        type = GetTypeFromName(newDoc["Name"].AsString);
                }

                if (type == null)
                    Reflection.Compute.RecordError("Type " + fullName + " failed to deserialise.");
                else if (type.IsGenericType && type.GetGenericArguments().Length == genericTypes.Where(x => x != null).Count())
                    type = type.MakeGenericType(genericTypes.ToArray()); 

                return type;
            }
            catch
            {
                Reflection.Compute.RecordError("Type " + fullName + " failed to deserialise.");
                return null;
            }
        }

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private Type GetTypeFromName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            Type type = null;
            if (fullName == "T")
                return null;
            if (fullName.StartsWith("BH.oM"))
                type = Reflection.Create.Type(fullName, true);
            else if (fullName.StartsWith("BH.Engine"))
                type = Reflection.Create.EngineType(fullName, true);
            else
                type = Type.GetType(fullName);

            if (type == null)
            {
                List<Type> types = Reflection.Create.AllTypes(fullName, true);
                if (types.Count > 0)
                    type = types.First();
            }

            return type;
        }

        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private IDiscriminatorConvention m_DiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));

        /*******************************************/
    }
}

