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
        public static void Serialise(this CustomObject value, BsonDocumentWriter writer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>(value.CustomData);

            writer.WriteStartDocument();

            if (!string.IsNullOrEmpty(value.Name) && value.Name.Length > 0)
            {
                writer.WriteName("Name");
                writer.WriteString(value.Name);
            }

            foreach (KeyValuePair<string, object> kvp in data)
            {
                writer.WriteName(kvp.Key);
                ISerialise(kvp.Value, writer);
            }

            if (value.Tags.Count > 0)
            {
                writer.WriteName("Tags");
                writer.WriteStartArray();
                foreach (string tag in value.Tags)
                    writer.WriteString(tag);
                writer.WriteEndArray();
            }

            writer.WriteName("BHoM_Guid");
            writer.WriteString(value.BHoM_Guid.ToString());

            writer.WriteEndDocument();
        }

        /*******************************************/
    }
}
