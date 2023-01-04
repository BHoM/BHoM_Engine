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

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Data;
using System.Collections.Generic;
using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;


namespace BH.Engine.Serialiser.BsonSerializers
{
    public class DataTableSerialiser : SerializerBase<DataTable>
    {

        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DataTable value)
        {
            context.Writer.WriteStartArray();
            foreach (DataRow dr in value.Rows)
            {
                var dictionary = dr.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col.ColumnName]);
                BsonSerializer.Serialize(context.Writer, dictionary);
            }
            context.Writer.WriteEndArray();
        }

        /*******************************************/

        public override DataTable Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;

            var bsonType = bsonReader.CurrentBsonType;
            if (bsonReader.State != BsonReaderState.Type)
                bsonType = bsonReader.GetCurrentBsonType();

            string message;
            switch (bsonType)
            {
                case BsonType.Array:
                    bsonReader.ReadStartArray();

                    DataTable table = new DataTable();

                    bool initialised = false;

                    while (bsonReader.State != BsonReaderState.EndOfArray && bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                    {
                        Dictionary<string, object> rowData = BsonSerializer.Deserialize<Dictionary<string, object>>(bsonReader);
                        if (!initialised)
                        {
                            foreach (var kvp in rowData)
                            {
                                table.Columns.Add(new DataColumn(kvp.Key, kvp.Value.GetType()));
                            }
                            initialised = true;
                        }

                        DataRow row = table.NewRow();

                        foreach (var kvp in rowData)
                        {
                            row[kvp.Key] = kvp.Value;
                        }
                        table.Rows.Add(row);
                    }

                    bsonReader.ReadEndArray();

                    return table;

                default:
                    message = string.Format("Cannot deserialize a '{0}' from BsonType '{1}'.", BsonUtils.GetFriendlyTypeName(typeof(DataTable)), bsonType);
                    throw new FormatException(message);
            }

        }

        /*******************************************/
    }
}




