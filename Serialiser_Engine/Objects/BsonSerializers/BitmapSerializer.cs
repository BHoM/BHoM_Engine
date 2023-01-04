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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BH.Engine.Serialiser.BsonSerializers
{
    public class BitmapSerializer : SerializerBase<Bitmap>
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Bitmap value)
        {
            IBsonWriter bsonWriter = context.Writer;

            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                var stream = new MemoryStream();
                value.Save(stream, value.RawFormat); // Will work for most formats: bmp, png, jepg, tiff but not all
                var bytes = stream.ToArray();
                BsonBinaryData data = new BsonBinaryData(bytes);

                bsonWriter.WriteBinaryData(data); 
            }
        }

        /*******************************************/

        public override Bitmap Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }

            BsonBinaryData data = context.Reader.ReadBinaryData();
            var stream = new MemoryStream(data.Bytes);
            return new Bitmap(stream);
        }

        /*******************************************/
    }
}

// Keeping this for a while as the alternative I explored. 
// It is more complex and hand-made (so I kept the above solution instead) but was having single difference between orginal and deserialsed : the palette.flags that I couldn't fix
/*
namespace BH.Engine.Serialiser.BsonSerializers
{
    public class BitmapSerializer : SerializerBase<Bitmap>, IBsonPolymorphicSerializer
    {

        public bool IsDiscriminatorCompatibleWithObjectSerializer { get; } = true;


        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Bitmap value)
        {
            IBsonWriter bsonWriter = context.Writer;
            bsonWriter.WriteStartDocument();

            var discriminator = m_DiscriminatorConvention.GetDiscriminator(typeof(object), typeof(Bitmap));
            bsonWriter.WriteName(m_DiscriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(context, discriminator);

            if (value == null)
            {
                bsonWriter.WriteName("Height");
                bsonWriter.WriteInt32(0);
            }
            else
            {
                // Get the bitmap data from the image
                BitmapData bitmapData = value.LockBits(
                        new Rectangle(new Point(), value.Size),
                        ImageLockMode.ReadOnly,
                        value.PixelFormat
                );

                int byteCount = bitmapData.Stride * value.Height;
                byte[] bitmapBytes = new byte[byteCount];
                Marshal.Copy(bitmapData.Scan0, bitmapBytes, 0, byteCount);
                value.UnlockBits(bitmapData);

                // Serialise the Bitmap
                bsonWriter.WriteName("Height");
                bsonWriter.WriteInt32(value.Height);

                bsonWriter.WriteName("Width");
                bsonWriter.WriteInt32(value.Width);

                bsonWriter.WriteName("Format");
                bsonWriter.WriteString(Enum.GetName(typeof(PixelFormat), value.PixelFormat));

                bsonWriter.WriteName("Flags");
                bsonWriter.WriteInt32(value.Palette.Flags);

                bsonWriter.WriteName("Data");
                bsonWriter.WriteBinaryData(bitmapBytes);
            }

            bsonWriter.WriteEndDocument();
        }


        public override Bitmap Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            bsonReader.ReadStartDocument();

            string text = bsonReader.ReadName();
            if (text == m_DiscriminatorConvention.ElementName)
                bsonReader.SkipValue();

            bsonReader.ReadName();
            int height = bsonReader.ReadInt32();

            if (height == 0)
                return null;

            bsonReader.ReadName();
            int width = bsonReader.ReadInt32();

            bsonReader.ReadName();
            string formatName = bsonReader.ReadString();

            bsonReader.ReadName();
            int flags = bsonReader.ReadInt32();

            bsonReader.ReadName();
            BsonBinaryData binaryData = bsonReader.ReadBinaryData();
            byte[] data = binaryData.Bytes;

            bsonReader.ReadEndDocument();

            try
            {
                PixelFormat format = (PixelFormat)Enum.Parse(typeof(PixelFormat), formatName);
                Bitmap bitmap = new Bitmap(width, height, format);

                BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle(new Point(), bitmap.Size),
                    ImageLockMode.WriteOnly,
                    format
                );

                Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
                bitmap.UnlockBits(bitmapData);

                // This doesn't fix the issue with flags. Flags actaully seems to be changing live (at least in VS while debugging) as well so not sure what can be done 
                if (bitmap.Palette != null)
                    typeof(ColorPalette).GetField("flags", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(bitmap.Palette, flags);

                return bitmap;
            }
            catch
            {
                Base.Compute.RecordError("Bitmap failed to deserialise.");
                return null;
            }
        }


        private IDiscriminatorConvention m_DiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));

    }
}
*/



