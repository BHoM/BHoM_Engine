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

using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Base.Attributes;
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
        
        private static void SerialiseDynamicObject(this IDynamicObject value, BsonDocumentWriter writer, Type targetType)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartDocument();

            writer.WriteName("_t");
            writer.WriteString(value.GetType().FullName);

            if (value is IDynamicPropertyProvider)
            {
                foreach (string prop in value.PropertyNames())
                {
                    object result;
                    if (BH.Engine.Base.Compute.TryRunExtensionMethod(value, "GetProperty", new object[] { prop }, out result))
                    {
                        writer.WriteName(prop);
                        ISerialise(result, writer, typeof(object));
                    }
                    else
                    {
                        Base.Compute.RecordWarning($"Failed to collect property {prop} when serialising {value.GetType().Name}. This property will be ignored.");
                    }
                } 
            }
            else
            {
                foreach (PropertyInfo prop in value.GetType().GetProperties())
                {
                    if (prop.GetCustomAttribute<DynamicPropertyAttribute>() != null && typeof(IDictionary).IsAssignableFrom(prop.PropertyType))
                    {
                        Type[] genericArguments = prop.PropertyType.GenericTypeArguments;
                        if (genericArguments.Length != 2 || !prop.PropertyType.GenericTypeArguments.First().IsEnum)
                        {
                            writer.WriteName(prop.Name);
                            ISerialise(prop.GetValue(value), writer, prop.PropertyType);
                            BH.Engine.Base.Compute.RecordWarning($"A dynamic property container of type {prop.PropertyType.ToString()} is not supported. It will be serialised as a regular property.");
                        }
                        else
                        {
                            Type valueType = genericArguments[1];
                            IDictionary dic = prop.GetValue(value) as IDictionary;
                            foreach (Enum key in dic.Keys.OfType<Enum>())
                            {
                                writer.WriteName(key.ToString());
                                ISerialise(dic[key], writer, valueType);
                            }
                        }
                    }
                    else
                    {
                        writer.WriteName(prop.Name);
                        ISerialise(prop.GetValue(value), writer, prop.PropertyType);
                    }
                }
            }

            writer.WriteEndDocument();
        }

        /*******************************************/
    }
}


