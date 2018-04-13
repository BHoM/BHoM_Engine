using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class TypeSerializer : SerializerBase<Type>
    {
        public override Type Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var fullName = context.Reader.ReadString();

            if (string.IsNullOrEmpty(fullName))
                return null;
            else
                return Reflection.Create.Type(fullName);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Type value)
        {
            if (value == null)
            {
                context.Writer.WriteString("");
            }
            else
            {
                context.Writer.WriteString(value.FullName);
            }
        }
    }
}
