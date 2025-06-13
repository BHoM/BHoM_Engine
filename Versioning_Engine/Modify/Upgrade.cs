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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BH.Engine.Versioning.Objects;
using BH.oM.Base;
using BH.oM.Versioning;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace BH.Engine.Versioning
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BsonDocument Upgrade(this BsonDocument document, Converter converter)
        {
            if (document == null)
                return null;

            //Clone to ensure able to compare with original document
            //Without this, the input document is changed as same reference
            BsonDocument newDoc = new BsonDocument(document);

            BsonDocument result = null;
            if (newDoc.Contains("_t"))
            {
                if (document["_t"] == "System.Reflection.MethodBase")
                    result = UpgradeMethod(newDoc, converter);
                else if (document["_t"] == "System.Type")
                    result = UpgradeType(newDoc, converter);
                else
                    result = UpgradeObject(newDoc, converter);
            }
            else if (newDoc.Contains("k") && newDoc.Contains("v"))
            {
                result = UpgradeObjectProperties(newDoc, converter);    //Calling UpgradeObjectProperties directly here, as that is the only part of UpgradeObejct method that is relevant. All other parts require _t to be set
            }

            return result;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static BsonDocument UpgradeMethod(BsonDocument document, Converter converter)
        {
            BsonValue typeName = document["TypeName"];
            BsonValue methodName = document["MethodName"];
            BsonArray parameterArray = document["Parameters"] as BsonArray;
            if (typeName == null || methodName == null || parameterArray == null)
                return null;

            // Get the method key
            string key = GetMethodKey(document);
            if (string.IsNullOrEmpty(key))
                return null;

            // First see if the method can be found in the dictionary using the old parameters
            if (converter.ToNewMethod.ContainsKey(key))
                return converter.ToNewMethod[key];

            // Check if the method is classified as deleted or without update
            CheckForNoUpgrade(converter, key, "method");

            // Update the parameter types if needed
            bool modified = false;
            List<BsonValue> parameters = new List<BsonValue>();
            foreach (BsonValue parameter in parameterArray)
            {
                string newParam = UpgradeType(parameter.AsString, converter);
                if (newParam != null)
                {
                    modified = true;
                    parameters.Add(newParam);
                }
                else
                    parameters.Add(parameter.AsString);
            }

            // Update the declaring type if needed
            string newDeclaringType = UpgradeType(typeName.AsString, converter);
            if (newDeclaringType != null)
            {
                typeName = newDeclaringType;
                modified = true;
            }

            if (modified)
            {
                document = new BsonDocument(new Dictionary<string, object>
                {
                    { "_t", "System.Reflection.MethodBase" },
                    { "TypeName", typeName },
                    { "MethodName", methodName.AsString },
                    { "Parameters", parameters }
                });
                key = GetMethodKey(document);

                if (converter.ToNewMethod.ContainsKey(key))
                    return converter.ToNewMethod[key];
                else
                    return document;
            }

            // No match found -> return null
            return null;
        }

        /***************************************************/

        private static BsonDocument UpgradeType(BsonDocument document, Converter converter)
        {
            if (document == null || !document.Contains("Name"))
                return null;

            // Check if the type is classified as deleted or without update
            string type = document["Name"].AsString;
            CheckForNoUpgrade(converter, type.Split(',').First(), "type");

            // Then check if the type can be upgraded
            bool modified = false;
            string typeFromDic = GetTypeFromDic(converter.ToNewType, type, true, !document.Contains("GenericArguments"));
            if (typeFromDic != null)
            {
                document["Name"] = typeFromDic;
                modified = true;
            }

            if (document.Contains("GenericArguments"))
            {
                BsonArray newGenerics = new BsonArray();
                BsonArray generics = document["GenericArguments"].AsBsonArray;
                if (generics != null)
                {
                    foreach (BsonDocument generic in generics)
                    {
                        BsonDocument newGeneric = Upgrade(generic, converter);
                        if (newGeneric != null)
                        {
                            modified = true;
                            newGenerics.Add(newGeneric);
                        }
                        else
                            newGenerics.Add(generic);
                    }
                }
                document["GenericArguments"] = newGenerics;
            }

            if (modified)
                return document;
            else
                return null;
        }

        /***************************************************/

        private static string UpgradeType(string type, Converter converter)
        {
            BsonDocument doc = null;
            BsonDocument.TryParse(type, out doc);
            BsonValue newType = Upgrade(doc, converter) as BsonValue;
            if (newType == null)
                return null;
            else
            {
                string newString = newType.ToString();
                if (newString.Length == 0)
                    return null;
                else
                    return newString;
            }
        }

        /***************************************************/

        private static BsonDocument UpgradeObject(BsonDocument document, Converter converter)
        {
            //Get the old type
            string oldType = "";
            try
            {
                oldType = CleanTypeString(document["_t"].AsString);
            }
            catch { }

            // Check if the object type is classified as deleted or without update
            CheckForNoUpgrade(converter, oldType, "object type");

            BsonDocument newDoc = document;

            //Check if there are any full object upgraders available
            bool upgradedExplicitly = false;
            if (converter.ToNewObject.ContainsKey(oldType))
            {
                //If so, use them to upgrade the object
                newDoc = UpgradeObjectExplicit(newDoc, converter, oldType) ?? newDoc;
                upgradedExplicitly = true;
            }

            // Upgrade properties
            newDoc = UpgradeObjectProperties(newDoc, converter) ?? newDoc;

            if (!upgradedExplicitly)
            {
                //If not, try upgrading the names of the types and properties
                newDoc = UpgradeObjectTypeAndPropertyNames(newDoc, converter, oldType) ?? newDoc;
            }

            return newDoc;
        }

        /***************************************************/

        private static BsonArray UpgradeArray(BsonArray array, Converter converter, out bool upgraded)
        {
            upgraded = false;
            if (array == null)
                return null;

            BsonArray newArray = new BsonArray();
            foreach (BsonValue item in array)
            {
                if (item is BsonDocument)
                {
                    BsonDocument doc = item as BsonDocument;
                    BsonDocument upgrade = Upgrade(doc, converter);
                    if (upgrade == null)
                        upgrade = doc;
                    else if (upgrade != doc)
                        upgraded = true;
                    newArray.Add(upgrade);
                }
                else if (item is BsonArray)
                {
                    bool itemUpgraded = false;
                    newArray.Add(UpgradeArray(item as BsonArray, converter, out itemUpgraded));
                    if (itemUpgraded)
                        upgraded = true;
                }
            }

            return newArray;
        }

        /***************************************************/

        private static BsonDocument UpgradeObjectProperties(BsonDocument document, Converter converter)
        {
            Dictionary<string, BsonValue> propertiesToUpgrade = new Dictionary<string, BsonValue>();
            foreach (BsonElement property in document)
            {
                string propName = property.Name;

                if (property.Value is BsonDocument)
                {
                    BsonDocument prop = property.Value as BsonDocument;
                    BsonDocument upgrade = Upgrade(prop, converter);
                    if (upgrade != null && prop != upgrade)
                        propertiesToUpgrade.Add(propName, upgrade);
                }
                else if (property.Value is BsonArray)
                {
                    bool upgraded = false;
                    BsonArray newArray = UpgradeArray(property.Value as BsonArray, converter, out upgraded);
                    if (upgraded)
                        propertiesToUpgrade.Add(propName, newArray);
                }
            }

            foreach (var kvp in propertiesToUpgrade)
                document[kvp.Key] = kvp.Value;

            if (propertiesToUpgrade.Count > 0)
                return document;
            else
                return null;
        }

        /***************************************************/

        private static BsonDocument UpgradeObjectExplicit(BsonDocument document, Converter converter, string oldType)
        {
            try
            {
                Dictionary<string, object> b = converter.ToNewObject[oldType](document.ToDictionary());
                if (b == null)
                    return null;

                BsonDocument newDoc = new BsonDocument(b);

                // Copy over BHoM properties if the old object was a BHoMObject (assumption around BHoM_Guid made here)
                if (document.Contains("BHoM_Guid"))
                {
                    string[] properties = new string[] { "BHoM_Guid", "CustomData", "Name", "Tags", "Fragments" };
                    foreach (string p in properties)
                    {
                        if (!newDoc.Contains(p) && document.Contains(p))
                            newDoc[p] = document[p];
                    }
                }

                return newDoc;
            }
            catch
            {
                return null;
            }
        }

        /***************************************************/

        private static BsonDocument UpgradeObjectTypeAndPropertyNames(BsonDocument document, Converter converter, string oldType)
        {
            // Upgrade the property names
            Dictionary<string, BsonElement> propertiesToRename = new Dictionary<string, BsonElement>();
            List<string> propertiesToRemove = new List<string>();
            foreach (BsonElement property in document)
            {
                string propName = property.Name;
                string key = oldType + "." + propName;
                if (converter.MessageForDeleted.ContainsKey(key))
                {
                    propertiesToRemove.Add(propName);
                }
                else if (converter.ToNewProperty.ContainsKey(key))
                {
                    propName = converter.ToNewProperty[key].Split('.').Last();
                    propertiesToRename.Add(property.Name, new BsonElement(propName, property.Value));
                }
            }

            foreach (string item in propertiesToRemove)
            {
                document.Remove(item);
            }

            foreach (var kvp in propertiesToRename)
            {
                document.Add(kvp.Value);
                document.Remove(kvp.Key);
            }

            //Try to find new type
            string newType = GetTypeFromDic(converter.ToNewType, oldType, true, true);
            if (newType != null)
            {
                document["_t"] = newType;
                return document;
            }
            else if (propertiesToRename.Count > 0)
                return document;
            else
                return null;
        }

        /***************************************************/

        private static string GetTypeFromDic(Dictionary<string, string> dic, string type, bool acceptPartialNamespace = true, bool checkRecursiveGenerics = false)
        {
            if (type.Contains(","))
                type = CleanTypeString(type);

            if (dic.ContainsKey(type))
                return dic[type];

            if (checkRecursiveGenerics && type.Contains("[["))
            {
                // Split out generic parts to be able to check for upgrades directly for each type
                int startIndex = type.IndexOf("[[");
                int endIndex = type.LastIndexOf("]]");
                string firstPart = type.Substring(0, startIndex);

                // We need something that can deal with recursive generics as well
                List<string> generics = type.Substring(startIndex + 2, endIndex - startIndex - 2)
                    .Split(new string[] { "],[" }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                //Call this method again, only for the first part. Check generics set to false, as this string now should not contain any generic arguments
                bool upgraded = false;

                string upgrFirst = GetTypeFromDic(dic, firstPart, acceptPartialNamespace, false);
                if (upgrFirst != null)
                {
                    firstPart = upgrFirst;
                    upgraded = true;
                }

                //Loop through and make sure all inner arguments are upgraded as well
                for (int i = 0; i < generics.Count; i++)
                {
                    string upgradedGeneric = GetTypeFromDic(dic, generics[i], acceptPartialNamespace, false);
                    if (upgradedGeneric != null)
                    {
                        generics[i] = upgradedGeneric;
                        upgraded = true;
                    }
                }
                //If any part upgraded, return it
                if (upgraded)
                    return firstPart + "[[" + generics.Aggregate((a, b) => a + "],[" + b) + "]]";
            }
            else
            {
                if (type.Contains("`"))
                {
                    int index = type.IndexOf("`");
                    string typeNoGenericIndicator = type.Substring(0, index);
                    if (dic.ContainsKey(typeNoGenericIndicator))
                    {
                        typeNoGenericIndicator = dic[typeNoGenericIndicator];
                        return typeNoGenericIndicator + type.Substring(index);
                    }
                }

                if (acceptPartialNamespace)
                {
                    int index = type.LastIndexOf('.');
                    while (index > 0)
                    {
                        string ns = type.Substring(0, index);
                        if (dic.ContainsKey(ns))
                            return dic[ns] + type.Substring(index);
                        else
                            index = ns.LastIndexOf('.');
                    }
                }
            }

            return null;
        }

        /***************************************************/

        private static void CheckForNoUpgrade(Converter converter, string key, string itemTypeName)
        {
            // Check if the method is classified as deleted or without update
            string message = "";
            if (converter.MessageForDeleted.ContainsKey(key))
                message = $"No upgrade for {key}. This {itemTypeName} has been deleted without replacement.\n" + converter.MessageForDeleted[key];
            else if (converter.MessageForNoUpgrade.ContainsKey(key))
                message = $"No upgrade for {key}. This {itemTypeName} cannot be upgraded automatically.\n" + converter.MessageForNoUpgrade[key];

            if (!string.IsNullOrWhiteSpace(message))
                throw new NoUpdateException(message);
        }

        /***************************************************/

        private static string GetMethodKey(BsonDocument method)
        {
            BsonValue typeName = method["TypeName"];
            BsonValue methodName = method["MethodName"];
            BsonArray parameters = method["Parameters"] as BsonArray;
            if (typeName == null || methodName == null || parameters == null)
                return null;

            string name = methodName.ToString();
            if (name == ".ctor")
                name = "";
            else
                name = "." + name;

            string declaringType = GetTypeString(typeName.AsString);

            string parametersString = "";
            List<string> parameterTypes = parameters.Select(x => GetTypeString(x.AsString)).ToList();
            if (parameterTypes.Count > 0)
                parametersString = parameterTypes.Aggregate((a, b) => a + ", " + b);

            return declaringType + name + "(" + parametersString + ")";
        }

        /***************************************************/

        private static string GetTypeString(string json)
        {
            // The type stored in json might not exist anywhere anymore so we have to go old school (i.e. no FromJson(json).ToText())
            string typeString = "";

            BsonDocument doc;
            if (BsonDocument.TryParse(json, out doc))
            {
                BsonValue name = doc["Name"];
                if (name == null)
                    return "";
                typeString = name.ToString();
                int cut = typeString.IndexOf(',');
                if (cut > 0)
                    typeString = typeString.Substring(0, cut);
            }
            else
                return "";

            int genericIndex = typeString.IndexOf('`');
            if (genericIndex < 0)
                return typeString;
            typeString = typeString.Substring(0, genericIndex);

            if (!doc.Contains("GenericArguments"))
                return typeString;

            BsonArray generics = doc["GenericArguments"] as BsonArray;
            if (generics == null)
                return typeString;

            string genericString = generics.Select(x => GetTypeString(x.ToString())).Aggregate((a, b) => a + ", " + b);
            return typeString + "<" + genericString + ">";
        }

        /***************************************************/

        private static string CleanTypeString(string oldType)
        {
            // The string might contain some version number, toke, culture, etc. So it neds cleaning first
            if (oldType.Contains("[["))
            {
                int startIndex = oldType.IndexOf("[[");
                int endIndex = oldType.LastIndexOf("]]");
                string firstPart = oldType.Substring(0, startIndex);

                // We need something that can deal with recursive generics as well
                List<string> generics = oldType.Substring(startIndex + 2, endIndex - startIndex - 2)
                    .Split(new string[] { "],[" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => CleanTypeString(x))
                    .ToList();

                return firstPart + "[[" + generics.Aggregate((a, b) => a + "],[" + b) + "]]";

            }
            else
                return oldType.Split(',').First();
        }

        /***************************************************/

    }
}




