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
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base.Attributes;
using MongoDB.Bson;

namespace BH.Engine.Versioning
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Provide a string representation of a method as it used for versioning by the PreviousVersion attribute.")]
        [Input("declaringType", "Type in which the method is declared. You can use just the name of the type or include a (part of the) namespace in front of it.")]
        [Input("methodName", "Name of the method. It has to be the exact string. If the method is a constructor, you can leave this input blank.")]
        [Output("keys", "String representation for each method that matches the input filters.")]
        public static List<string> VersioningKey(string declaringType, string methodName = "")
        {
            if (methodName == "")
                methodName = ".ctor";

            return Engine.Base.Query.AllMethodList()
                .Where(x => x.Name == methodName && x.DeclaringType.FullName.EndsWith(declaringType))
                .Select(x => x.VersioningKey())
                .ToList();
        }

        /***************************************************/

        [Description("Provide a short string representation of a BsonDocument as it is used for versioning.")]
        [Input("document", "BsonDocument that needs to be represented as a short string.")]
        [Output("key", "Short string representation of the document.")]
        public static string VersioningKey(BsonDocument document)
        {
            if (!document.Contains("_t"))
                return "Unknown type";
            string typeString = document["_t"].AsString;

            if (typeString == "System.Reflection.MethodBase")
                return GetMethodKey(document);
            else if (typeString == "System.Type")
                return document["Name"].AsString.Split(',').First();
            else
                return typeString;
        }


        /***************************************************/
        /**** Private Methods                           ****/
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
    }
}





