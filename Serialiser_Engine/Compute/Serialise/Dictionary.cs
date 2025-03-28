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

using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static void Serialise<T>(this IDictionary<string, T> value, BsonDocumentWriter writer, Type targetType)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartDocument();
            if (value.GetType() != targetType)
            {
                writer.WriteName("_t");
                writer.WriteString(value.GetType().FullName);
            }
            foreach (var kvp in value)
            {
                writer.WriteName(kvp.Key);
                ISerialise(kvp.Value, writer, typeof(T));
            }
            writer.WriteEndDocument();

        }

        /*******************************************/

        private static void Serialise<TK, TV>(this IDictionary<TK, TV> value, BsonDocumentWriter writer, Type targetType)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            WriteAsDocumentIfUnmatchingType(value, writer, targetType, () =>
            {
                writer.WriteStartArray();
                foreach (var kvp in value)
                {
                    writer.WriteStartDocument();
                    writer.WriteName("k");
                    ISerialise(kvp.Key, writer, typeof(TK));
                    writer.WriteName("v");
                    ISerialise(kvp.Value, writer, typeof(TV));
                    writer.WriteEndDocument();
                }
                writer.WriteEndArray();
            });
        }

        /*******************************************/
    }
}


