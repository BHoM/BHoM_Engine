using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM_Engine.DataStream.Convert
{
    public static class Json
    {
        public static string Write(object obj)
        {
            return obj.ToJson();
        }

        /*******************************************/

        public static object Read(string json)
        {
            BsonDocument document;
            if (BsonDocument.TryParse(json, out document))
            {
                return Bson.Read(document);
            }
            else
            {
                return null;
            }
        }
    }
}
