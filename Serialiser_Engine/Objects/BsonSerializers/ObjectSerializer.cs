using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Reflection;

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
            throw new FormatException($"ObjectSerializer does not support BSON type '{currentBsonType}'.");
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
            var serializer = BsonSerializer.LookupSerializer(actualType);

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
                            var discriminator = _discriminatorConvention.GetDiscriminator(typeof(object), actualType);
                            bsonWriter.WriteStartDocument();
                            bsonWriter.WriteName(_discriminatorConvention.ElementName);
                            BsonValueSerializer.Instance.Serialize(context, discriminator);
                            bsonWriter.WriteName("_v");
                            serializer.Serialize(context, value);
                            bsonWriter.WriteEndDocument();
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
            IBsonReader reader = context.Reader;
            Type actualType = _discriminatorConvention.GetActualType(reader, typeof(object));
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
            IBsonSerializer bsonSerializer = BsonSerializer.LookupSerializer(actualType);
            IBsonPolymorphicSerializer bsonPolymorphicSerializer = bsonSerializer as IBsonPolymorphicSerializer;
            if (bsonPolymorphicSerializer != null && bsonPolymorphicSerializer.IsDiscriminatorCompatibleWithObjectSerializer)
            {
                return bsonSerializer.Deserialize(context, args);
            }
            object result = null;
            bool flag = false;
            reader.ReadStartDocument();
            while (reader.ReadBsonType() != 0)
            {
                string text = reader.ReadName();
                if (text == _discriminatorConvention.ElementName)
                {
                    reader.SkipValue();
                }
                else
                {
                    if (!(text == "_v"))
                    {
                        throw new FormatException($"Unexpected element name: '{text}'.");
                    }
                    result = bsonSerializer.Deserialize(context);
                    flag = true;
                }
            }
            reader.ReadEndDocument();
            if (!flag)
            {
                throw new FormatException("_v element missing.");
            }
            return result;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private readonly IDiscriminatorConvention _discriminatorConvention;


        /*******************************************/
    }
}
