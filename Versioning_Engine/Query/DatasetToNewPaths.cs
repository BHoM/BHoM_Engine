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
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace BH.Engine.Versioning
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Fetches paths going from an old dataset path to a new.")]
        [Output("upgradePaths", "A dictionary containing strings going from an old to new version of library path.")]
        public static Dictionary<string, string> DatasetToNewPaths()
        {
            if (m_DatasetToNewPaths == null)
                ExtractDataSetUpgraders();

            return m_DatasetToNewPaths;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractDataSetUpgraders()
        {
            m_DatasetToNewPaths = new Dictionary<string, string>();
            m_DatasetToOldPaths = new Dictionary<string, List<string>>();
            m_DatasetToMessageForDeleted = new Dictionary<string, string>();

            foreach (string filePath in Directory.GetFiles(Base.Query.BHoMFolderUpgrades(), $"*.json", SearchOption.AllDirectories))
            {
                try
                {

                    if (File.Exists(filePath))
                    {
                        string json = File.ReadAllText(filePath);
                        BsonDocument upgrades = null;
                        if (!BsonDocument.TryParse(json, out upgrades))
                        {
                            continue;
                        }

                        if (upgrades.Contains("Dataset"))
                        {
                            BsonDocument datasetUpgrade = upgrades["Dataset"] as BsonDocument;
                            if (datasetUpgrade.Contains("ToNew"))
                            {
                                BsonDocument toNew = datasetUpgrade["ToNew"] as BsonDocument;
                                if (toNew != null)
                                {
                                    Dictionary<string, string> itemUpgrades = toNew.ToDictionary(x => x.Name, x => x.Value.AsString);
                                    foreach (KeyValuePair<string, string> upgrade in itemUpgrades)
                                    {
                                        m_DatasetToNewPaths[upgrade.Key] = upgrade.Value;
                                    }
                                }
                            }

                            if (datasetUpgrade.Contains("ToOld"))
                            {
                                BsonDocument toOld = datasetUpgrade["ToOld"] as BsonDocument;
                                if (toOld != null)
                                {
                                    Dictionary<string, string> itemUpgrades = toOld.ToDictionary(x => x.Name, x => x.Value.AsString);
                                    foreach (KeyValuePair<string, string> upgrade in itemUpgrades)
                                    {
                                        if (!m_DatasetToOldPaths.ContainsKey(upgrade.Key))
                                            m_DatasetToOldPaths[upgrade.Key] = new List<string> { upgrade.Value };
                                        else
                                            m_DatasetToOldPaths[upgrade.Key].Add(upgrade.Value);
                                    }
                                }
                            }

                            if (datasetUpgrade.Contains("MessageForDeleted"))
                            {
                                BsonDocument toNew = datasetUpgrade["MessageForDeleted"] as BsonDocument;
                                if (toNew != null)
                                {
                                    Dictionary<string, string> itemUpgrades = toNew.ToDictionary(x => x.Name, x => x.Value.AsString);
                                    foreach (KeyValuePair<string, string> upgrade in itemUpgrades)
                                    {
                                        m_DatasetToMessageForDeleted[upgrade.Key] = upgrade.Value;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Engine.Base.Compute.RecordError("Failed to extract Dataset upgrade files.");
                }
            }
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, string> m_DatasetToNewPaths = null;
        private static Dictionary<string, List<string>> m_DatasetToOldPaths = null;
        private static Dictionary<string, string> m_DatasetToMessageForDeleted = null;

        /***************************************************/
    }
}



