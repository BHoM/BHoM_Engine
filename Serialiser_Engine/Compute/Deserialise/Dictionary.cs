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

using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static Dictionary<TK,TV> DeserialiseDictionary<TK,TV>(this BsonValue bson, Dictionary<TK, TV> value, string version, bool isUpgraded)
        {
            if (value == null)
                value = new Dictionary<TK, TV>();

            if (bson.IsBsonNull)
                return null;
            else if (bson.IsBsonDocument && typeof(TK) == typeof(string))
            {
                foreach (BsonElement item in bson.AsBsonDocument)
                {
                    if(item.Name != "_t" && item.Name != "_bhomVersion")
                        value[(TK)(object)(item.Name)] = (TV)item.Value.IDeserialise(typeof(TV), null, version, isUpgraded);
                }
            }
            else if (typeof(TK) != typeof(string))
            {
                BsonArray array = DictionaryItemArray(bson);

                if (array == null)
                {           
                    BH.Engine.Base.Compute.RecordError("Expected to deserialise a dictionary and received " + bson.ToString() + " instead.");
                }
                else
                {
                    foreach (BsonValue item in array)
                    {
                        if (item.IsBsonDocument)
                        {
                            BsonDocument doc = item.AsBsonDocument;
                            TK key = (TK)(doc["k"].IDeserialise(typeof(TK), null, version, isUpgraded));
                            TV val = (TV)(doc["v"].IDeserialise(typeof(TV), null, version, isUpgraded));
                            if (key != null)
                                value[key] = val;
                            else
                            {
                                BH.Engine.Base.Compute.RecordError("Cannot assign a null key to a dictionary.");
                            }
                        }
                    }
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a dictionary and received " + bson.ToString() + " instead.");
            }

            return value;
        }

        /*******************************************/

        private static BsonArray DictionaryItemArray(BsonValue bson)
        {
            if (bson.IsBsonDocument)
            {
                if (bson.AsBsonDocument.Contains("_v"))     //Case when dictionary is the top level object
                    return bson["_v"].AsBsonArray;
                else
                {
                    BsonArray array = new BsonArray();      //Older case that should no longer be used. Kept to support older formats of serialisation
                    foreach (var element in bson.AsBsonDocument.Elements)
                    {
                        if (element.Name.StartsWith("_"))
                            continue;

                        BsonDocument doc = new BsonDocument();
                        doc["k"] = element.Name;
                        doc["v"] = element.Value;
                        array.Add(doc);
                    }
                    return array;
                }
            }
            else if (bson.IsBsonArray)
                return bson.AsBsonArray;    //Non-top level serialisation

            return null;
        }

        /*******************************************/
    }
}
