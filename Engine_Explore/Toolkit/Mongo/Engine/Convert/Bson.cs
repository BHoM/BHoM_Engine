/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHE = Engine_Explore.Engine;


namespace Engine_Explore.Engine.Convert
{
    public static class Bson
    {
        public static BsonDocument Write(object obj, string key, DateTime timestamp)
        {
            BsonDocument document = obj.ToBsonDocument();

            /*if (obj is string) return Write(obj as string, key, timestamp);
            var document = BsonDocument.Parse(BHE.Convert.Json.Write(obj));*/

            if (key != "")
            {
                document["__Key__"] = key;
                document["__Time__"] = timestamp;
            }

            return document;
        }

        /*******************************************/

        /*public static BsonDocument Write(string obj, string key, DateTime timestamp)
        {
            var document = BsonDocument.Parse(obj);
            if (key != "")
            {
                document["__Key__"] = key;
                document["__Time__"] = timestamp;
            }

            return document;
        }*/

        /*******************************************/

        /*public static BsonDocument Write<T>(T obj) where T: BHoM.Base.BHoMObject
        {
            return new BsonDocument();
        }*/

        /*******************************************/

        public static object ReadObject(BsonDocument bson)
        {
            return BsonSerializer.Deserialize(bson, typeof(object));

            /*MongoDB.Bson.IO.JsonWriterSettings writerSettings = new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict };
            return BHE.Convert.Json.ReadObject(bson.ToJson(writerSettings));*/
        }
    }
}
