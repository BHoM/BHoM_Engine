﻿/*
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
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/
        public static void Serialise<T1, T2>(this Tuple<T1, T2> value, BsonDocumentWriter writer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();
            value.Item1.ISerialise(writer);
            value.Item2.ISerialise(writer);
            writer.WriteEndArray();
        }

        /*******************************************/
        public static void Serialise<T1, T2, T3>(this Tuple<T1, T2, T3> value, BsonDocumentWriter writer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();
            value.Item1.ISerialise(writer);
            value.Item2.ISerialise(writer);
            value.Item3.ISerialise(writer);
            writer.WriteEndArray();
        }

        /*******************************************/
        public static void Serialise<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> value, BsonDocumentWriter writer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();
            value.Item1.ISerialise(writer);
            value.Item2.ISerialise(writer);
            value.Item3.ISerialise(writer);
            value.Item4.ISerialise(writer);
            writer.WriteEndArray();
        }

        /*******************************************/
        public static void Serialise<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> value, BsonDocumentWriter writer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();
            value.Item1.ISerialise(writer);
            value.Item2.ISerialise(writer);
            value.Item3.ISerialise(writer);
            value.Item4.ISerialise(writer);
            value.Item5.ISerialise(writer);
            writer.WriteEndArray();
        }

        /*******************************************/
    }
}