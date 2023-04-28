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

using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        private static void Serialise<T>(this IDictionary<string, T> value, BsonDocumentWriter writer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartDocument();
            foreach (var kvp in value)
            {
                writer.WriteName(kvp.Key);
                ISerialise(kvp.Value, writer);
            }
            writer.WriteEndDocument();

        }

        /*******************************************/

        private static void Serialise<TK, TV>(this IDictionary<TK, TV> value, BsonDocumentWriter writer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (writer.SerializationDepth == 0)
            {
                writer.WriteStartDocument();
                writer.WriteName("_t");
                writer.WriteString(value.GetType().FullName);
                writer.WriteName("_v");
            }
            
            writer.WriteStartArray();
            foreach (var kvp in value)
            {
                writer.WriteStartDocument();
                writer.WriteName("k");
                ISerialise(kvp.Key, writer);
                writer.WriteName("v");
                ISerialise(kvp.Value, writer);
                writer.WriteEndDocument();
            }
            writer.WriteEndArray();

            if (writer.SerializationDepth == 0)
                writer.WriteEndDocument();
        }

        /*******************************************/
    }
}
