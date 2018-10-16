using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class MethodBaseSerializer : SerializerBase<MethodBase>, IBsonPolymorphicSerializer
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, MethodBase value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteStartDocument();

            var discriminator = m_DiscriminatorConvention.GetDiscriminator(typeof(object), typeof(MethodBase));
            bsonWriter.WriteName(m_DiscriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(context, discriminator);

            bsonWriter.WriteName("TypeName");
            bsonWriter.WriteString(value.DeclaringType.ToJson());

            bsonWriter.WriteName("MethodName");
            bsonWriter.WriteString(value.Name);

            ParameterInfo[] parameters = value.GetParameters();
            bsonWriter.WriteName("Parameters");
            bsonWriter.WriteStartArray();
            foreach (ParameterInfo info in parameters)
                bsonWriter.WriteString(info.ParameterType.ToJson());
            bsonWriter.WriteEndArray();

            bsonWriter.WriteEndDocument();
        }

        /*******************************************/

        public override MethodBase Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            bsonReader.ReadStartDocument();

            string text = bsonReader.ReadName();
            if (text == m_DiscriminatorConvention.ElementName)
                bsonReader.SkipValue();

            bsonReader.ReadName();
            string typeName = bsonReader.ReadString();

            bsonReader.ReadName();
            string methodName = bsonReader.ReadString();

            List<string> paramTypes = new List<string>();
            bsonReader.ReadName();
            bsonReader.ReadStartArray();
            while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                paramTypes.Add(bsonReader.ReadString());
            bsonReader.ReadEndArray();

            context.Reader.ReadEndDocument();

            try
            {
                MethodBase method = RestoreMethod((Type)Convert.FromJson(typeName), methodName, paramTypes/*.Select(x => (Type)Convert.FromJson(x)).ToList()*/);
                if (method == null)
                    Reflection.Compute.RecordError("Method " + methodName + " from " + typeName + " failed to deserialise.");
                return method;
            }
            catch
            {
                Reflection.Compute.RecordError("Method " + methodName + " from " + typeName + " failed to deserialise.");
                return null;
            }
        }


        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static MethodBase RestoreMethod(Type type, string methodName, List<string> paramTypes)
        {
            List<MethodBase> methods;
            if (methodName == ".ctor")
                methods = type.GetConstructors().ToList<MethodBase>();
            else
                methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).ToList<MethodBase>();

            for (int k = 0; k < methods.Count; k++)
            {
                MethodBase method = methods[k];

                if (method.Name == methodName)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == paramTypes.Count)
                    {
                        /*if (method.ContainsGenericParameters && method is MethodInfo)
                        {
                            Type[] generics = method.GetGenericArguments().Select(x => GetTypeFromGenericParameters(x)).ToArray();
                            method = ((MethodInfo)method).MakeGenericMethod(generics);
                            parameters = method.GetParameters();
                        }

                        bool matching = true;
                        for (int i = 0; i < paramTypes.Count; i++)
                        {
                            matching &= (paramTypes[i] == null || parameters[i].ParameterType == paramTypes[i]);
                        }*/

                        bool matching = true;
                        List<string> names = parameters.Select(x => Convert.ToJson(x.ParameterType)).ToList();
                        for (int i = 0; i < paramTypes.Count; i++)
                            matching &= names[i] == paramTypes[i];

                        if (matching)
                        {
                            return method;
                        }
                    }
                }
            }

            return null;
        }

        /*******************************************/

        private static Type GetTypeFromGenericParameters(Type type)
        {
            Type[] constrains = type.GetGenericParameterConstraints();
            if (constrains.Length == 0)
                return typeof(object);
            else
                return constrains[0];
        }

        /*******************************************/

        private IDiscriminatorConvention m_DiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
    }
}
