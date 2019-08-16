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
using BH.oM.Reflection.Attributes;
using System.ComponentModel;


namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public static string ToDiffingJson(this object obj, PropertyInfo[] fieldsToNullify = null)
        {
            List<PropertyInfo> propList = fieldsToNullify.ToList();
            if (propList == null && propList.Count == 0)
                return BH.Engine.Serialiser.Convert.ToJson(obj);

            List<string> propNames = new List<string>();
            propList.ForEach(prop => propNames.Add(prop.Name));
            return ToDiffingJson(obj, propNames);
        }

        public static string ToDiffingJson(this object obj, List<string> fieldsToNullify)
        {
            if (fieldsToNullify == null && fieldsToNullify.Count == 0)
                return BH.Engine.Serialiser.Convert.ToJson(obj);

            var jObject = JsonConvert.DeserializeObject<JObject>(BH.Engine.Serialiser.Convert.ToJson(obj));

            // Sets fields to be ignored as null, without altering the tree.
            fieldsToNullify.ForEach(propName =>
            jObject.Properties()
                .Where(attr => attr.Name.StartsWith(propName))
                .ToList()
                .ForEach(attr => attr.Value = null)
            );
            return jObject.ToString();
        }


        ///***************************************************/

    }
}
