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
using MongoDB.Bson;
using Mono.Cecil;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BH.Engine.Versioning
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Converter Converter(string version)
        {
            if (Global.Converters.ContainsKey(version))
                return Global.Converters[version];

            // Load the upgrades from the json files matching the version
            Converter converter = new Converter();
            foreach (string file in Directory.GetFiles(Base.Query.BHoMFolderUpgrades(), $"*{version.Replace(".", "")}.json", SearchOption.AllDirectories))
                LoadUpgrades(converter, file);

            // Load the custom upgrades and downgrades found in the upgrade dlls
            if (Global.CustomUpgrades.ContainsKey(version))
                converter.ToNewObject.AddRange(Global.CustomUpgrades[version]);
            if (Global.CustomDowngrades.ContainsKey(version))
                converter.ToOldObject.AddRange(Global.CustomDowngrades[version]);

            Global.Converters[version] = converter;

            return converter;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void LoadUpgrades(Converter converter, string upgradesFile)
        {
            if (!File.Exists(upgradesFile))
            {
                Console.WriteLine("Failed to find the upgrade file: " + upgradesFile);
                return;
            }

            string json = File.ReadAllText(upgradesFile);
            BsonDocument upgrades = null;
            if (!BsonDocument.TryParse(json, out upgrades))
            {
                Console.WriteLine("Failed to load the upgrade file.");
                return;
            }

            if (upgrades.Contains("Type"))
                LoadTypeUpgrades(converter, upgrades["Type"] as BsonDocument);

            if (upgrades.Contains("Namespace"))
                AddNamespaceUpgrades(converter, upgrades["Namespace"] as BsonDocument);

            if (upgrades.Contains("Method"))
                LoadMethodUpgrades(converter, upgrades["Method"] as BsonDocument);

            if (upgrades.Contains("Property"))
                LoadPropertyUpgrades(converter, upgrades["Property"] as BsonDocument);

            if (upgrades.Contains("MessageForDeleted"))
                LoadMessageForDeleted(converter, upgrades["MessageForDeleted"] as BsonDocument);

            if (upgrades.Contains("MessageForNoUpgrade"))
                LoadMessageForNoUpgrade(converter, upgrades["MessageForNoUpgrade"] as BsonDocument);
        }

        /***************************************************/

        private static void LoadTypeUpgrades(Converter converter, BsonDocument data)
        {
            if (data == null)
                return;

            if (data.Contains("ToNew"))
            {
                BsonDocument toNew = data["ToNew"] as BsonDocument;
                if (toNew != null)
                    converter.ToNewType.AddRange(toNew.ToDictionary(x => x.Name, x => x.Value.AsString));
            }

            if (data.Contains("ToOld"))
            {
                BsonDocument toOld = data["ToOld"] as BsonDocument;
                if (toOld != null)
                    converter.ToOldType.AddRange(toOld.ToDictionary(x => x.Name, x => x.Value.ToString()));
            }
        }

        /***************************************************/

        private static void AddNamespaceUpgrades(Converter converter, BsonDocument data)
        {
            if (data == null)
                return;

            if (data.Contains("ToNew"))
            {
                BsonDocument toNew = data["ToNew"] as BsonDocument;
                if (toNew != null)
                    converter.ToNewType.AddRange(toNew.ToDictionary(x => x.Name, x => x.Value.AsString));
            }

            if (data.Contains("ToOld"))
            {
                BsonDocument toOld = data["ToOld"] as BsonDocument;
                if (toOld != null)
                    converter.ToOldType.AddRange(toOld.ToDictionary(x => x.Name, x => x.Value.AsString));
            }
        }

        /***************************************************/

        private static void LoadMethodUpgrades(Converter converter, BsonDocument data)
        {
            if (data.Contains("ToNew"))
            {
                BsonDocument toNew = data["ToNew"] as BsonDocument;
                if (toNew != null)
                    converter.ToNewMethod.AddRange(toNew.ToDictionary(x => x.Name, x => x.Value.ToBsonDocument()));
            }

            if (data.Contains("ToOld"))
            {
                BsonDocument toOld = data["ToOld"] as BsonDocument;
                if (toOld != null)
                    converter.ToOldMethod.AddRange(toOld.ToDictionary(x => x.Name, x => x.Value.ToBsonDocument()));
            }
        }

        /***************************************************/

        private static void LoadPropertyUpgrades(Converter converter, BsonDocument data)
        {
            if (data == null)
                return;

            if (data.Contains("ToNew"))
            {
                BsonDocument toNew = data["ToNew"] as BsonDocument;
                if (toNew != null)
                    converter.ToNewProperty.AddRange(toNew.ToDictionary(x => x.Name, x => x.Value.AsString));
            }

            if (data.Contains("ToOld"))
            {
                BsonDocument toOld = data["ToOld"] as BsonDocument;
                if (toOld != null)
                    converter.ToOldProperty.AddRange(toOld.ToDictionary(x => x.Name, x => x.Value.ToString()));
            }
        }

        /***************************************************/

        private static void LoadMessageForDeleted(Converter converter, BsonDocument data)
        {
            if (data != null)
                converter.MessageForDeleted.AddRange(data.ToDictionary(x => x.Name, x => x.Value.AsString));
        }

        /***************************************************/

        private static void LoadMessageForNoUpgrade(Converter converter, BsonDocument data)
        {
            if (data != null)
                converter.MessageForNoUpgrade.AddRange(data.ToDictionary(x => x.Name, x => x.Value.AsString));
        }

        /***************************************************/

        private static void AddRange<T>(this Dictionary<string, T> target, Dictionary<string, T> source)
        {
            foreach (var kvp in source)
                target[kvp.Key] = kvp.Value;
        }

        /***************************************************/
    }
}






