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

using BH.Engine.Versioning;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/
        
        private static object DeserialiseDeprecate(this BsonDocument doc, string version, bool isUpgraded)
        {
            if (string.IsNullOrEmpty(version))
                version = doc.Version();

            if (!isUpgraded && TryUpgrade(doc, version, out object upgrade))
            {
                return upgrade;
            }
            else
            {
                return DeserialiseDeprecatedCustomObject(doc, version);
            }
            
        }

        /*******************************************/

        private static CustomObject DeserialiseDeprecatedCustomObject(this BsonDocument doc, string version, bool raiseError = true)
        {
            if(raiseError)
                Engine.Base.Compute.RecordError($"The type {doc["_t"]} from version {(string.IsNullOrEmpty(version) ? "unknown" : version)} is unknown -> data returned as custom objects.");

            CustomObject customObj = DeserialiseCustomObject(doc, null, "", true);
            customObj.CustomData["_t"] = doc["_t"];
            customObj.CustomData["_bhomVersion"] = version;
            return customObj;
        }

        /*******************************************/

        private static bool TryUpgrade<T>(this BsonDocument doc, string version, out T upgraded) where T : class
        {
            if (string.IsNullOrEmpty(version))
                version = doc.Version();

            BsonDocument newDoc = Versioning.Convert.ToNewVersion(doc, version);

            if (newDoc != null && !newDoc.Equals(doc))
            {
                upgraded = IDeserialise(newDoc, "", true) as T;
                return upgraded != null;
            }
            else
            { 
                upgraded = null;
                return false;
            }
        }

        /*******************************************/
    }
}


