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

using BH.Engine.Reflection;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using BH.Engine.Versioning;
using System;
using System.Reflection;
using BH.Engine.Serialiser.Objects;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class DeprecatedSerializer : MongoDB.Bson.Serialization.Serializers.ObjectSerializer
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteNull();
        }

        /***************************************************/

        public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            // This is where we can call the Version_Engine to return the new type string from the old on if exist
            BsonReaderBookmark bookmark = context.Reader.GetBookmark();
            IBsonSerializer bSerializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
            BsonDocument doc = bSerializer.Deserialize(context, args) as BsonDocument;
            BsonDocument newDoc = Versioning.Convert.ToNewVersion(doc);

            if (newDoc == null || newDoc.Equals(doc))
            {
                string newType = newDoc["_t"].AsString;
                if (newType.Contains("[["))
                {
                    // If the object is of a generic type, try one last time by ensuring that the assembly name is provided
                    try
                    {
                        string baseType = newType.Split(new char[] { '[' }).First();
                        List<Type> matchingTypes = BH.Engine.Base.Create.AllTypes(baseType, true);
                        if (matchingTypes.Count == 1)
                        {
                            string assemblyName = matchingTypes.First().Assembly?.FullName;
                            newDoc["_t"] += "," + assemblyName;
                            return Convert.FromBson(newDoc);
                        }
                    }
                    catch { }
                }

                Engine.Base.Compute.RecordWarning("The type " + doc["_t"] + " is unknown -> data returned as custom objects.");
                context.Reader.ReturnToBookmark(bookmark);
                IBsonSerializer customSerializer = BsonSerializer.LookupSerializer(typeof(CustomObject));
                return customSerializer.Deserialize(context, args);
            }   
            else
            {
                return Convert.FromBson(newDoc);
            }
                
        }

        /***************************************************/

    }
}




