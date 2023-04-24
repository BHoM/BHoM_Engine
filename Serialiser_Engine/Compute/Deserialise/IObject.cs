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

using BH.Engine.Base;
using BH.Engine.Versioning;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/
        public static IObject DeserialiseIObject(this BsonValue bson, ref bool failed, IObject value, string version, bool isUpgraded)
        {
            if (bson.IsBsonNull)
                return null;
            else if (!bson.IsBsonDocument)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise an IObject and received " + bson.ToString() + " instead.");
                failed = true;
                return value;
            }

            BsonDocument doc = bson.AsBsonDocument;
            string docVersion = doc.Version();
            if (!string.IsNullOrEmpty(docVersion))
                version = docVersion;

            Type type = doc["_t"].DeserialiseType(ref failed, null, version, isUpgraded);
            if (type == null)
            {
                return DeserialiseDeprecate(doc, ref failed) as IObject;
            }
            if (typeof(IImmutable).IsAssignableFrom(type))
                return bson.DeserialiseImmutable(ref failed, type, version, isUpgraded);
            else if (value == null || value.GetType() != type)
                value = Activator.CreateInstance(type) as IObject;

            return SetProperties(doc, ref failed, type, value, version, isUpgraded);
        }


        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static IObject SetProperties(this BsonDocument doc, ref bool failed, Type type, IObject value, string version, bool isUpgraded)
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
                            bool upgradeFailed = false;
                            IObject newObj = DeserialiseDeprecate(doc, ref upgradeFailed) as IObject;
                            if (!upgradeFailed)
                                return newObj;
                        }

                        if (value is IBHoMObject && !item.Name.StartsWith("_"))
                        {
                            BH.Engine.Base.Compute.RecordNote($"Unable to find a proeprty named {item.Name}. Data stored in CustomData of the {type.Name}.");
                            ((IBHoMObject)value).CustomData[item.Name] = item.Value.IDeserialise(ref failed, version, isUpgraded);
                        }
                        else
                        {
                            return DeserialiseCustomObject(doc, ref failed, null, version, true);
                        }

                    }
                    else if (item.Name == "CustomData" && value is IBHoMObject)
                        ((IBHoMObject)value).CustomData = item.Value.DeserialiseDictionary(ref failed, ((IBHoMObject)value).CustomData, version, isUpgraded);
                    else
                    {
                        if (!prop.CanWrite)
                        {
                            if (!(value is IImmutable))
                            {
                                BH.Engine.Base.Compute.RecordError("Property is not settable.");
                                failed = true;
                            }
                        }
                        else
                        {

                            object propertyValue = item.Value.IDeserialise(prop.PropertyType, ref failed, prop.GetValue(value), version, isUpgraded);

                            if (CanSetValueToProperty(prop, propertyValue))
                            {
                                prop.SetValue(value, propertyValue);
                            }
                            else if (!isUpgraded)
                            {
                                return DeserialiseDeprecate(doc, ref failed) as IObject;
                            }
                            else
                            {
                                failed = true;
                                return DeserialiseCustomObject(doc, ref failed, null, version, isUpgraded);
                            }
                        }
                    }
                }

                return value;
            }
            catch (Exception e)
            {
                if (!isUpgraded)
                    return DeserialiseDeprecate(doc, ref failed) as IObject;
                else
                    return DeserialiseCustomObject(doc, ref failed, null, version, isUpgraded);
            }
            
        }

        /*******************************************/

        private static bool CanSetValueToProperty(PropertyInfo prop, object value)
        {
            if (value == null)
                return !prop.PropertyType.IsValueType || Nullable.GetUnderlyingType(prop.PropertyType) != null;
            else
                return prop.PropertyType.IsAssignableFrom(value.GetType());
        }

        /*******************************************/
    }
}
