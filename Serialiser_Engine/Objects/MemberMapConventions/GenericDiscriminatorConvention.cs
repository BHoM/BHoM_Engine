using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BH.Engine.Serialiser.Objects.MemberMapConventions
{
    public class GenericDiscriminatorConvention : IDiscriminatorConvention
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string ElementName { get { return "_t"; } }


        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            // the BsonReader is sitting at the value whose actual type needs to be found
            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType == BsonType.Document)
            {
                // ensure KnownTypes of nominalType are registered (so IsTypeDiscriminated returns correct answer)
                //BsonSerializer.EnsureKnownTypesAreRegistered(nominalType);

                // we can skip looking for a discriminator if nominalType has no discriminated sub types
                if (BsonSerializer.IsTypeDiscriminated(nominalType))
                {
                    var bookmark = bsonReader.GetBookmark();
                    bsonReader.ReadStartDocument();
                    var actualType = nominalType;
                    if (bsonReader.FindElement(ElementName))
                    {
                        var context = BsonDeserializationContext.CreateRoot(bsonReader);
                        var discriminator = BsonValueSerializer.Instance.Deserialize(context);
                        if (discriminator.IsBsonArray)
                        {
                            discriminator = discriminator.AsBsonArray.Last(); // last item is leaf class discriminator
                        }
                        actualType = BsonSerializer.LookupActualType(nominalType, discriminator);
                    }
                    bsonReader.ReturnToBookmark(bookmark);
                    return actualType;
                }
            }

            return nominalType;
        }

        /***************************************************/

        public BsonValue GetDiscriminator(Type nominalType, Type actualType)
        {
            if (actualType.IsGenericType)
                return actualType.AssemblyQualifiedName;
            else
                return actualType.FullName;
        }

        /***************************************************/
    }
}
