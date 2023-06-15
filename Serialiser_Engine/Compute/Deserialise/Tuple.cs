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

        private static object DeserialiseTuple(this BsonValue bson, Type targetType, string version, bool isUpgraded)
        {
            BsonValue value = bson;
            if (bson.IsBsonDocument)
            {
                BsonDocument doc = bson.AsBsonDocument;
                value = doc["_v"];
                if (targetType == null)
                    targetType = doc["_t"].DeserialiseType(targetType, version, isUpgraded);
            }
            
            Type[] keys = targetType.GetGenericArguments();
            object tuple = Activator.CreateInstance(targetType, keys.Select(x => GetDefaultValue(x)).ToArray());
            if (tuple != null)
                return DeserialiseTuple(value, tuple as dynamic, version, isUpgraded);
            else
            {
                BH.Engine.Base.Compute.RecordError("Failed to create tuple from " + bson.ToString());
                return null;
            }
        }

        /*******************************************/

        private static Tuple<T1, T2> DeserialiseTuple<T1, T2>(this BsonValue bson, Tuple<T1, T2> value, string version, bool isUpgraded)
        {
            if (bson.IsBsonNull)
                return null;
            else if (bson.IsBsonArray)
            {
                BsonArray array = bson.AsBsonArray;
                if (array.Count != 2)
                {
                    BH.Engine.Base.Compute.RecordError("The tuple was expected to be represented by an array of size 2.");
                }
                else
                {
                    return new Tuple<T1, T2>(
                        (T1)array[0].IDeserialise(typeof(T1), value.Item1, version, isUpgraded),
                        (T2)array[1].IDeserialise(typeof(T2), value.Item2, version, isUpgraded)
                    );
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a tuple and received " + bson.ToString() + " instead.");
            }

            return value;
        }

        /*******************************************/
        
        private static Tuple<T1, T2, T3> DeserialiseTuple<T1, T2, T3>(this BsonValue bson, Tuple<T1, T2, T3> value, string version, bool isUpgraded)
        {
            if (bson.IsBsonNull)
                return null;
            else if (bson.IsBsonArray)
            {
                BsonArray array = bson.AsBsonArray;
                if (array.Count != 3)
                {
                    BH.Engine.Base.Compute.RecordError("The tuple was expected t obe represented by an array of size 3.");
                }
                else
                {
                    return new Tuple<T1, T2, T3>(
                        (T1)array[0].IDeserialise(typeof(T1), value.Item1, version, isUpgraded),
                        (T2)array[1].IDeserialise(typeof(T2), value.Item2, version, isUpgraded),
                        (T3)array[2].IDeserialise(typeof(T3), value.Item3, version, isUpgraded)
                    );
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a tuple and received " + bson.ToString() + " instead.");
            }

            return value;
        }

        /*******************************************/
        
        private static Tuple<T1, T2, T3, T4> DeserialiseTuple<T1, T2, T3, T4>(this BsonValue bson, Tuple<T1, T2, T3, T4> value, string version, bool isUpgraded)
        {
            if (bson.IsBsonNull)
                return null;
            else if (bson.IsBsonArray)
            {
                BsonArray array = bson.AsBsonArray;
                if (array.Count != 4)
                {
                    BH.Engine.Base.Compute.RecordError("The tuple was expected t obe represented by an array of size 4.");
                }
                else
                {
                    return new Tuple<T1, T2, T3, T4>(
                        (T1)array[0].IDeserialise(typeof(T1), value.Item1, version, isUpgraded),
                        (T2)array[1].IDeserialise(typeof(T2), value.Item2, version, isUpgraded),
                        (T3)array[2].IDeserialise(typeof(T3), value.Item3, version, isUpgraded),
                        (T4)array[3].IDeserialise(typeof(T4), value.Item4, version, isUpgraded)
                    );
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a tuple and received " + bson.ToString() + " instead.");
            }

            return value;
        }

        /*******************************************/
        
        private static Tuple<T1, T2, T3, T4, T5> DeserialiseTuple<T1, T2, T3, T4, T5>(this BsonValue bson, Tuple<T1, T2, T3, T4, T5> value, string version, bool isUpgraded)
        {
            if (bson.IsBsonNull)
                return null;
            else if (bson.IsBsonArray)
            {
                BsonArray array = bson.AsBsonArray;
                if (array.Count != 5)
                {
                    BH.Engine.Base.Compute.RecordError("The tuple was expected t obe represented by an array of size 5.");
                }
                else
                {
                    return new Tuple<T1, T2, T3, T4, T5>(
                        (T1)array[0].IDeserialise(typeof(T1), value.Item1, version, isUpgraded),
                        (T2)array[1].IDeserialise(typeof(T2), value.Item2, version, isUpgraded),
                        (T3)array[2].IDeserialise(typeof(T3), value.Item3, version, isUpgraded),
                        (T4)array[3].IDeserialise(typeof(T4), value.Item4, version, isUpgraded),
                        (T5)array[4].IDeserialise(typeof(T5), value.Item5, version, isUpgraded)
                    );
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a tuple and received " + bson.ToString() + " instead.");
            }

            return value;
        }

        /*******************************************/
    }
}
