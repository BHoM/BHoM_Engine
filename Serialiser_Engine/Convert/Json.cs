using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Linq;

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
            BsonValue document;
            try
            {
                document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonValue>(json);
            }
            catch
            {
                Reflection.Compute.RecordError("the string provided is not a valid json format.");
                return null;
            }

            if (document.IsBsonDocument)
                return Convert.FromBson(document.AsBsonDocument);

            else if (document.IsBsonArray)
                return document.AsBsonArray.ToArray().Select(b => Convert.FromBson(b.AsBsonDocument));

            else
                Reflection.Compute.RecordError("The string provided is not a supported json format");
                return null;
        }

        /*******************************************/
    }
}
