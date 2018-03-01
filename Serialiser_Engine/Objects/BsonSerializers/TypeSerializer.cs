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
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                return null;
            }
            else
            {
                var fullName = context.Reader.ReadString();

                List<Type> types;
                if (Reflection.Query.BHoMTypeDictionary().TryGetValue(fullName, out types))
                {
                    if (types.Count > 0)
                        return types.First();
                }

                return Type.GetType(fullName);
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Type value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(value.FullName);
            }
        }
    }
}
