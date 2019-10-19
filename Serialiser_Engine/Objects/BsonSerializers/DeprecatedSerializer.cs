/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class DeprecatedSerializer : MongoDB.Bson.Serialization.Serializers.ObjectSerializer
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public DeprecatedSerializer()
        {
            _discriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(IDeprecated));
        }


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
            // First try to recover the object type
            IBsonReader reader = context.Reader;
            BsonReaderBookmark bookmark = reader.GetBookmark();
            Type actualType = typeof(CustomObject);
            try
            {
                actualType = _discriminatorConvention.GetActualType(reader, typeof(object));
            }
            catch
            {
                // This is were we can call the Version_Engine to return the new type string from the old one if exists
                string recordedType = GetCurrentTypeValue(reader);

                // If failed, return Custom object
                context.Reader.ReturnToBookmark(bookmark);
                Engine.Reflection.Compute.RecordWarning("The type " + recordedType + " is unknown -> data returned as custom objects.");
                IBsonSerializer customSerializer = BsonSerializer.LookupSerializer(typeof(CustomObject));
                return customSerializer.Deserialize(context, args);
            }

            // This is where we can call the Version_Engine to return the new type string from the old on if exist
            IBsonSerializer bSerializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
            BsonDocument doc = bSerializer.Deserialize(context, args) as BsonDocument;
            BsonDocument newDoc = Versioning.Convert.ToNewVersion(doc);
            return BH.Engine.Serialiser.Convert.FromBson(newDoc);
        }


        /***************************************************/
        /**** Private Helper Methods                    ****/
        /***************************************************/

        protected string GetCurrentTypeValue(IBsonReader reader)
        {
            // You gotta do what you gotta do. I couldn't really find an easy way to get the type as a string without spending time in the source code of Mongo
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            Type type = reader.GetType();

            FieldInfo field = type.GetField("_currentValue", bindFlags);
            if (field == null)
                return "";

            BsonValue value = field.GetValue(reader) as BsonValue;
            if (value == null)
                return "";

            return value.AsString;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private readonly IDiscriminatorConvention _discriminatorConvention;

        /*******************************************/
    }


}
