using BH.oM.Base;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections.Generic;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class CustomObjectSerializer : SerializerBase<CustomObject>
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CustomObject value)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(value.CustomData);
            data["BHoM_Guid"] = value.BHoM_Guid;

            context.Writer.WriteStartDocument();

            if (value.Tags.Count > 0)
            {
                context.Writer.WriteName("Name");
                BsonSerializer.Serialize(context.Writer, value.Name);
            }

            if (value.Name.Length > 0)
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
    }
}
