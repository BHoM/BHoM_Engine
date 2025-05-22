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

using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.Engine.Versioning;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/
        
        private static IObject DeserialiseIObject(this BsonValue bson, IObject value, string version, bool isUpgraded)
        {
            if (bson.IsBsonNull)
                return null;
            else if (!bson.IsBsonDocument)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise an IObject and received " + bson.ToString() + " instead.");
                return value;
            }

            BsonDocument doc = bson.AsBsonDocument;
            string docVersion = doc.Version();
            if (!string.IsNullOrEmpty(docVersion))
                version = docVersion;

            Type type = doc["_t"].DeserialiseType(null, version, isUpgraded);
            if (type == null)
            {
                return DeserialiseDeprecate(doc, version, isUpgraded) as IObject;
            }

            if (typeof(IDynamicObject).IsAssignableFrom(type))
                bson = RecoverDynamicObject(type, bson as BsonDocument);


            if (typeof(IImmutable).IsAssignableFrom(type))
                return bson.DeserialiseImmutable(type, version, isUpgraded);
            else if (value == null || value.GetType() != type)
                value = Activator.CreateInstance(type) as IObject;

            return SetProperties(doc, type, value, version, isUpgraded);
        }

        /*******************************************/

        private static IObject SetProperties(this BsonDocument doc, Type type, IObject value, string version, bool isUpgraded)
        {
            try
            {
                foreach (BsonElement item in doc)
                {
                    if (item.Name.StartsWith("_"))
                        continue;

                    PropertyInfo prop = type.GetProperty(item.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (prop == null)
                    {
                        if (!isUpgraded)
                        {
                            if (TryUpgrade(doc, version, out IObject upgraded))
                            {
                                return upgraded;
                            }
                        }

                        if (value is IBHoMObject)
                        {
                            BH.Engine.Base.Compute.RecordWarning($"Unable to find a property named {item.Name}. Data stored in CustomData of the {type.Name}.");
                            ((IBHoMObject)value).CustomData[item.Name] = item.Value.IDeserialise(version, isUpgraded);
                        }
                        else
                        {
                            BH.Engine.Base.Compute.RecordError($"Unable to find a property named {item.Name} on object of type {type.Name}. CustomObject returned in its place.");
                            return DeserialiseDeprecatedCustomObject(doc, version, false);
                        }

                    }
                    else if (item.Name == "CustomData" && value is IBHoMObject)
                        ((IBHoMObject)value).CustomData = item.Value.DeserialiseDictionary(((IBHoMObject)value).CustomData, version, isUpgraded);
                    else
                    {
                        if (!prop.CanWrite)
                        {
                            if (!(value is IImmutable))
                            {
                                Base.Compute.RecordError("Property is not settable.");
                            }
                        }
                        else
                        {
                            object propertyValue = item.Value.IDeserialise(prop.PropertyType, prop.GetValue(value), version, isUpgraded);

                            if (CanSetValueToProperty(prop.PropertyType, propertyValue))
                            {
                                prop.SetValue(value, propertyValue);
                            }
                            else if (!isUpgraded)
                            {
                                return DeserialiseDeprecate(doc, version, isUpgraded) as IObject;
                            }
                            else
                            {
                                Base.Compute.RecordError($"Unable to set property {item.Name} to object of type {value?.GetType().Name ?? "uknown type"} due to a type missmatch. Expected {prop.PropertyType.Name} but serialised value was {propertyValue?.GetType().Name ?? "null"}.");
                                return DeserialiseDeprecatedCustomObject(doc, version, false);
                            }
                        }
                    }
                }

                return value;
            }
            catch (Exception e)
            {
                if (!isUpgraded)
                {
                    return DeserialiseDeprecate(doc, version, isUpgraded) as IObject;
                }
                else
                {
                    return DeserialiseDeprecatedCustomObject(doc, version);
                }
            }
            
        }

        /*******************************************/

        private static bool CanSetValueToProperty(Type propType, object value)
        {
            if (value == null)
                return !propType.IsValueType || Nullable.GetUnderlyingType(propType) != null;
            else
                return propType.IsAssignableFrom(value.GetType());
        }

        /*******************************************/

        private static BsonDocument RecoverDynamicObject(Type type, BsonDocument doc)
        {
            List<PropertyInfo> dynamicContainers = type.GetProperties()
                .Where(x => x.GetCustomAttribute<DynamicPropertyAttribute>() != null && typeof(IDictionary).IsAssignableFrom(x.PropertyType))
                .ToList();

            foreach (PropertyInfo containerInfo in dynamicContainers)
            {
                Type keyType = containerInfo.PropertyType.GenericTypeArguments.FirstOrDefault();
                if (keyType == null || !keyType.IsEnum)
                    continue;

                List<string> propNames = Enum.GetValues(keyType)
                    .OfType<Enum>()
                    .Select(x => x.IToText())
                    .Where(x => doc.Contains(x))
                    .ToList();

                BsonArray container = new BsonArray();

                foreach (string propName in propNames)
                {
                    BsonDocument kvp = new BsonDocument();
                    kvp["k"] = propName;
                    kvp["v"] = doc[propName];
                    container.Add(kvp);
                    doc.Remove(propName);
                }
                   
                doc[containerInfo.Name] = container;
            }

            return doc;
        }

        /*******************************************/
    }
}


