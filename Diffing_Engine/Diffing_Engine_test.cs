using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using BH.Engine.Serialiser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diffing_Engine
{
    public static partial class TestDiffing
    {
        public static void Main()
        {
            List<object> objs = new List<object>();
            List<string> listStringHashes = new List<string>();


            Bar bar = new Bar();
            Bar bar1 = new Bar();

            bar.Name = "bar";
            objs.Add(bar);
            bar1.Name = "bar1";
            objs.Add(bar1);

            TestclassA point = new TestclassA();
            point.X = 10;
            point.Y = 10;
            TestclassB coord = new TestclassB();
            coord.X = 10;
            coord.Y = 10;

            objs.Add(point);
            objs.Add(coord);

            //objs.Add(BH.Engine.Base.Create.RandomObject(typeof(BH.oM.Geometry.Point)));

            //objs.Add(BH.Engine.Base.Create.RandomObject(typeof(Bar)));
            //objs.Add(BH.Engine.Base.Create.RandomObject(typeof(Bar)));


            //TestHashing_Protobuf(objs);


            for (int i = 0; i < objs.Count; i++)
            {
                var json = objs[i].PrepareJson(new List<string>() { "BHoM_Guid" });
                //var json = objs[i].PrepareJson(typeof(BH.oM.Base.BHoMObject).GetProperties());

                var hashString = GetHashString(json);
                Console.WriteLine(objs[i].GetType().Name + ": " + hashString);
            }

        }

        public static byte[] GetSHA256Hash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetSHA256Hash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        static string GetMd5Hash(MD5 md5Hash, byte[] objByte)
        {
            byte[] data = md5Hash.ComputeHash(objByte);

            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static string PrepareJson(this object obj, PropertyInfo[] propertiesToRemove = null)
        {
            List<PropertyInfo> propList = propertiesToRemove.ToList();
            if (propList == null && propList.Count == 0)
                return obj.ToJson();

            List<string> propNames = new List<string>();
            propList.ForEach(prop => propNames.Add(prop.Name));
            return PrepareJson(obj, propNames);
        }

        public static string PrepareJson(this object obj, List<string> propertiesToRemove = null)
        {
            if (propertiesToRemove == null && propertiesToRemove.Count == 0)
                return obj.ToJson();

            var jObject = JsonConvert.DeserializeObject<JObject>(obj.ToJson());

            // Sets properties to be ignored as null, without altering the tree.
            propertiesToRemove.ForEach(propName =>
            jObject.Properties()
                .Where(attr => attr.Name.StartsWith(propName))
                .ToList()
                .ForEach(attr => attr.Value = null)
            );
            return jObject.ToString();
        }
    }
}
