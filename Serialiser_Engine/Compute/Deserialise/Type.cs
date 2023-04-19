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
        public static Type DeserialiseType(this BsonValue bson, ref bool failed, Type value = null)
        {
            // Handle the case where the type is represented as a string
            if (bson.IsString)
            {
                Type type = BH.Engine.Base.Create.Type(bson.AsString);
                if (type != null)
                    return type;
                else
                {
                    failed = true;
                    BH.Engine.Base.Compute.RecordError("Failed to convert the string into a type: " + bson.AsString);
                    return value;
                }
            }

            // Then, if the bson is not a docuemnt, return an error
            if (!bson.IsBsonDocument)
            {
                failed = true;
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a type and received " + bson.ToString() + " instead.");
                return value;
            }

            // Handle the case where the type is stored as a BsonDocument
            string fullName = "";
            string version = "";
            List<Type> genericTypes = new List<Type>();
            List<Type> constraints = new List<Type>();

            foreach (BsonElement element in bson.AsBsonDocument)
            {
                string name = element.Name; ;

                switch (name)
                {
                    case "Name":
                        fullName = element.Value.DeserialiseString(ref failed);
                        break;
                    case "_bhomVersion":
                        version = element.Value.DeserialiseString(ref failed);
                        break;
                    case "GenericArguments":
                        genericTypes = element.Value.DeserialiseList(ref failed, genericTypes);
                        break;
                    case "Constraints":
                        constraints = element.Value.DeserialiseList(ref failed, constraints);
                        break;
                }
            }

            try
            {
                Type type = GetTypeFromName(fullName);

                if (type == null && fullName != "T")
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
        /**** Private Methods                   ****/
        /*******************************************/

        private static Type GetTypeFromName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            Type type = null;
            if (fullName == "T")
                return null;
            if (fullName.StartsWith("BH.oM"))
                type = Base.Create.Type(fullName, true);
            else if (fullName.StartsWith("BH.Engine"))
                type = Base.Create.EngineType(fullName, true);
            else
                type = Type.GetType(fullName);

            if (type == null)
            {
                List<Type> types = Base.Create.AllTypes(fullName, true);
                if (types.Count > 0)
                    type = types.First();
            }

            return type;
        }

        /*******************************************/
    }
}
