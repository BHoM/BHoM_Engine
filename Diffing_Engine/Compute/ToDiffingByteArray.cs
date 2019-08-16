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
using MongoDB.Bson;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public static byte[] ToDiffingByteArray(this object obj, PropertyInfo[] fieldsToIgnore = null)
        {
            List<PropertyInfo> propList = fieldsToIgnore?.ToList();

            if (propList == null || propList.Count == 0)
                return BsonExtensionMethods.ToBson(obj.ToBsonDocument()); // .ToBsonDocument() for consistency with other cases

            List<string> propNames = new List<string>();
            propList.ForEach(prop => propNames.Add(prop.Name));

            return ToDiffingByteArray(obj, propNames);
        }

        public static byte[] ToDiffingByteArray(this object obj, List<string> fieldsToIgnore)
        {
            if (fieldsToIgnore == null || fieldsToIgnore.Count == 0)
                return BsonExtensionMethods.ToBson(obj);

            var objDoc = obj.ToBsonDocument();

            fieldsToIgnore.ForEach(propName =>
                objDoc.Remove(propName)
            );

            return BsonExtensionMethods.ToBson(objDoc);
        }


        ///***************************************************/

    }
}
