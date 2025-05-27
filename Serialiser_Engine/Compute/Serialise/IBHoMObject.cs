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
using MongoDB.Bson.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static void Serialise(this IBHoMObject value, BsonDocumentWriter writer, Type targetType)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            HashSet<string> toIgnore = new HashSet<string>();
            if (value.Fragments != null && value.Fragments.Count > 0)
                toIgnore.Add("Fragments");
            if (value.Tags != null && value.Tags.Count > 0)
                toIgnore.Add("Tags");
            if (value.CustomData != null && value.CustomData.Count > 0)
                toIgnore.Add("CustomData");

            if (value is IDynamicObject)
            {
                SerialiseDynamicObject(value as IDynamicObject, writer, targetType, toIgnore);
                return;
            }

            writer.WriteStartDocument();

            writer.WriteName("_t");
            writer.WriteString(value.GetType().FullName);

            foreach (PropertyInfo prop in value.GetType().GetProperties())
            {
                if(!toIgnore.Contains(prop.Name))
                {
                    writer.WriteName(prop.Name);
                    ISerialise(prop.GetValue(value), writer, prop.PropertyType);
                }
                
            }
            writer.WriteEndDocument();
        }

    }
}


