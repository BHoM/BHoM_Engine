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
        /**** Public Methods                    ****/
        /*******************************************/

        public static object DeserialiseTuple(this BsonValue bson, ref bool failed, Type targetType)
        {
            Type[] keys = targetType.GetGenericArguments();
            object tuple = Activator.CreateInstance(targetType, keys.Select(x => GetDefaultValue(x)).ToArray());
            if (tuple != null)
                return DeserialiseTuple(bson, ref failed, tuple as dynamic);
            else
            {
                failed = true;
                BH.Engine.Base.Compute.RecordError("Failed to create tuple from " + bson.ToString());
                return null;
            }
        }


        /*******************************************/

        public static Tuple<T1, T2> DeserialiseTuple<T1, T2>(this BsonValue bson, ref bool failed, Tuple<T1, T2> value = null)
        {
            if (bson.IsBsonArray)
            {
                BsonArray array = bson.AsBsonArray;
                if (array.Count != 2)
                {
                    failed = true;
                    BH.Engine.Base.Compute.RecordError("The tuple was expected to be represented by an array of size 2.");
                }
                else
                {
                    return new Tuple<T1, T2>(
                        (T1)array[0].IDeserialise(typeof(T1), ref failed, value.Item1),
                        (T2)array[1].IDeserialise(typeof(T2), ref failed, value.Item2)
                    );
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a tuple and received " + bson.ToString() + " instead.");
                failed = true;
            }

            return value;
        }

        /*******************************************/
        public static Tuple<T1, T2, T3> DeserialiseTuple<T1, T2, T3>(this BsonValue bson, ref bool failed, Tuple<T1, T2, T3> value = null)
        {
            if (bson.IsBsonArray)
            {
                BsonArray array = bson.AsBsonArray;
                if (array.Count != 3)
                {
                    failed = true;
                    BH.Engine.Base.Compute.RecordError("The tuple was expected t obe represented by an array of size 3.");
                }
                else
                {
                    return new Tuple<T1, T2, T3>(
                        (T1)array[0].IDeserialise(typeof(T1), ref failed, value.Item1),
                        (T2)array[1].IDeserialise(typeof(T2), ref failed, value.Item2),
                        (T3)array[2].IDeserialise(typeof(T3), ref failed, value.Item3)
                    );
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a tuple and received " + bson.ToString() + " instead.");
                failed = true;
            }

            return value;
        }

        /*******************************************/
        public static Tuple<T1, T2, T3, T4> DeserialiseTuple<T1, T2, T3, T4>(this BsonValue bson, ref bool failed, Tuple<T1, T2, T3, T4> value = null)
        {
            if (bson.IsBsonArray)
            {
                BsonArray array = bson.AsBsonArray;
                if (array.Count != 4)
                {
                    failed = true;
                    BH.Engine.Base.Compute.RecordError("The tuple was expected t obe represented by an array of size 4.");
                }
                else
                {
                    return new Tuple<T1, T2, T3, T4>(
                        (T1)array[0].IDeserialise(typeof(T1), ref failed, value.Item1),
                        (T2)array[1].IDeserialise(typeof(T2), ref failed, value.Item2),
                        (T3)array[2].IDeserialise(typeof(T3), ref failed, value.Item3),
                        (T4)array[3].IDeserialise(typeof(T4), ref failed, value.Item4)
                    );
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a tuple and received " + bson.ToString() + " instead.");
                failed = true;
            }

            return value;
        }

        /*******************************************/
        public static Tuple<T1, T2, T3, T4, T5> DeserialiseTuple<T1, T2, T3, T4, T5>(this BsonValue bson, ref bool failed, Tuple<T1, T2, T3, T4, T5> value = null)
        {
            if (bson.IsBsonArray)
            {
                BsonArray array = bson.AsBsonArray;
                if (array.Count != 5)
                {
                    failed = true;
                    BH.Engine.Base.Compute.RecordError("The tuple was expected t obe represented by an array of size 5.");
                }
                else
                {
                    return new Tuple<T1, T2, T3, T4, T5>(
                        (T1)array[0].IDeserialise(typeof(T1), ref failed, value.Item1),
                        (T2)array[1].IDeserialise(typeof(T2), ref failed, value.Item2),
                        (T3)array[2].IDeserialise(typeof(T3), ref failed, value.Item3),
                        (T4)array[3].IDeserialise(typeof(T4), ref failed, value.Item4),
                        (T5)array[4].IDeserialise(typeof(T5), ref failed, value.Item5)
                    );
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a tuple and received " + bson.ToString() + " instead.");
                failed = true;
            }

            return value;
        }

        /*******************************************/
    }
}
