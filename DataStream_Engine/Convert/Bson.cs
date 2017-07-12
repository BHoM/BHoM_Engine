using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BHoM_Engine.DataStream.Convert
{
    public static class Bson
    {
        public static BsonDocument Write(object obj)
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

        public static object Read(BsonDocument bson)
        {
            bson.Remove("_id");
            return BsonSerializer.Deserialize(bson, typeof(object));
        }
    }
}
