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

using BH.Engine.Base;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/
        public static IImmutable DeserialiseImmutable(this BsonValue bson, ref bool failed, Type targetType)
        {
            if (!bson.IsBsonDocument)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise an Immutable object and received " + bson.ToString() + " instead.");
                failed = true;
                return null;
            }

            BsonDocument doc = bson.AsBsonDocument;
            ConstructorInfo[] constructors = targetType.GetConstructors();
            if (constructors.Length > 0)
            {
                var ctor = targetType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
                var parameters = ctor.GetParameters();

                var matches = parameters
                    .GroupJoin(doc,
                        parameter => parameter.Name,
                        property => property.Name,
                        (parameter, props) => new { Parameter = parameter, Properties = props },
                        StringComparer.OrdinalIgnoreCase);

                if (matches.All(m => m.Properties.Count() == 1))
                {
                    List<object> arguments = new List<object>();
                    foreach (var match in matches)
                        arguments.Add(IDeserialise(match.Properties.First().Value, match.Parameter.ParameterType, ref failed));
                    IImmutable result = ctor.Invoke(arguments.ToArray()) as IImmutable;

                    if (result != null)
                        return DeserialiseIObject(bson, ref failed, result as dynamic);
                }
            }

            return null;

        }

        /*******************************************/
    }
}
