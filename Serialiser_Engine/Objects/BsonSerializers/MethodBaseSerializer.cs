using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class MethodBaseSerializer : SerializerBase<MethodBase>
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, MethodBase value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteStartDocument();

            bsonWriter.WriteName("TypeName");
            bsonWriter.WriteString(value.DeclaringType.AssemblyQualifiedName);

            bsonWriter.WriteName("MethodName");
            bsonWriter.WriteString(value.Name);

            ParameterInfo[] parameters = value.GetParameters();
            bsonWriter.WriteName("Parameters");
            bsonWriter.WriteStartArray();
            foreach (ParameterInfo info in parameters)
                bsonWriter.WriteString(info.ParameterType.AssemblyQualifiedName);
            bsonWriter.WriteEndArray();

            bsonWriter.WriteEndDocument();
        }

        /*******************************************/

        public override MethodBase Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            bsonReader.ReadStartDocument();

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
                MethodBase method = RestoreMethod(Type.GetType(typeName), methodName, paramTypes.Select(x => Type.GetType(x)).ToList());
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

        private static MethodBase RestoreMethod(Type type, string methodName, List<Type> paramTypes)
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
                        if (method.ContainsGenericParameters && method is MethodInfo)
                        {
                            Type[] generics = method.GetGenericArguments().Select(x => GetTypeFromGenericParameters(x)).ToArray();
                            method = ((MethodInfo)method).MakeGenericMethod(generics);
                            parameters = method.GetParameters();
                        }

                        bool matching = true;
                        for (int i = 0; i < paramTypes.Count; i++)
                        {
                            matching &= (paramTypes[i] == null || parameters[i].ParameterType == paramTypes[i]);
                        }
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
    }
}
