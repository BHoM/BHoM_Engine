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
                BsonDocument document;
                if (BsonDocument.TryParse(json, out document))
                {
                    return Convert.FromBson(document);
                }
                else
                {
                    Reflection.Compute.RecordError("The string provided is not a supported json format");
                    return null;
                }
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
                // Could we do something when a string is not a valid json?
                Reflection.Compute.RecordError("The string provided is not a valid json format");
                return null;
            }
        }

        /*******************************************/
    }
}
