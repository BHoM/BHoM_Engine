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
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static T DeserialiseEnum<T>(this BsonValue bson, ref bool failed, T value, string version, bool isUpgraded) where T : Enum
        {
            if (bson.IsString)
                value = BH.Engine.Base.Compute.ParseEnum<T>(bson.AsString);
            else if (bson.IsBsonDocument)
            {
                Type type = DeserialiseType(bson["TypeName"], ref failed, null, version, isUpgraded);
                if (type != typeof(T))
                {
                    BH.Engine.Base.Compute.RecordError("The type of enum to deserialise doesn't match. Expected " + typeof(T).ToString() + " and got " + type.ToString() + " instead.");
                    failed = true;
                }
                else
                    value = BH.Engine.Base.Compute.ParseEnum<T>(bson["Value"].AsString);
            }
            else if (bson.IsInt32)
                value = (T)(object)bson.AsInt32;
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise an enum and received " + bson.ToString() + " instead.");
                failed = true;
            }

            return value;
        }

        /*******************************************/

        private static Enum DeserialiseEnumTopLevel(this BsonValue bson, ref bool failed, Enum value, string version, bool isUpgraded)
        {
            if (bson.IsBsonDocument)
            {
                Type type = DeserialiseType(bson["TypeName"], ref failed, null, version, isUpgraded);
                return DeserialiseEnum(bson, ref failed, EnsureNotNull(value, type) as dynamic, version, isUpgraded);
            }

            return null;
        }

        /*******************************************/
    }
}
