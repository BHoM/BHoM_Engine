/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using System;
using System.Collections;
using BH.oM.Base;
using BH.Engine.Base;

namespace BH.Engine.Serialiser
{
    public static partial class Convert
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        [Description("Convert a BHoM object To a Json string")]
        [Input("obj", "Object to be converted")]
        [Output("json", "String representing the object in json")]
        public static string ToJson(this object obj)
        {
            if (obj == null)
                return "";

            if (obj is string)
                return "{ \"_t\": \"System.String\", \"_v\": " + BsonExtensionMethods.ToJson<string>(obj as string) + "}";

            if (obj is IEnumerable && !(obj is IObject) && !(obj is IDictionary))
                return ToJsonArray((obj as IEnumerable).OfType<object>());

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return Convert.ToBson(obj).ToJson<BsonDocument>(jsonWriterSettings);
        }

        /*******************************************/

        [Description("Convert a Json string to a BHoMObject")]
        [Input("json", "String representing the object in json")]
        [Output("obj", "Object recovered from the Json string")]
        public static object FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            // Trim the json to remove any trailing or initial whitespace/newline (#3109).
            json = json.Trim();

            if (json.StartsWith("{"))
            {
                BsonDocument document;
                if (BsonDocument.TryParse(json, out document))
                {
                    return Convert.FromBson(document);
                }
                else
                {
                    Base.Compute.RecordError("The string provided is not a supported json format");
                    return null;
                }
            }
            else if (json.StartsWith("["))
            {
                Base.Compute.RecordNote($"The string provided appears to be a Json Array and will be deserialized as a `List<object>`." +
                    $"\nIf calling this method from an UI and you intend to retrieve the individual list items, please use the method {nameof(FromJsonArray)} instead.");

                return FromJsonArray(json);
            }
            else
            {
                // Could we do something when a string is not a valid json?
                Base.Compute.RecordError("The string provided is not a valid json format");
                return null;
            }
        }

        /*******************************************/

        [Description("Convert a List of objects to a Json array.")]
        [Input("objs", "Objects to be converted.")]
        [Output("json", "String representing the object in json.")]
        public static string ToJsonArray(this IEnumerable<object> objs)
        {
            List<string> allLines = new List<string>();
            string json = "";

            // Join all individually serialised objects with a comma and a newline
            json = string.Join(",\n", objs.Where(c => c != null).Select(obj => obj.ToJson()));

            // Put between square brackets, to make a valid JSON array.
            json = "[" + json + "]";

            return json;
        }

        /*******************************************/

        [Description("Convert a Json string to a collection of objects")]
        [Input("jsonArray", "String representing the objects in a Json Array form.")]
        [Output("objs", "Objects recovered from the Json string")]
        public static IEnumerable<object> FromJsonArray(this string jsonArray)
        {
            if (!jsonArray.StartsWith("[") || !jsonArray.EndsWith("]"))
            {
                BH.Engine.Base.Compute.RecordError("The specified text is not a well-formed Json Array.");
                return null;
            }

            BsonArray array = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(jsonArray);

            IEnumerable<object> objList = array.Select(b => b.IsBsonDocument ? FromBson(b.AsBsonDocument) : FromJson(b.ToString())).ToList();

            return objList;
        }

        /*******************************************/
    }
}



