/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

        private static MethodBase GetMethod(string methodName, string typeName, List<string> paramTypeNames, string version, bool isUpgraded)
        {
            MethodBase method = null;
            BsonDocument typeDocument;
            if (BsonDocument.TryParse(typeName, out typeDocument) && typeDocument.Contains("Name"))
            {
                typeName = typeDocument["Name"].AsString.Split(new char[] { ',' }).First();
                MakeSureAssemblyIsLoadedForType(typeName);

                for (int i = 0; i < paramTypeNames.Count; i++)
                {
                    if (paramTypeNames[i].StartsWith("{"))
                    {
                        BsonDocument paramTypeDoc;
                        if (BsonDocument.TryParse(paramTypeNames[i], out paramTypeDoc))
                            paramTypeNames[i] = OldTypeDocToText(paramTypeDoc);
                    }
                }

                // Try to create a BHoM method
                method = BH.Engine.Reflection.Create.MethodBase(typeName, methodName, paramTypeNames); // type overload

                // If this fails using type names, revert to deserialising the types themselves.
                // This should not happen on code that is BHoM compliant and doesn't expose external types publicly.
                if (method == null)
                {
                    Type type = DeserialiseType(typeDocument, null, version, isUpgraded);
                    if (type != null)
                    {
                        // Prefer the Type overload as it also covers instance and inherited methods. It falls back to the name matching if needed
                        List<Type> paramTypes = paramTypeNames.Select(x => BH.Engine.Base.Create.Type(x, true)).ToList();
                        if (paramTypes.All(x => x != null))
                            method = BH.Engine.Reflection.Create.MethodBase(type, methodName, paramTypes);
                        else
                            method = BH.Engine.Reflection.Create.MethodBase(type, methodName, paramTypeNames);
                    }
                }
            }

            return method;
        }

        /*******************************************/

        private static string OldTypeDocToText(BsonDocument typeDoc)
        {
            string name = typeDoc.Contains("Name") ? typeDoc["Name"].AsString : "";
            string fullName = StripAssemblyQualification(name);

            int backtickIndex = fullName.IndexOf('`');
            if (backtickIndex < 0)
                return fullName;

            if (typeDoc.Contains("GenericArguments"))
            {
                string baseName = fullName.Substring(0, backtickIndex);
                List<string> args = typeDoc["GenericArguments"].AsBsonArray
                    .Select(x => OldTypeDocToText(x.AsBsonDocument))
                    .ToList();

                return baseName + "<" + string.Join(", ", args) + ">";
            }
            else
            {
                // Generic arguments embedded inline in Name (e.g. by-ref/out parameters of a generic type,
                // such as "List`1[[System.Double, ...]]&"). ToText never expands these - for such types
                // IsGenericType is false, so it falls back to Namespace + "." + Name, and Name keeps the
                // backtick/arity plus any by-ref/array suffix but drops the bracketed argument info.
                int arityEnd = backtickIndex + 1;
                while (arityEnd < fullName.Length && char.IsDigit(fullName[arityEnd]))
                    arityEnd++;

                string baseName = fullName.Substring(0, arityEnd);
                string remainder = fullName.Substring(arityEnd);
                return baseName + StripEmbeddedGenericArgBrackets(remainder);
            }
        }

        /*******************************************/

        private static string StripAssemblyQualification(string rawName)
        {
            int depth = 0;
            for (int i = 0; i < rawName.Length; i++)
            {
                char c = rawName[i];
                if (c == '[')
                    depth++;
                else if (c == ']')
                    depth--;
                else if (c == ',' && depth == 0)
                    return rawName.Substring(0, i);
            }
            return rawName;
        }

        /*******************************************/

        private static string StripEmbeddedGenericArgBrackets(string remainder)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            while (i < remainder.Length)
            {
                if (remainder[i] == '[' && i + 1 < remainder.Length && remainder[i + 1] == '[')
                {
                    int depth = 0;
                    int j = i;
                    for (; j < remainder.Length; j++)
                    {
                        if (remainder[j] == '[')
                            depth++;
                        else if (remainder[j] == ']')
                        {
                            depth--;
                            if (depth == 0)
                            {
                                j++;
                                break;
                            }
                        }
                    }
                    i = j;
                }
                else
                {
                    sb.Append(remainder[i]);
                    i++;
                }
            }
            return sb.ToString();
        }

        /*******************************************/
    }
}



