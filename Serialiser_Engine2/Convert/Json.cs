using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BH.Engine.SerialiserTesting
{
    public static partial class Convert
    {
        public static string ToJson2(this object obj)
        {
            /*var x = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });*/
            var s = JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects});// JsonSerializer.Serialize(obj);
            s = s.Replace("$type", "_t");
            return s;

        }

        public static object FromJson2(string json)
        {
            dynamic dyn = JsonConvert.DeserializeObject<dynamic>(json);

            if(dyn._t != null)
            {
                var type = BH.Engine.Base.Query.AllTypeList().Where(x => dyn._t.ToString().Contains(x.FullName)).FirstOrDefault();
                if (type != null)
                {
                    return JsonConvert.DeserializeObject(json, type);
                }
            }

            return null;
            /*dynamic dyn = JsonSerializer.Deserialize<dynamic>(json);

            if(dyn.T != null)
            {
                var type = BH.Engine.Base.Query.AllTypeList().Where(x => x.FullName == dyn.T).FirstOrDefault();
                if (type != null)
                {
                    return JsonSerializer.Deserialize(json, type);
                }
            }

            //JsonSerializer.Deserialize<string>(json);
            return null;*/
        }
    }
}
