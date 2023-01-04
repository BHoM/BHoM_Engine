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
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Drawing;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class ColourSerializer : SerializerBase<Color>
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Color value)
        {
            context.Writer.WriteStartDocument();

            context.Writer.WriteName("_t");
            context.Writer.WriteString("System.Drawing.Color");

            context.Writer.WriteName("A");
            context.Writer.WriteInt32(value.A);

            context.Writer.WriteName("R");
            context.Writer.WriteInt32(value.R);

            context.Writer.WriteName("G");
            context.Writer.WriteInt32(value.G);

            context.Writer.WriteName("B");
            context.Writer.WriteInt32(value.B);

            context.Writer.WriteEndDocument();
        }

        /*******************************************/

        public override Color Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            INameDecoder decoder = Utf8NameDecoder.Instance;

            context.Reader.ReadStartDocument();

            if (context.Reader.ReadName() == "_t")
                context.Reader.ReadString();

            int a = context.Reader.ReadInt32();
            int r = context.Reader.ReadInt32();
            int g = context.Reader.ReadInt32();
            int b = context.Reader.ReadInt32();

            string version = "";
            if (context.Reader.FindElement("_bhomVersion"))
                version = context.Reader.ReadString();

            context.Reader.ReadEndDocument();

            return Color.FromArgb(a, r, g, b);
        }

        /*******************************************/
    }
}




