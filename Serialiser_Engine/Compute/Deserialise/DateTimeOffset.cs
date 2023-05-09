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
        /**** Public Methods                    ****/
        /*******************************************/
        private static DateTimeOffset DeserialiseDateTimeOffset(this BsonValue bson, ref bool failed, DateTimeOffset value = default(DateTimeOffset))
        {
            bson = ExtractValue(bson);
            BsonArray array;
            if (bson.IsBsonArray)
            {
                array = bson.AsBsonArray;
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a DateTimeOffset and received " + bson.ToString() + " instead.");
                failed = true;
                return value;
            }

            if (array.Count != 2)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a DateTimeOffset and received " + bson.ToString() + " instead. This should be represented with an array of two values.");
                failed = true;
                return value;
            }

            return new DateTimeOffset(array[0].ToInt64(), new TimeSpan(TimeSpan.TicksPerMinute * array[1].ToInt64()));

        }

        /*******************************************/
    }
}
