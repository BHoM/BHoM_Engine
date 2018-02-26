using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;

namespace BH.Engine.Serialiser.Conventions
{
    /// <summary>
    /// Maps a BHoM IImmutable type. 
    /// </summary>
    public class BHoMDictionaryConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            Type memberType = memberMap.MemberType;
            TypeInfo typeInfo = memberType.GetTypeInfo();

            if (typeInfo.Name == "Dictionary`2")
            {
                Type keyType = typeInfo.GenericTypeArguments[0];

                DictionaryRepresentation representation = DictionaryRepresentation.Document;
                if (keyType != typeof(string))
                    representation = DictionaryRepresentation.ArrayOfDocuments;

                var serializer = memberMap.GetSerializer();
                var dictionaryRepresentationConfigurable = serializer as IDictionaryRepresentationConfigurable;
                if (dictionaryRepresentationConfigurable != null && dictionaryRepresentationConfigurable.DictionaryRepresentation != representation)
                {
                    var reconfiguredSerializer = dictionaryRepresentationConfigurable.WithDictionaryRepresentation(representation);
                    memberMap.SetSerializer(reconfiguredSerializer);
                }
                return;
            }
        }
    }
}
