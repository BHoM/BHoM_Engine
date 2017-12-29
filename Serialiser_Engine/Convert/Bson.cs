using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using MongoDB.Bson.Serialization.Conventions;
using BH.Engine.Serialiser.BsonSerializers;

namespace BH.Engine.Serialiser
{
    public static partial class Convert
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static BsonDocument ToBson(this object obj)
        {
            if (obj is string)
            {
                BsonDocument document;
                BsonDocument.TryParse(obj as string, out document);
                return document;
            }
            else
                return obj.ToBsonDocument();
        }

        /*******************************************/

        public static object FromBson(BsonDocument bson)
        {
            if (!m_TypesRegistered)
                RegisterTypes();

            bson.Remove("_id");
            object obj = BsonSerializer.Deserialize(bson, typeof(object));
            if (obj is ExpandoObject)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>(obj as ExpandoObject);
                CustomObject co = new CustomObject();
                if (dic.ContainsKey("Name"))
                {
                    co.Name = dic["Name"] as string;
                    dic.Remove("Name");
                }   
                if (dic.ContainsKey("Tags"))
                {
                    co.Tags = new HashSet<string>(((List<object>)dic["Tags"]).Cast<string>());
                    dic.Remove("Tags");
                }
                if (dic.ContainsKey("BHoM_Guid"))
                {
                    co.BHoM_Guid = (Guid)dic["BHoM_Guid"];
                    dic.Remove("BHoM_Guid");
                }
                co.CustomData = dic;
                return co;
            }
            else
                return obj;
        }


        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        static Convert()
        {
            RegisterTypes();
        }

        /*******************************************/

        private static void RegisterTypes()
        {
            // Define the conventions   //TODO: Try to use conventions to better integrate with Bson
            //var pack = new ConventionPack();
            //pack.Add(new KeepPropertiesAtTopLevelConvention());
            //ConventionRegistry.Register("BHoM Conventions", pack, x => x is object);

            // Register the types
            try
            {
                BsonSerializer.RegisterSerializer(typeof(System.Drawing.Color), new ColourSerializer());
                BsonSerializer.RegisterSerializer(typeof(CustomObject), new CustomObjectSerializer());
                BsonSerializer.RegisterSerializer(typeof(object), new BH_ObjectSerializer());
            }
            catch(Exception)
            {
                Console.WriteLine("Problem with initialisation of the Bson Serializer");
            }

            foreach (Type type in BH.Engine.Reflection.Query.BHoMTypeList())
            {
                if (!type.IsGenericType && !BsonClassMap.IsClassMapRegistered(type))
                    RegisterClassMap(type);
            }
            RegisterClassMap(typeof(System.Drawing.Color));

            m_TypesRegistered = true;
        }


        /*******************************************/

        private static void RegisterClassMap(Type type)
        {
            BsonClassMap cm = new BsonClassMap(type);
            cm.AutoMap();
            cm.SetDiscriminator(type.ToString());
            cm.SetDiscriminatorIsRequired(true);
            BsonClassMap.RegisterClassMap(cm);
        }


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private static bool m_TypesRegistered = false;


        /*******************************************/
    }
}
