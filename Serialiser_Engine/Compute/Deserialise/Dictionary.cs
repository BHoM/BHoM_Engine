﻿/*
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
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static Dictionary<TK,TV> DeserialiseDictionary<TK,TV>(this BsonValue bson, ref bool failed, Dictionary<TK, TV> value)
        {
            if (value == null)
                value = new Dictionary<TK, TV>();

            if (bson.IsBsonNull)
                return null;
            else if (bson.IsBsonDocument && typeof(TK) == typeof(string))
            {
                foreach (BsonElement item in bson.AsBsonDocument)
                    value[(TK)(object)(item.Name)] = (TV)item.Value.IDeserialise(typeof(TV), ref failed);
            }
            else if (typeof(TK) != typeof(string))
            {
                BsonArray array = null;
                if (bson.IsBsonDocument)
                    array = bson["_v"].AsBsonArray;
                else
                    array = bson.AsBsonArray;

                if (array == null)
                {
                    BH.Engine.Base.Compute.RecordError("Expected to deserialise a dictionary and received " + bson.ToString() + " instead.");
                    failed = true;
                }
                else
                {
                    foreach (BsonValue item in array)
                    {
                        if (item.IsBsonDocument)
                        {
                            BsonDocument doc = item.AsBsonDocument;
                            TK key = (TK)(doc["k"].IDeserialise(typeof(TK), ref failed));
                            TV val = (TV)(doc["v"].IDeserialise(typeof(TV), ref failed));
                            if (key != null)
                                value[key] = val;
                            else
                            {
                                BH.Engine.Base.Compute.RecordError("Cannot assign a null key to a dictionary.");
                                failed = true;
                            }
                        }
                    }
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a long and received " + bson.ToString() + " instead.");
                failed = true;
            }

            return value;
        }

        /*******************************************/
    }
}