using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHE = Engine_Explore.Engine;


namespace Engine_Explore.Engine.Convert
{
    public static class Bson
    {
        public static BsonDocument Write(object obj, string key, DateTime timestamp)
        {
            BsonDocument document = obj.ToBsonDocument();

            /*if (obj is string) return Write(obj as string, key, timestamp);
            var document = BsonDocument.Parse(BHE.Convert.Json.Write(obj));*/

            if (key != "")
            {
                document["__Key__"] = key;
                document["__Time__"] = timestamp;
            }

            return document;
        }

        /*******************************************/

        /*public static BsonDocument Write(string obj, string key, DateTime timestamp)
        {
            var document = BsonDocument.Parse(obj);
            if (key != "")
            {
                document["__Key__"] = key;
                document["__Time__"] = timestamp;
            }

            return document;
        }*/

        /*******************************************/

        /*public static BsonDocument Write<T>(T obj) where T: BHoM.Base.BHoMObject
        {
            return new BsonDocument();
        }*/

        /*******************************************/

        public static object ReadObject(BsonDocument bson)
        {
            return BsonSerializer.Deserialize(bson, typeof(object));

            /*MongoDB.Bson.IO.JsonWriterSettings writerSettings = new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict };
            return BHE.Convert.Json.ReadObject(bson.ToJson(writerSettings));*/
        }
    }
}
