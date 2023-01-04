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
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using BH.Engine.Versioning;
using System;
using System.Reflection;
using BH.Engine.Serialiser.Objects;
using System.Collections.Generic;
using BH.Engine.Serialiser.Objects.MemberMapConventions;
using System.Linq;
using System.Collections;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class BH_ObjectSerializer : MongoDB.Bson.Serialization.Serializers.ObjectSerializer
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public BH_ObjectSerializer()
        {
            _discriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            var bsonWriter = context.Writer;

            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                var actualType = value.GetType();
                if (actualType == typeof(object))
                {
                    bsonWriter.WriteStartDocument();
                    bsonWriter.WriteEndDocument();
                }
                else
                {
                    // certain types can be written directly as BSON value
                    // if we're not at the top level document, or if we're using the JsonWriter
                    if (bsonWriter.State == BsonWriterState.Value || bsonWriter is JsonWriter)
                    {
                        switch (Type.GetTypeCode(actualType))
                        {
                            case TypeCode.Boolean:
                                bsonWriter.WriteBoolean((bool)value);
                                return;

                            case TypeCode.DateTime:
                                // TODO: is this right? will lose precision after round trip
                                var bsonDateTime = new BsonDateTime(BsonUtils.ToUniversalTime((DateTime)value));
                                bsonWriter.WriteDateTime(bsonDateTime.MillisecondsSinceEpoch);
                                return;

                            case TypeCode.Double:
                                bsonWriter.WriteDouble((double)value);
                                return;

                            case TypeCode.Int16:
                                // TODO: is this right? will change type to Int32 after round trip
                                bsonWriter.WriteInt32((short)value);
                                return;

                            case TypeCode.Int32:
                                bsonWriter.WriteInt32((int)value);
                                return;

                            case TypeCode.Int64:
                                bsonWriter.WriteInt64((long)value);
                                return;

                            case TypeCode.Object:
                                if (actualType == typeof(Decimal128))
                                {
                                    var decimal128 = (Decimal128)value;
                                    bsonWriter.WriteDecimal128(decimal128);
                                    return;
                                }
                                if (actualType == typeof(Guid))
                                {
                                    var guid = (Guid)value;
                                    var guidRepresentation = bsonWriter.Settings.GuidRepresentation;
                                    var binaryData = new BsonBinaryData(guid, guidRepresentation);
                                    bsonWriter.WriteBinaryData(binaryData);
                                    return;
                                }
                                if (actualType == typeof(ObjectId))
                                {
                                    bsonWriter.WriteObjectId((ObjectId)value);
                                    return;
                                }
                                break;

                            case TypeCode.String:
                                bsonWriter.WriteString((string)value);
                                return;
                        }
                    }

                    SerializeDiscriminatedValue(context, args, value, actualType);
                }
            }
        }

        /***************************************************/

        public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            IBsonReader reader = context.Reader;
            BsonType currentBsonType = reader.GetCurrentBsonType();
            switch (currentBsonType)
            {
                case BsonType.Array:
                    if (context.DynamicArraySerializer != null)
                    {
                        return context.DynamicArraySerializer.Deserialize(context);
                    }
                    break;
                case BsonType.Binary:
                    {
                        BsonBinaryData bsonBinaryData = reader.ReadBinaryData();
                        BsonBinarySubType subType = bsonBinaryData.SubType;
                        if (subType == BsonBinarySubType.UuidStandard || subType == BsonBinarySubType.UuidLegacy)
                        {
                            return bsonBinaryData.ToGuid();
                        }
                        break;
                    }
                case BsonType.Boolean:
                    return reader.ReadBoolean();
                case BsonType.DateTime:
                    return new BsonDateTime(reader.ReadDateTime()).ToUniversalTime();
                case BsonType.Decimal128:
                    return reader.ReadDecimal128();
                case BsonType.Document:
                    return DeserializeDiscriminatedValue(context, args);
                case BsonType.Double:
                    return reader.ReadDouble();
                case BsonType.Int32:
                    return reader.ReadInt32();
                case BsonType.Int64:
                    return reader.ReadInt64();
                case BsonType.Null:
                    reader.ReadNull();
                    return null;
                case BsonType.ObjectId:
                    return reader.ReadObjectId();
                case BsonType.String:
                    return reader.ReadString();
            }

            Engine.Base.Compute.RecordError($"ObjectSerializer does not support BSON type '{currentBsonType}'.");
            return null;
        }


        /***************************************************/
        /**** Private Helper Methods                    ****/
        /***************************************************/

        private void SerializeDiscriminatedValue(BsonSerializationContext context, BsonSerializationArgs args, object value, Type actualType)
        {
            if (actualType.Name == "RuntimeMethodInfo" || actualType.Name == "RuntimeConstructorInfo")
                actualType = typeof(MethodBase);
            else if (actualType.Name == "RuntimeType")
                actualType = typeof(Type);
            else if (value is Enum)
                actualType = typeof(Enum);

            if (!BsonClassMap.IsClassMapRegistered(actualType))
                Compute.RegisterClassMap(actualType);

            var serializer = BsonSerializer.LookupSerializer(actualType);
            if (serializer.GetType().Name == "EnumerableInterfaceImplementerSerializer`2" && context.Writer.State == BsonWriterState.Initial)
            {
                if (!m_FallbackSerialisers.ContainsKey(actualType))
                    CreateFallbackSerialiser(actualType);
                serializer = m_FallbackSerialisers[actualType];
            }

            var polymorphicSerializer = serializer as IBsonPolymorphicSerializer;
            if (polymorphicSerializer != null && polymorphicSerializer.IsDiscriminatorCompatibleWithObjectSerializer)
            {
                serializer.Serialize(context, args, value);
            }
            else
            {
                if (context.IsDynamicType != null && context.IsDynamicType(value.GetType()))
                {
                    args.NominalType = actualType;
                    serializer.Serialize(context, args, value);
                }
                else
                {
                    var bsonWriter = context.Writer;
                    if (actualType.Name == "Dictionary`2")
                    {
                        Type keyType = actualType.GenericTypeArguments[0];
                        if (keyType == typeof(string))
                            serializer.Serialize(context, value);
                        else
                        {
                            DictionarySerializer dicSerialiser = new DictionarySerializer();
                            dicSerialiser.Serialize(context, value);
                        }
                    }
                    else
                    {
                        serializer.Serialize(context, value);
                    }
                }
            }
        }

        /***************************************************/

        private object DeserializeDiscriminatedValue(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            // First try to recover the object type
            IBsonReader reader = context.Reader;
            BsonReaderBookmark bookmark = reader.GetBookmark();
            Type actualType = typeof(CustomObject);
            try
            {
                actualType = _discriminatorConvention.GetActualType(reader, typeof(object));
            }
            catch 
            {
                try
                {
                    context.Reader.ReturnToBookmark(bookmark);
                    DeprecatedSerializer deprecatedSerialiser = new DeprecatedSerializer();
                    return deprecatedSerialiser.Deserialize(context, args);
                }
                catch { }
            }
            context.Reader.ReturnToBookmark(bookmark);

            if (actualType == null)
                return null;

            // Handle the special case where the type is object
            if (actualType == typeof(object))
            {
                BsonType currentBsonType = reader.GetCurrentBsonType();
                if (currentBsonType == BsonType.Document && context.DynamicDocumentSerializer != null)
                {
                    return context.DynamicDocumentSerializer.Deserialize(context, args);
                }
                reader.ReadStartDocument();
                reader.ReadEndDocument();
                return new object();
            }

            // Handle the general case of finding the correct deserialiser and calling it
            try
            {
                if (!BsonClassMap.IsClassMapRegistered(actualType))
                    Compute.RegisterClassMap(actualType); // LookupSerializer creates the classMap if it doesn't exist so important to do it through our own method

                IBsonSerializer bsonSerializer = BsonSerializer.LookupSerializer(actualType);

                if (bsonSerializer.GetType().Name == "EnumerableInterfaceImplementerSerializer`2" && context.Reader.CurrentBsonType == BsonType.Document)
                {
                    if (!m_FallbackSerialisers.ContainsKey(actualType))
                        CreateFallbackSerialiser(actualType);
                    bsonSerializer = m_FallbackSerialisers[actualType];
                }
                else if (actualType.Name == "Dictionary`2" && context.Reader.CurrentBsonType == BsonType.Document)
                {
                    DictionarySerializer dicSerialiser = new DictionarySerializer();
                    return dicSerialiser.Deserialize(context, args);
                }

                return bsonSerializer.Deserialize(context, args);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Could not load file or assembly"))
                    Engine.Base.Compute.RecordError(e.Message);

                context.Reader.ReturnToBookmark(bookmark);
                DeprecatedSerializer deprecatedSerialiser = new DeprecatedSerializer();
                return deprecatedSerialiser.Deserialize(context, args);
            }
        }

        /***************************************************/

        private void CreateFallbackSerialiser(Type actualType)
        {
            var classMap = BsonClassMap.LookupClassMap(actualType);
            var classMapSerializerDefinition = typeof(BsonClassMapSerializer<>);
            var classMapSerializerType = classMapSerializerDefinition.MakeGenericType(actualType);
            m_FallbackSerialisers[actualType] = (IBsonSerializer)Activator.CreateInstance(classMapSerializerType, classMap);
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private readonly IDiscriminatorConvention _discriminatorConvention;
        private Dictionary<Type, IBsonSerializer> m_FallbackSerialisers = new Dictionary<Type, IBsonSerializer>();


        /*******************************************/
    }
}




