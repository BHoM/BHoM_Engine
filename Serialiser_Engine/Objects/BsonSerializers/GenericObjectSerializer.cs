using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Drawing;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class GenericObjectSerializer : SerializerBase<object>
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            context.Writer.WriteStartDocument();

            context.Writer.WriteEndDocument();
        }

        /*******************************************/

        public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            INameDecoder decoder = Utf8NameDecoder.Instance;

            context.Reader.ReadStartDocument();

            context.Reader.ReadEndDocument();

            return null;
        }

        /*******************************************/
    }
}
