using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson;

namespace BH.Engine.Serialiser.Conventions
{
    /// <summary>
    /// Maps a BHoM IImmutable type. 
    /// </summary>
    public class BHoMEnumConvention : ConventionBase, IMemberMapConvention
    {
        /*******************************************/
        /**** Interface Methods                 ****/
        /*******************************************/

        public void Apply(BsonMemberMap memberMap)
        {
            var memberType = memberMap.MemberType;
            var memberTypeInfo = memberType.GetTypeInfo();

            if (memberTypeInfo.IsEnum)
            {
                var serializer = memberMap.GetSerializer();
                var representationConfigurableSerializer = serializer as IRepresentationConfigurable;
                if (representationConfigurableSerializer != null)
                {
                    var reconfiguredSerializer = representationConfigurableSerializer.WithRepresentation(BsonType.String);
                    memberMap.SetSerializer(reconfiguredSerializer);
                }
                return;
            }

            if (IsNullableEnum(memberType))
            {
                var serializer = memberMap.GetSerializer();
                var childSerializerConfigurableSerializer = serializer as IChildSerializerConfigurable;
                if (childSerializerConfigurableSerializer != null)
                {
                    var childSerializer = childSerializerConfigurableSerializer.ChildSerializer;
                    var representationConfigurableChildSerializer = childSerializer as IRepresentationConfigurable;
                    if (representationConfigurableChildSerializer != null)
                    {
                        var reconfiguredChildSerializer = representationConfigurableChildSerializer.WithRepresentation(BsonType.String);
                        var reconfiguredSerializer = childSerializerConfigurableSerializer.WithChildSerializer(reconfiguredChildSerializer);
                        memberMap.SetSerializer(reconfiguredSerializer);
                    }
                }
                return;
            }
        }


        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private bool IsNullableEnum(Type type)
        {
            return
                type.GetTypeInfo().IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                Nullable.GetUnderlyingType(type).GetTypeInfo().IsEnum;
        }

        /*******************************************/
    }
}
