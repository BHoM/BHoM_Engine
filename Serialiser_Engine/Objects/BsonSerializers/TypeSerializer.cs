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

            bsonWriter.WriteName("Name");
            if (value == null)
            {
                //Using context.Writer.WriteNull() leads to problem in the deserialisation. 
                //We think that BSON think that the types will always be types to be deserialised rather than properties of objects.
                //If that type is null bson throws an exception believing that it wont be able to deserialise an object of type null, while for this case it is ment to be used as a property.
                bsonWriter.WriteString("");
            }
            else
            {
                bsonWriter.WriteString(value.FullName);
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

            context.Reader.ReadEndDocument();

            try
            {
                if (string.IsNullOrEmpty(fullName))
                    return null;

                Type type = Reflection.Create.Type(fullName);
                if (type == null)
                    Reflection.Compute.RecordError("Type " + fullName + " failed to deserialise.");
                return type;
            }
            catch
            {
                Reflection.Compute.RecordError("Type " + fullName + " failed to deserialise.");
                return null;
            }
        }


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private IDiscriminatorConvention m_DiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));

        /*******************************************/
    }
}
