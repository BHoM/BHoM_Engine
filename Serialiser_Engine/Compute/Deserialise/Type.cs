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
using BH.oM.Base;
using Humanizer;
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
        
        private static Type DeserialiseType(this BsonValue bson, Type value, string version, bool isUpgraded)
        {
            // Handle the case where the type is represented as a string
            if (bson.IsBsonNull)
                return null;
            else if (bson.IsString)
            {
                Type type = BH.Engine.Base.Create.Type(bson.AsString, true, true);
                if (type != null)
                    return type;
                else
                {
                    if (!isUpgraded)
                    {
                        BsonDocument doc = new BsonDocument();
                        doc["_t"] = "System.Type";
                        doc["Name"] = bson.AsString;
                        BsonDocument newDoc = Versioning.Convert.ToNewVersion(doc, version);
                        if (newDoc != null && newDoc.Contains("Name"))
                            type = GetTypeFromName(newDoc["Name"].AsString);
                    }
                    if (type != null)
                        return type;
                    else
                    {
                        BH.Engine.Base.Compute.RecordError("Failed to convert the string into a type: " + bson.AsString);
                        return value;
                    }
                }
            }

            // Then, if the bson is not a docuemnt, return an error
            if (!bson.IsBsonDocument)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a type and received " + bson.ToString() + " instead.");
                return value;
            }

            // Handle the case where the type is stored as a BsonDocument
            string fullName = "";
            List<Type> genericTypes = new List<Type>();
            List<Type> constraints = new List<Type>();

            foreach (BsonElement element in bson.AsBsonDocument)
            {
                string name = element.Name; ;

                switch (name)
                {
                    case "Name":
                        fullName = element.Value.DeserialiseString();
                        break;
                    case "_bhomVersion":
                        version = element.Value.DeserialiseString();
                        break;
                    case "GenericArguments":
                        genericTypes = element.Value.DeserialiseList(genericTypes, version, isUpgraded);
                        break;
                    case "Constraints":
                        constraints = element.Value.DeserialiseList(constraints, version, isUpgraded);
                        break;
                }
            }

            try
            {
                Type type = GetTypeFromName(fullName);

                if (type == null && fullName != "T" && !isUpgraded)
                {
                    // Try to upgrade through versioning
                    BsonDocument doc = new BsonDocument();
                    doc["_t"] = "System.Type";
                    doc["Name"] = fullName;
                    BsonDocument newDoc = Versioning.Convert.ToNewVersion(doc, version);
                    if (newDoc != null && newDoc.Contains("Name"))
                        type = GetTypeFromName(newDoc["Name"].AsString);
                }

                if (type == null)
                {
                    if (!string.IsNullOrWhiteSpace(fullName) && fullName != "T")  // To mirror the structure of the code above (line 59), we need to check if the fullName is empty.
                        Base.Compute.RecordError("Type " + fullName + " failed to deserialise.");
                }
                else if (type.IsGenericType && type.GetGenericArguments().Length == genericTypes.Where(x => x != null).Count())
                    type = type.MakeGenericType(genericTypes.ToArray());

                return type;
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(fullName) && fullName != "T") // To mirror the structure of the code above (line 59), we need to check if the fullName is empty.
                    Base.Compute.RecordError("Type " + fullName + " failed to deserialise.");
                return null;
            }
        }

        /*******************************************/

        private static Type GetTypeFromName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            Type type = null;
            if (fullName == "T")
                return null;
            if (fullName.IsOmNamespace())
                type = Base.Create.Type(fullName, true, true);
            else if (fullName.IsEngineNamespace())
                type = Base.Create.EngineType(fullName, true, true);
            else
                type = Type.GetType(fullName);

            if (type == null)
            {
                List<Type> types = Base.Create.AllTypes(fullName, true);
                if (types.Count > 0)
                    type = types.OrderBy(x => x.Assembly.FullName).First();
            }

            return type;
        }

        /*******************************************/

    }
}


