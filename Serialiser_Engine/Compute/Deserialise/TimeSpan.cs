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
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/
        
        private static TimeSpan DeserialiseTimeSpan(this BsonValue bson, TimeSpan value = default(TimeSpan))
        {
            bson = ExtractValue(bson);

            if (!bson.IsString)
            { 
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a timespan and received " + bson.ToString() + " instead.");
                return value;
            }

            if (!TimeSpan.TryParse(bson.AsString, out value))
            {
                BH.Engine.Base.Compute.RecordError("Failed to convert " + bson.AsString + " to a TimeSpan.");
            }
            return value;
        }

        /*******************************************/
    }
}


