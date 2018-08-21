using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Linq;
using System.Collections.Generic;

namespace BH.Engine.Serialiser
{
    public static partial class Convert
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static string ToJson(this object obj)
        {
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return Convert.ToBson(obj).ToJson<BsonDocument>(jsonWriterSettings);  
        }

        /*******************************************/

        public static object FromJson(string json)
        {
            if (json == "")
            {
                return null;
            }
            else if (json.StartsWith("{"))
            {
                return Convert.FromBson(BsonDocument.Parse(json));
            }
            else if (json.StartsWith("["))
            {
                Reflection.Compute.RecordNote("The string provided represents a top-level Json Array instead of a Json object.\nDeserializing to a CustomObject.");
                BsonArray array = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(json);

                return new BH.oM.Base.CustomObject
                {
                    CustomData = new Dictionary<string, object>()
                    {
                        { "Objects", array.Select(b => Convert.FromBson(b.AsBsonDocument)) }
                    }
                };
            }
            else
            {
                Reflection.Compute.RecordError("The string provided is not a valid json format");
                return null;
            }
        }

        /*******************************************/
    }
}
