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

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class IntPtrSerializer : SerializerBase<IntPtr>
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, IntPtr value)
        {
            context.Writer.WriteInt64(value.ToInt64());
        }

        /*******************************************/

        public override IntPtr Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            IBsonReader bsonReader = context.Reader;

            if (bsonReader.CurrentBsonType == BsonType.Int32)
                return new IntPtr(bsonReader.ReadInt32());
            else if (bsonReader.CurrentBsonType == BsonType.Int64)
                return new IntPtr(bsonReader.ReadInt64());
            else
                return new IntPtr();
        }

        /*******************************************/
    }
}




