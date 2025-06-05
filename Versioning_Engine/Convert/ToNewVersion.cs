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

using BH.Engine.Versioning.Objects;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Versioning;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Versioning
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Upgrades the input json string so it aligns with the current version of the BHoM.")]
        [Input("json", "Json string that needs to be upgraded.")]
        [Output("upgraded", "Upgraded json string that is compatible with the current version of the BHoM.")]
        public static string ToNewVersion(string json)
        {
            BsonDocument document;
            if (BsonDocument.TryParse(json, out document))
            {
                BsonDocument newDocument = ToNewVersion(document);
                if (newDocument != null)
                    return newDocument.ToJson();
                else
                    return json;
            }
            else
            {
                Base.Compute.RecordError("The string provided is not a supported json format");
                return json;
            }
        }

        /***************************************************/

        [Description("Upgrades the input Bson document so it aligns with the current version of the BHoM.")]
        [Input("document", "Bson document that needs to be upgraded.")]
        [Output("upgraded", "Upgraded Bson document that is compatible with the current version of the BHoM.")]
        public static BsonDocument ToNewVersion(this BsonDocument document, string version = "")
        {
            if (document == null)
                return null;

            if (document.Contains("_t") && document["_t"].ToString() == "DBNull")
                return null;

            if (version == "")
                version = document.Version();

            // Keep a description of the old document
            string oldDocument = Compute.VersioningKey(document);
            string noUpdateMessage = null;
            bool wasUpdated = false;

            // Get the list of upgraders to call
            List<string> versions = Query.UpgradersToCall(version);

            lock (m_versioningLock)
            {
                // Call all the upgraders in sequence
                for (int i = 0; i < versions.Count; i++)
                {
                    BsonDocument result = null;
                    try
                    {
                        Converter converter = Query.Converter(versions[i]);
                        if (converter != null)
                            result = Modify.Upgrade(document, converter);    //Upgrade
                    }
                    catch (NoUpdateException e)
                    {
                        Engine.Base.Compute.RecordError(e.Message);
                        noUpdateMessage = e.Message;
                        result = null;
                    }
                    catch (Exception e)
                    {
                        Engine.Base.Compute.RecordError(e, "BHoMUpgrader exception:");
                    }

                    if (result != null && document != result)
                    {
                        wasUpdated = true;
                        document = result;
                    }
                }
            }

            if (wasUpdated || noUpdateMessage != null)
            {
                string newDocument = noUpdateMessage != null ? null : Compute.VersioningKey(document);
                string newVersion = Engine.Base.Query.BHoMVersion();
                string oldVersion = string.IsNullOrWhiteSpace(version) ? "?.?" : version;
                string message = noUpdateMessage ?? $"{oldDocument} from version {oldVersion} has been upgraded to {newDocument} (version {newVersion})";

                BH.Engine.Base.Compute.RecordEvent(new VersioningEvent
                {
                    OldDocument = oldDocument,
                    NewDocument = newDocument,
                    OldVersion = oldVersion,
                    NewVersion = newVersion,
                    Message = message
                });
            }
            return document;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static object m_versioningLock = new object();

        /***************************************************/
    }
}




