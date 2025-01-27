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

using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        [Description("Used to support ToJson, not recomended to be used in isolation.")]
        public static void ISerialise(this object value, BsonDocumentWriter writer)
        {
            if (value == null)
                writer.WriteNull();
            else
                Serialise(value as dynamic, writer, typeof(object));
        }

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static void ISerialise(this object value, BsonDocumentWriter writer, Type targetType)
        {
            if (value == null)
                writer.WriteNull();
            else
                Serialise(value as dynamic, writer, targetType);
        }


        /*******************************************/
        /**** Fallback Methods                  ****/
        /*******************************************/

        private static void Serialise(this object value, BsonDocumentWriter writer, Type targetType)
        {
            if (value == null || value.GetType() == typeof(System.DBNull))
                writer.WriteNull();
            else if (value.GetType() == typeof(object))
            {
                writer.WriteStartDocument();
                writer.WriteEndDocument();
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Object of type " + value.GetType().ToString() + " cannot be serialised.");
                writer.WriteNull();
            } 
        }

        /*******************************************/
        /**** Private Methods - Support         ****/
        /*******************************************/

        private static void WriteAsDocumentIfUnmatchingType(object value, BsonDocumentWriter writer, Type targetType, Action action)
        {
            bool asDocument = value.GetType() != targetType;

            if (asDocument)
            {
                writer.WriteStartDocument();
                writer.WriteName("_t");
                writer.WriteString(value.GetType().FullName);
                writer.WriteName("_v");
            }

            action.Invoke();

            if (asDocument)
                writer.WriteEndDocument();

        }

        /*******************************************/
    }
}



