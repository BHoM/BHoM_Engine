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
using System.Collections.ObjectModel;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static object IDeserialise(this BsonValue bson, ref bool failed)
        {
            if (bson.IsBsonArray)
                return bson.DeserialiseList(ref failed, new List<object>());
            else if (bson.IsBsonDocument)
            {
                BsonDocument doc = bson.AsBsonDocument;
                if (!doc.Contains("_t"))
                {
                    return bson.DeserialiseCustomObject(ref failed);
                }
                else if (doc["_t"] == "System.String")
                    return bson.DeserialiseString(ref failed);
                else
                {
                    Type type = doc["_t"].DeserialiseType(ref failed);
                    if (type == null)
                    {
                        failed = true;
                        BH.Engine.Base.Compute.RecordError("Failed to deserialise object of unknown type " + bson["_t"].AsString + ".");
                        return null;
                    }
                    else
                        return IDeserialise(bson, type, ref failed);
                }
            }
            else
                return BsonTypeMapper.MapToDotNetValue(bson);
        }

        /*******************************************/

        public static object IDeserialise(this BsonValue bson, Type targetType, ref bool failed, object value = null)
        {
            // Cover all base types
            switch (targetType.FullName)
            {
                case "System.Boolean":
                    return DeserialiseBoolean(bson, ref failed);
                case "System.Drawing.Bitmap":
                    return DeserialiseBitmap(bson, ref failed);
                case "BH.oM.Base.CustomObject":
                    return DeserialiseCustomObject(bson, ref failed);
                case "System.Data.DataTable":
                    return DeserialiseDataTable(bson, ref failed);
                case "System.DateTime":
                    return DeserialiseDateTime(bson, ref failed);
                case "System.DateTimeOffset":
                    return DeserialiseDateTimeOffset(bson, ref failed);
                case "System.Decimal":
                    return DeserialiseDecimal(bson, ref failed);
                case "System.Double":
                    return DeserialiseDouble(bson, ref failed);
                case "System.Drawing.Color":
                    return DeserialiseColor(bson, ref failed);
                case "BH.oM.Base.FragmentSet":
                    return DeserialiseFragmentSet(bson, ref failed);
                case "System.Guid":
                    return DeserialiseGuid(bson, ref failed);
                case "System.Int16": // short
                    return DeserialiseShort(bson, ref failed);
                case "System.Int32": // integer
                    return DeserialiseInteger(bson, ref failed);
                case "System.Int64": // long
                    return DeserialiseLong(bson, ref failed);
                case "System.IntPtr":
                    return DeserialiseIntPtr(bson, ref failed);
                case "System.Object":
                    return IDeserialise(bson, ref failed);
                case "System.Reflection.MethodBase":
                    return DeserialiseMethodBase(bson, ref failed);
                case "System.Text.RegularExpressions.Regex":
                    return DeserialiseRegex(bson, ref failed);
                case "System.Single": // float
                    return DeserialiseFloat(bson, ref failed);
                case "System.String":
                    return DeserialiseString(bson, ref failed);
                case "System.TimeSpan":
                    return DeserialiseTimeSpan(bson, ref failed);
                case "System.Type":
                    return DeserialiseType(bson, ref failed);
            }

            // Cover Sytem generic types
            switch (targetType.Name)
            {
                case "Dictionary`2":
                    return DeserialiseDictionary(bson, ref failed, EnsureNotNullAndClear(value as ICollection, targetType) as dynamic);
                case "HashSet`1":
                    return DeserialiseHashSet(bson, ref failed, EnsureNotNullAndClear(value as ICollection, targetType) as dynamic);
                case "List`1":
                    return DeserialiseList(bson, ref failed, EnsureNotNullAndClear(value as ICollection, targetType) as dynamic);
                case "IDictionary`2":
                    return DeserialiseDictionary(bson, ref failed, CreateEmptyDictionary(targetType) as dynamic);
                case "IEnumerable`1":
                case "IList`1":
                case "IReadOnlyList`1":
                    return DeserialiseList(bson, ref failed, CreateEmptyList(targetType) as dynamic);
                case "IComparable":
                case "IComparable`1":
                    return IDeserialise(bson, ref failed);
                case "Nullable`1":
                    return DeserialiseNullable(bson, ref failed, targetType);
                case "SortedDictionary`2":
                    return DeserialiseSortedDictionary(bson, ref failed, CreateEmptyDictionary(targetType) as dynamic);
                case "ReadOnlyCollection`1":
                    return DeserialiseReadOnlyCollection(bson, ref failed, CreateEmptyList(targetType) as dynamic);
                case "Tuple`2":
                case "Tuple`3":
                case "Tuple`4":
                case "Tuple`5":
                    return DeserialiseTuple(bson, ref failed, targetType);
            }

            if (targetType.IsEnum)
                return DeserialiseEnum(bson, ref failed, EnsureNotNull(value, targetType) as dynamic);
            else if (typeof(IObject).IsAssignableFrom(targetType) || (targetType.IsInterface && targetType.FullName.StartsWith("BH.")))
                return DeserialiseIObject(bson, ref failed, value as dynamic);
            else if (typeof(Array).IsAssignableFrom(targetType))
                return DeserialiseArray(bson, ref failed, CreateEmptyArray(targetType) as dynamic);
            else
            {
                BH.Engine.Base.Compute.RecordError("Object of type " + targetType.ToString() + " cannot be deserialised.");
                failed = true;
                return null;
            }
        }


        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static object EnsureNotNull(object value, Type targetType)
        {
            if (targetType.IsAbstract)
                return value;
            else if (value == null)
                return Activator.CreateInstance(targetType);
            else
                return value;
        }

        /*******************************************/

        private static object EnsureNotNullAndClear(ICollection value, Type targetType)
        {
            if (targetType.IsAbstract)
                return value;
            else if (value == null || value.Count > 0)
                return Activator.CreateInstance(targetType);
            else
                return value;
        }

        /*******************************************/

        private static object CreateEmptyList(Type targetType)
        {
            Type itemType = targetType.GetGenericArguments()[0];
            Type listType = typeof(List<>);
            Type constructedListType = listType.MakeGenericType(itemType);
            return Activator.CreateInstance(constructedListType);  
        }

        /*******************************************/

        private static object CreateEmptyDictionary(Type targetType)
        {
            Type[] types = targetType.GetGenericArguments();
            Type dicType = typeof(Dictionary<,>);
            Type constructedDicType = dicType.MakeGenericType(types);
            return Activator.CreateInstance(constructedDicType);
        }

        /*******************************************/

        private static object CreateEmptyArray(Type targetType)
        {
            if (targetType.Name.EndsWith("[]"))
                return Activator.CreateInstance(targetType, new object[] { 0 });
            else if (targetType.Name.EndsWith("[,]"))
                return Activator.CreateInstance(targetType, new object[] { 0, 0 });
            else
                return null;
        }

        /*******************************************/

        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        /*******************************************/
    }
}



