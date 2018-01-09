using BH.oM.Base;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections.Generic;
using System;
using System.Dynamic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class CustomObjectSerializer : SerializerBase<CustomObject>, IBsonPolymorphicSerializer
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CustomObject value)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(value.CustomData);
            data["BHoM_Guid"] = value.BHoM_Guid;

            context.Writer.WriteStartDocument();

            if (value.Name.Length > 0)
            {
                context.Writer.WriteName("Name");
                BsonSerializer.Serialize(context.Writer, value.Name);
            }

            if (value.Tags.Count > 0)
            {
                context.Writer.WriteName("Tags");
                context.Writer.WriteStartArray();
                foreach (string tag in value.Tags)
                    context.Writer.WriteString(tag);
                context.Writer.WriteEndArray();
            }

            foreach (KeyValuePair<string, object> kvp in data)
            {
                context.Writer.WriteName(kvp.Key);
                BsonSerializer.Serialize(context.Writer, kvp.Value);
            }
            context.Writer.WriteEndDocument();
        }


        /*******************************************/

        public override CustomObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;

            var bsonType = bsonReader.GetCurrentBsonType();
            string message;
            switch (bsonType)
            {
                case BsonType.Document:
                    var dynamicContext = context.With(ConfigureDeserializationContext);
                    bsonReader.ReadStartDocument();
                    CustomObject document = new CustomObject();
                    Dictionary<string, object> dic = document.CustomData;
                    while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                    {
                        var name = bsonReader.ReadName();
                        var value = m_ObjectSerializer.Deserialize(dynamicContext);
                        switch (name)
                        {
                            case "Name":
                                document.Name = value as string;
                                break;
                            case "Tags":
                                document.Tags = new HashSet<string>(((List<object>)value).Cast<string>());
                                break;
                            case "BHoM_Guid":
                                document.BHoM_Guid = (Guid)value;
                                break;
                            default:
                                dic[name] = value;
                                break;
                        }
                    }
                    bsonReader.ReadEndDocument();
                    return document;

                default:
                    message = string.Format("Cannot deserialize a '{0}' from BsonType '{1}'.", BsonUtils.GetFriendlyTypeName(typeof(CustomObject)), bsonType);
                    throw new FormatException(message);
            }
        }

        /*******************************************/

        protected void ConfigureDeserializationContext(BsonDeserializationContext.Builder builder)
        {
            builder.DynamicDocumentSerializer = this;
            builder.DynamicArraySerializer = m_ListSerializer;
        }


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private readonly IBsonSerializer<List<object>> m_ListSerializer = BsonSerializer.LookupSerializer<List<object>>();
        private static readonly IBsonSerializer<object> m_ObjectSerializer = BsonSerializer.LookupSerializer<object>();


        /*******************************************/
    }
}
