/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.Engine.Versioning;
using System.ComponentModel;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        [Description("Used to support FromJson, not recomended to be used in isolation.")]
        public static object IDeserialise(this BsonValue bson)
        {
            return IDeserialise(bson, "", false);
        }

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static object IDeserialise(this BsonValue bson, string version, bool isUpgraded)
        {
            if (bson.IsBsonNull)
                return null;
            else if (bson.IsBsonArray)
            {
                if (IsNestedList(bson))
                    return bson.DeserialiseNestedList(new List<List<object>>(), version, isUpgraded);
                else
                    return bson.DeserialiseList(new List<object>(), version, isUpgraded);
            }
            else if (bson.IsBsonDocument)
            {
                BsonDocument doc = bson.AsBsonDocument;
                if (!doc.Contains("_t"))
                {
                    return bson.DeserialiseCustomObject(null, version, isUpgraded);
                }
                else if (doc["_t"] == "System.String")
                    return bson.DeserialiseString();
                else
                {
                    string docVersion = doc.Version();
                    if (!string.IsNullOrEmpty(docVersion))
                        version = docVersion;
                    Type type = doc["_t"].DeserialiseType(null, version, isUpgraded);
                    if (type == null)
                        return DeserialiseDeprecate(doc, version, isUpgraded) as IObject;
                    else
                        return IDeserialise(bson, type, null, version, isUpgraded);
                }
            }
            else
                return BsonTypeMapper.MapToDotNetValue(bson);
        }

        /*******************************************/

        private static object IDeserialise(this BsonValue bson, Type targetType, object value, string version, bool isUpgraded)
        {
            if (bson.IsBsonNull)
                return null;

            // Cover all base types
            switch (targetType.FullName)
            {
                case "System.Boolean":
                    return DeserialiseBoolean(bson);
                case "System.Drawing.Bitmap":
                    return DeserialiseBitmap(bson);
                case "BH.oM.Base.CustomObject":
                    return DeserialiseCustomObject(bson, null, version, isUpgraded);
                case "System.Data.DataTable":
                    return DeserialiseDataTable(bson, null, version, isUpgraded);
                case "System.DateTime":
                    return DeserialiseDateTime(bson);
                case "System.DateTimeOffset":
                    return DeserialiseDateTimeOffset(bson);
                case "System.Decimal":
                    return DeserialiseDecimal(bson);
                case "System.Double":
                    return DeserialiseDouble(bson);
                case "System.Drawing.Color":
                    return DeserialiseColour(bson);
                case "BH.oM.Base.FragmentSet":
                    return DeserialiseFragmentSet(bson, null, version, isUpgraded);
                case "System.Guid":
                    return DeserialiseGuid(bson);
                case "System.Int16": // short
                    return DeserialiseShort(bson);
                case "System.Int32": // integer
                    return DeserialiseInteger(bson);
                case "System.Int64": // long
                    return DeserialiseLong(bson);
                case "System.UInt32": // unsigned integer (uint)
                    return DeserialiseUnsignedInteger(bson);
                case "System.Numerics.Complex":
                    return DeserialiseComplex(bson);
                case "System.IntPtr":
                    return DeserialiseIntPtr(bson);
                case "System.Object":
                    return IDeserialise(bson, version, isUpgraded);
                case "System.Reflection.MethodBase":
                case "System.Reflection.MethodInfo":
                case "System.Reflection.ConstructorInfo":
                    return DeserialiseMethodBase(bson);
                case "System.Text.RegularExpressions.Regex":
                    return DeserialiseRegex(bson);
                case "System.Single": // float
                    return DeserialiseFloat(bson);
                case "System.String":
                    return DeserialiseString(bson);
                case "System.TimeSpan":
                    return DeserialiseTimeSpan(bson);
                case "System.Type":
                    return DeserialiseType(bson, null, version, isUpgraded);
            }

            // Cover Sytem generic types
            switch (targetType.Name)
            {
                case "Dictionary`2":
                    return DeserialiseDictionary(bson, EnsureNotNullAndClear(value as ICollection, targetType) as dynamic, version, isUpgraded);
                case "HashSet`1":
                    return DeserialiseHashSet(bson, EnsureNotNullAndClear(value as ICollection, targetType) as dynamic, version, isUpgraded);
                case "List`1":
                    return DeserialiseList(bson, EnsureNotNullAndClear(value as ICollection, targetType) as dynamic, version, isUpgraded);
                case "IDictionary`2":
                    return DeserialiseDictionary(bson, CreateEmptyDictionary(targetType) as dynamic, version, isUpgraded);
                case "IEnumerable`1":
                case "IList`1":
                case "IReadOnlyList`1":
                    return DeserialiseList(bson, CreateEmptyList(targetType) as dynamic, version, isUpgraded);
                case "IComparable":
                case "IComparable`1":
                    return IDeserialise(bson, version, isUpgraded);
                case "Nullable`1":
                    return DeserialiseNullable(bson, targetType, version, isUpgraded);
                case "SortedDictionary`2":
                    return DeserialiseSortedDictionary(bson, CreateEmptyDictionary(targetType) as dynamic, version, isUpgraded);
                case "ReadOnlyCollection`1":
                    return DeserialiseReadOnlyCollection(bson, CreateEmptyList(targetType) as dynamic, version, isUpgraded);
                case "Tuple`2":
                case "Tuple`3":
                case "Tuple`4":
                case "Tuple`5":
                    return DeserialiseTuple(bson, targetType, version, isUpgraded);
                case "Enum":
                    return DeserialiseEnumTopLevel(bson, value as Enum, version, isUpgraded);
            }

            if (targetType.IsEnum)
                return DeserialiseEnum(bson, EnsureNotNull(value, targetType) as dynamic, version, isUpgraded);
            else if (typeof(IObject).IsAssignableFrom(targetType) || (targetType.IsInterface && targetType.FullName.StartsWith("BH.")))
                return DeserialiseIObject(bson, value as dynamic, version, isUpgraded);
            else if (typeof(Array).IsAssignableFrom(targetType))
                return DeserialiseArray(bson, CreateEmptyArray(targetType) as dynamic, version, isUpgraded);
            else
            {
                BH.Engine.Base.Compute.RecordError("Object of type " + targetType.ToString() + " cannot be deserialised.");
                return null;
            }
        }


        /*******************************************/
        /**** Private Methods - Support         ****/
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

        private static BsonValue ExtractValue(BsonValue bson)
        {
            if (bson != null && bson.IsBsonDocument)
            {
                BsonDocument doc = bson.AsBsonDocument;
                if (doc.Contains("_v"))
                    return doc["_v"];
                else
                    BH.Engine.Base.Compute.RecordError($"BsonDocument does not contain expected _v: {bson.ToString()}");
            }

            return bson;
        }

        /*******************************************/
    }
}



