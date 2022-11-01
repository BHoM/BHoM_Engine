/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections.Generic;
using System;
using System.Dynamic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Reflection;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class CustomObjectSerializer : SerializerBase<CustomObject>, IBsonPolymorphicSerializer
    {
        /*******************************************/
        /**** Properties                        ****/
        /*******************************************/

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CustomObject value)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(value.CustomData);

            context.Writer.WriteStartDocument();

            if (!string.IsNullOrEmpty(value.Name) && value.Name.Length > 0)
            {
                context.Writer.WriteName("Name");
                context.Writer.WriteString(value.Name);
            }

            foreach (KeyValuePair<string, object> kvp in data)
            {
                context.Writer.WriteName(kvp.Key);
                BsonSerializer.Serialize(context.Writer, kvp.Value);
            }

            if (value.Tags.Count > 0)
            {
                context.Writer.WriteName("Tags");
                context.Writer.WriteStartArray();
                foreach (string tag in value.Tags)
                    context.Writer.WriteString(tag);
                context.Writer.WriteEndArray();
            }

            context.Writer.WriteName("BHoM_Guid");
            context.Writer.WriteString(value.BHoM_Guid.ToString());

            context.Writer.WriteEndDocument();
        }


        /*******************************************/

        public override CustomObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;

            var bsonType = bsonReader.CurrentBsonType;
            if (bsonReader.State != BsonReaderState.Type)
                bsonType = bsonReader.GetCurrentBsonType();

            string message;
            switch (bsonType)
            {
                case BsonType.Document:
                    var dynamicContext = context.With(ConfigureDeserializationContext);
                    bsonReader.ReadStartDocument();
                    CustomObject document = new CustomObject();
                    Dictionary<string, object> dic = document.CustomData;
                    while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                    {
                        var name = bsonReader.ReadName();
                        var value = m_ObjectSerializer.Deserialize(dynamicContext);
                        switch (name)
                        {
                            case "Name":
                                document.Name = value as string;
                                break;
                            case "Tags":
                                document.Tags = new HashSet<string>(((List<object>)value).Cast<string>());
                                break;
                            case "BHoM_Guid":
                                document.BHoM_Guid = new Guid(value as string);
                                break;
                            case "CustomData":
                                while (value is CustomObject)
                                    value = ((CustomObject)value).CustomData;
                                if (value is Dictionary<string, object>)
                                {
                                    Dictionary<string, object> customData = value as Dictionary<string, object>;
                                    if (customData.Count > 0)
                                        dic["CustomData"] = value;
                                }
                                break;
                            case "_bhomVersion":
                                break;
                            default:
                                dic[name] = value;
                                break;
                        }
                    }
                    bsonReader.ReadEndDocument();
                    return document;

                default:
                    message = string.Format("Cannot deserialize a '{0}' from BsonType '{1}'.", BsonUtils.GetFriendlyTypeName(typeof(CustomObject)), bsonType);
                    throw new FormatException(message);
            }
        }

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        protected void ConfigureDeserializationContext(BsonDeserializationContext.Builder builder)
        {
            builder.DynamicDocumentSerializer = this;
            builder.DynamicArraySerializer = m_ListSerializer;
        }
 
        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private readonly IBsonSerializer<List<object>> m_ListSerializer = BsonSerializer.LookupSerializer<List<object>>();
        private static readonly IBsonSerializer<object> m_ObjectSerializer = BsonSerializer.LookupSerializer<object>();


        /*******************************************/
    }
}



