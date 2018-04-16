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
                //Using context.Writer.WriteNull() leads to problem in the deserialisation. 
                //We think that BSON think that the types will always be types to be deserialised rather than properties of objects.
                //If that type is null bson throws an exception believing that it wont be able to deserialise an object of type null, while for this case it is ment to be used as a property.
                context.Writer.WriteString("");
            }
            else
            {
                context.Writer.WriteString(value.FullName);
            }
        }
    }
}
