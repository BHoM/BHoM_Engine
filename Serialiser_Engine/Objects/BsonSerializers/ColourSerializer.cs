using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Drawing;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class ColourSerializer : SerializerBase<Color>
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Color value)
        {
            context.Writer.WriteStartDocument();

            context.Writer.WriteName("A");
            context.Writer.WriteInt32(value.A);

            context.Writer.WriteName("R");
            context.Writer.WriteInt32(value.R);

            context.Writer.WriteName("G");
            context.Writer.WriteInt32(value.G);

            context.Writer.WriteName("B");
            context.Writer.WriteInt32(value.B);

            context.Writer.WriteEndDocument();
        }

        /*******************************************/

        public override Color Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            INameDecoder decoder = Utf8NameDecoder.Instance;

            context.Reader.ReadStartDocument();

            context.Reader.ReadName(decoder);
            int a = context.Reader.ReadInt32();

            context.Reader.ReadName(decoder);
            int r = context.Reader.ReadInt32();

            context.Reader.ReadName(decoder);
            int g = context.Reader.ReadInt32();

            context.Reader.ReadName(decoder);
            int b = context.Reader.ReadInt32();

            context.Reader.ReadEndDocument();

            return Color.FromArgb(a, r, g, b);
        }

        /*******************************************/
    }
}
