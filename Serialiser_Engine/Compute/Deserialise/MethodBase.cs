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

using BH.Engine.Reflection;
using BH.Engine.Base;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BH.Engine.Versioning;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/
        
        private static MethodBase DeserialiseMethodBase(this BsonValue bson, MethodBase value = null)
        {
            if (bson.IsBsonNull)
                return null;
            else if (!bson.IsBsonDocument)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a MethodBase and received " + bson.ToString() + " instead.");
                return value;
            }

            BsonDocument doc = bson.AsBsonDocument;
            string typeName = doc["TypeName"].DeserialiseString();
            string methodName = doc["MethodName"].DeserialiseString();
            string version = doc.Version();

            BsonArray paramArray = doc["Parameters"].AsBsonArray;
            List<string> paramTypesJson = new List<string>();
            foreach (var element in paramArray)
                paramTypesJson.Add(element.DeserialiseString());

            try
            {
                MethodBase method = GetMethod(methodName, typeName, paramTypesJson, version, false);

                if (method == null || method.IsDeprecated())
                {
                    // Try to upgrade through versioning
                    BsonDocument newDoc = Versioning.Convert.ToNewVersion(doc, version);
                    if (newDoc != null && newDoc.Contains("TypeName") && newDoc.Contains("MethodName") && newDoc.Contains("Parameters"))
                        method = GetMethod(newDoc["MethodName"].AsString, newDoc["TypeName"].AsString, newDoc["Parameters"].AsBsonArray.Select(x => x.AsString).ToList(), version, true);
                }

                if (method == null)
                    Base.Compute.RecordError("Method " + methodName + " from " + typeName + " failed to deserialise.");
                return method;
            }
            catch
            {
                Base.Compute.RecordError("Method " + methodName + " from " + typeName + " failed to deserialise.");
                return null;
            }
        }

        /*******************************************/

        private static MethodBase GetMethod(string methodName, string typeName, List<string> paramTypesJson, string version, bool isUpgraded)
        {
            List<Type> types = new List<Type>();
            foreach (string paramType in paramTypesJson)
            {
                BsonDocument paramTypeDoc;
                if (BsonDocument.TryParse(paramType, out paramTypeDoc) && paramTypeDoc.Contains("Name"))
                {
                    Type t = DeserialiseType(paramTypeDoc, null, version, isUpgraded);
                    types.Add(t);
                }
                else
                {
                    Type t = DeserialiseType(paramType, null, version, isUpgraded);
                    types.Add(t);
                }
            }

            MethodBase method = null;
            BsonDocument typeDocument;
            if (BsonDocument.TryParse(typeName, out typeDocument) && typeDocument.Contains("Name"))
            {
                typeName = typeDocument["Name"].AsString.Split(new char[] { ',' }).First();
                Type type = DeserialiseType(typeDocument, null, version, isUpgraded);
                if (type == null)
                    return null;
                method = BH.Engine.Reflection.Create.MethodBase(type, methodName, types.Select(x => x.ToText(true)).ToList()); // type overload
                if (method == null)
                {
                    try
                    {
                        Type[] typesArray = types.ToArray();
                        if (typesArray != null)
                        {
                            if (methodName == ".ctor")
                                method = type.GetConstructor(typesArray);
                            else
                                method = type.GetMethod(methodName, typesArray);
                        }
                    }
                    catch { }
                }

                if (method != null)
                    return method;
            }

            return method;
        }

        /*******************************************/
    }
}


