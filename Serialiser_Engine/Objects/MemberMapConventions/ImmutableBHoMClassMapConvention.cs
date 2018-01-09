using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;

namespace BH.Engine.Serialiser.MemberMapConventions
{
    /// <summary>
    /// Maps a BHoM IImmutable type. 
    /// </summary>
    public class ImmutableBHoMClassMapConvention : ConventionBase, IClassMapConvention
    {
        public void Apply(BsonClassMap classMap)
        {
            var typeInfo = classMap.ClassType.GetTypeInfo();
            if (typeInfo.IsAbstract)
            {
                return;
            }

            if (typeInfo.GetConstructor(Type.EmptyTypes) != null)
            {
                return;
            }

            if (typeInfo.GetInterface("IImmutable") == null)
            {
                return; // only applies to classes that inherit from IImutable
            }

            var properties = typeInfo.GetProperties();

            var anyConstructorsWereMapped = false;
            ConstructorInfo[] constructors = typeInfo.GetConstructors();
            if (constructors.Length > 0)
            {
                var ctor = typeInfo.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();

                var parameters = ctor.GetParameters();

                var matches = parameters
                    .GroupJoin(properties,
                        parameter => parameter.Name,
                        property => property.Name,
                        (parameter, props) => new { Parameter = parameter, Properties = props },
                        StringComparer.OrdinalIgnoreCase);

                if (matches.Any(m => m.Properties.Count() != 1))
                {
                    //continue;
                }

                classMap.MapConstructor(ctor);

                anyConstructorsWereMapped = true;
            }

            if (anyConstructorsWereMapped)
            {
                var classType = classMap.ClassType;
                
                foreach (var property in properties)
                {
                    if (property.DeclaringType == classType)
                        classMap.MapMember(property);
                }
            }
        }
    }
}
