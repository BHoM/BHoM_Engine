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

using BH.Engine.Versioning;
using BH.oM.Base;
using MongoDB.Bson.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/
        private static void Serialise(this Type value, BsonDocumentWriter writer, Type targetType)
        {
            writer.WriteStartDocument();

            writer.WriteName("_t");
            writer.WriteString(typeof(Type).ToString());

            if (value == null)
            {
                //Using context.Writer.WriteNull() leads to problem in the deserialisation. 
                //We think that BSON think that the types will always be types to be deserialised rather than properties of objects.
                //If that type is null bson throws an exception believing that it wont be able to deserialise an object of type null, while for this case it is ment to be used as a property.
                writer.WriteName("Name");
                writer.WriteString("");
            }
            else
            {
                // Handle the case of generic types
                Type[] generics = new Type[] { };
                if (value.IsGenericType)
                {
                    generics = value.GetGenericArguments();
                    value = value.GetGenericTypeDefinition();
                }

                // Write the name of the type
                writer.WriteName("Name");
                if (value.IsGenericParameter)
                {
                    writer.WriteString("T");

                    Type[] constraints = value.GetGenericParameterConstraints();
                    if (constraints.Length > 0)
                    {
                        writer.WriteName("Constraints");
                        writer.WriteStartArray();
                        foreach (Type constraint in constraints)
                        {
                            // T : IComparable<T> creates an infinite loop. Thankfully, that's the only case where a type constrained by itself even makes sense
                            if (constraint.Name == "IComparable`1" && constraint.GenericTypeArguments.FirstOrDefault() == value)
                                typeof(IEnumerable).Serialise(writer, typeof(Type));
                            else
                                constraint.Serialise(writer, typeof(Type));
                        }

                        writer.WriteEndArray();
                    }
                }
                else if (value.Namespace.StartsWith("BH.oM"))
                    writer.WriteString(value.FullName);
                else if (value.AssemblyQualifiedName != null)
                    writer.WriteString(value.AssemblyQualifiedName);
                else
                    writer.WriteString(""); //TODO: is that even possible?


                // Add additional information for generic types
                if (generics.Length > 0)
                {
                    writer.WriteName("GenericArguments");
                    writer.WriteStartArray();
                    foreach (Type arg in generics)
                        arg.Serialise(writer, typeof(Type));
                    writer.WriteEndArray();
                }

                // Add the BHoM verion 
                writer.AddVersion();
            }

            writer.WriteEndDocument();
        }

        /*******************************************/
    }
}
