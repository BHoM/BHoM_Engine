using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diffing_Engine
{
    public static partial class Compute
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        //public static Delta Diffing(List<IBHoMObject> ToPush, List<IBHoMObject> Read)
        //{
        //    var groups_ToPush = ToPush.GroupBy(x => x.GetType());
        //    var groups_Read = Read.GroupBy(x => x.GetType());

        //    Delta delta = new Delta(null, null, null, "", "", 0, "");
        //    return null;
        //}


        //public static string PrepareJson(this object obj, PropertyInfo[] propertiesToRemove = null)
        //{
        //    List<PropertyInfo> propList = propertiesToRemove.ToList();
        //    if (propList == null && propList.Count == 0)
        //        return obj.ToJson();

        //    List<string> propNames = new List<string>();
        //    propList.ForEach(prop => propNames.Add(prop.Name));
        //    return PrepareJson(obj, propNames);
        //}

        //public static string PrepareJson(this object obj, List<string> propertiesToRemove = null)
        //{
        //    if (propertiesToRemove == null && propertiesToRemove.Count == 0)
        //        return obj.ToJson();

        //    var jObject = JsonConvert.DeserializeObject<JObject>(obj.ToJson());

        //    // Sets properties to be ignored as null, without altering the tree.
        //    propertiesToRemove.ForEach(propName =>
        //    jObject.Properties()
        //        .Where(attr => attr.Name.StartsWith(propName))
        //        .ToList()
        //        .ForEach(attr => attr.Value = null)
        //    );
        //    return jObject.ToString();
        //}

        ///***************************************************/

        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/


        //private static byte[] SHA256(string inputString)
        //{
        //    HashAlgorithm algorithm = System.Security.Cryptography.SHA256.Create();
        //    return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        //}

        //private static string GetMd5Hash(MD5 md5Hash, byte[] objByte)
        //{
        //    byte[] data = md5Hash.ComputeHash(objByte);

        //    StringBuilder sBuilder = new StringBuilder();

        //    // Loop through each byte of the hashed data and format each one as a hexadecimal string
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        sBuilder.Append(data[i].ToString("x2"));
        //    }

        //    return sBuilder.ToString();
        //}

        ///***************************************************/

    }
}
