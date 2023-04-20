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
        public static CustomObject DeserialiseCustomObject(this BsonValue bson, ref bool failed, CustomObject value = null)
        {
            if (bson.IsBsonNull)
                return null;
            else if (!bson.IsBsonDocument)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a Custom object and received " + bson.ToString() + " instead.");
                failed = true;
                return new CustomObject();
            }

            if (value == null)
                value = new CustomObject();

            BsonDocument doc = bson.AsBsonDocument;

            foreach (BsonElement element in doc)
            {
                switch (element.Name)
                {
                    case "_t":
                        // ignore
                        break;
                    case "_bhomVersion":
                        // ignore
                        break;
                    case "Name":
                        value.Name = element.Value.DeserialiseString(ref failed, value.Name);
                        break;
                    case "BHoM_Guid":
                        value.BHoM_Guid = element.Value.DeserialiseGuid(ref failed, value.BHoM_Guid);
                        break;
                    case "Tags":
                        value.Tags = element.Value.DeserialiseHashSet(ref failed, value.Tags);
                        break;
                    case "Fragments":
                        value.Fragments = element.Value.DeserialiseFragmentSet(ref failed, value.Fragments);
                        break;
                    case "CustomData":
                        value.CustomData = element.Value.DeserialiseDictionary(ref failed, value.CustomData);
                        break;
                    default:
                        value.CustomData[element.Name] = element.Value.IDeserialise(ref failed);
                        break;
                }
            }

            return value;
        }

        /*******************************************/
    }
}
