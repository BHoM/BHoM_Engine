/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Base.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace BH.Engine.Base.Objects
{
    public static class Initialisation
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public static readonly Regex DefaultAssemblyNameFilter = new Regex(@"oM$|_Engine$|_Adapter$");

        public static string DefaultAssemblyContentFilePath =>
            System.IO.Path.Combine(BH.Engine.Base.Query.BHoMFolderResources(), "AssemblyContent.tsv");


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Reads existing code elements from a tsv file. Returns an empty list if the file does not exist.")]
        public static List<T> LoadCodeElements<T>(string tsvFilePath, Func<string, T> fromTsv) where T : CodeElementRecord
        {
            if (!File.Exists(tsvFilePath))
                return new List<T>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                List<T> codeElements = File.ReadAllLines(tsvFilePath)
                    .Select(x => fromTsv(x))
                    .Where(x => x != null)
                    .ToList();

                stopwatch.Stop();
                BH.Engine.Base.Compute.RecordNote($"Time to load code elements: {stopwatch.Elapsed.TotalMilliseconds / 1000} s.");

                return codeElements;
            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError(e, $"Failed to load the code elements from '{System.IO.Path.GetFileName(tsvFilePath)}'.");
                return null;
            }
        }

        /***************************************************/

        [Description("Scans disk for new/updated assemblies matching the filter, loads them, harvests their code elements, merges them into the given list and persists the result back to tsv.")]
        public static List<T> RefreshFromNewAssemblies<T>(
            IReadOnlyList<T> codeElements,
            Regex assemblyNameFilter,
            string tsvFilePath,
            Func<T, string> toTsv,
            Func<IReadOnlyList<string>, List<T>> harvestNewElements) where T : CodeElementRecord
        {
            List<T> currentElements = codeElements?.ToList() ?? new List<T>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Dictionary<string, DateTime> lastAssemblyUpdateTimes = currentElements
                .GroupBy(x => x.AssemblyName)
                .ToDictionary(x => x.Key, x => x.First().AssemblyModifiedTime);

            List<string> loadedAssemblies = LoadNewAssemblies(lastAssemblyUpdateTimes, assemblyNameFilter);

            stopwatch.Stop();
            BH.Engine.Base.Compute.RecordNote($"Time to load all updated/new assemblies from current domain: {stopwatch.Elapsed.TotalMilliseconds / 1000} s.");

            if (loadedAssemblies.Count == 0)
                return currentElements;

            List<T> loadedCodeElements = harvestNewElements(loadedAssemblies);
            if (loadedCodeElements.Count == 0)
                return currentElements;

            stopwatch.Restart();

            List<T> updatedElements = currentElements
                .Where(x => !loadedAssemblies.Contains(x.AssemblyName, StringComparer.OrdinalIgnoreCase))
                .Concat(loadedCodeElements)
                .ToList();

            List<string> lines = updatedElements
                .Select(x => toTsv(x))
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            try
            {
                string directory = Path.GetDirectoryName(tsvFilePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllLines(tsvFilePath, lines);
            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError(e, $"Failed to save the assembly content to {tsvFilePath}.");
            }

            stopwatch.Stop();
            BH.Engine.Base.Compute.RecordNote($"Time to update the code elements with the content of the updated/new assemblies: {stopwatch.Elapsed.TotalMilliseconds / 1000} s.");

            return updatedElements;
        }

        /***************************************************/

        public static AssemblyResolver CreateAssemblyResolver(IEnumerable<CodeElementRecord> codeElements)
        {
            List<CodeElementRecord> elements = codeElements?.ToList() ?? new List<CodeElementRecord>();

            Dictionary<string, List<string>> assemblyNamesPerType = elements
                .Where(x => x.Type == CodeElementType.Type)
                .GroupBy(x => x.DisplayText)
                .ToDictionary(group => group.Key, group => group.Select(x => x.AssemblyName).Distinct().ToList());

            Dictionary<string, Dictionary<string, List<string>>> assemblyNamesPerExtensionMethod
                = BuildExtensionMethodDictionary(elements);

            return new AssemblyResolver(assemblyNamesPerType, assemblyNamesPerExtensionMethod);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dictionary<string, Dictionary<string, List<string>>> BuildExtensionMethodDictionary(
            List<CodeElementRecord> codeElements)
        {
            Dictionary<string, Dictionary<string, List<string>>> result
                = new Dictionary<string, Dictionary<string, List<string>>>();

            foreach (CodeElementRecord record in codeElements.Where(x =>
                x.Type == CodeElementType.Method_Query ||
                x.Type == CodeElementType.Method_Compute ||
                x.Type == CodeElementType.Method_Convert ||
                x.Type == CodeElementType.Method_Modify))
            {
                try
                {
                    string firstParamTypeName = record.InputKeys?.FirstOrDefault();

                    if (!string.IsNullOrEmpty(firstParamTypeName))
                    {
                        string methodName = ExtractMethodName(record.DisplayText);

                        if (!result.ContainsKey(methodName))
                            result[methodName] = new Dictionary<string, List<string>>();

                        if (!result[methodName].ContainsKey(firstParamTypeName))
                            result[methodName][firstParamTypeName] = new List<string>();

                        if (!result[methodName][firstParamTypeName].Contains(record.AssemblyName))
                            result[methodName][firstParamTypeName].Add(record.AssemblyName);
                    }
                }
                catch (Exception ex)
                {
                    BH.Engine.Base.Compute.RecordWarning($"Failed to parse extension method from {record.DisplayText}: {ex.Message}");
                }
            }

            return result;
        }

        /***************************************************/

        private static string ExtractMethodName(string displayText)
        {
            int openParen = displayText.IndexOf('(');
            if (openParen < 0)
                return displayText;

            string beforeParams = displayText.Substring(0, openParen);

            int genericStart = beforeParams.IndexOf('<');
            if (genericStart > 0)
                beforeParams = beforeParams.Substring(0, genericStart);

            int lastDot = beforeParams.LastIndexOf('.');
            if (lastDot >= 0)
                return beforeParams.Substring(lastDot + 1);

            return beforeParams;
        }

        /***************************************************/

        [Description("Loads all BHoM assemblies from the current domain that match the provided filter.")]
        [Input("lastAssemblyUpdateTimes", "Records of the last time each assembly was updated.")]
        [Input("assemblyNameFilter", "Regex filter applied to assembly names.")]
        [Output("loadedAssemblies", "Assemblies loaded as considered new.")]
        public static List<string> LoadNewAssemblies(Dictionary<string, DateTime> lastAssemblyUpdateTimes, Regex assemblyNameFilter)
        {
            if (lastAssemblyUpdateTimes == null)
            {
                BH.Engine.Base.Compute.RecordError("lastAssemblyUpdateTimes was not provided. No assembly was loaded.");
                return new List<string>();
            }

            if (assemblyNameFilter == null)
            {
                BH.Engine.Base.Compute.RecordError("assemblyNameFilter was not provided. No assembly was loaded.");
                return new List<string>();
            }

            Dictionary<string, DateTime> lastUpdateTimes = lastAssemblyUpdateTimes.ToDictionary(x => x.Key.ToLower(), x => x.Value);
            HashSet<string> loadedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> visitedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            string bhomFolder = Query.BHoMFolder();
            foreach (string subFolder in SubFoldersForRuntime())
            {
                string runtimeFolder = Path.Combine(bhomFolder, subFolder);
                LoadNewAssembliesForFolder(runtimeFolder, lastUpdateTimes, loadedAssemblies, visitedAssemblies, assemblyNameFilter);
            }

            LoadNewAssembliesForFolder(bhomFolder, lastUpdateTimes, loadedAssemblies, visitedAssemblies, assemblyNameFilter);

            return loadedAssemblies.ToList();
        }

        /***************************************************/

        [Description("Convert a row in an Excel file (in tsv format) into a code element record.")]
        [Input("tsv", "Excel row that contains the data related to the code element in a tsv format.")]
        [Output("codeElement", "Converted code element.")]
        public static CodeElementRecord FromTsv(this string tsv)
        {
            string[] parts = tsv.Split('\t');
            if (parts.Length < 5)
            {
                Compute.RecordError("Failed to extract code element record from tvs content because it doesn't contain 5 parts. Input tsv: " + tsv);
                return null;
            }

            if (!Enum.TryParse(parts[1], out CodeElementType type))
            {
                Compute.RecordError($"Failed to extract code element record from tvs content because the code element type ({parts[1]}) is not recognised. Input tsv: " + tsv);
                return null;
            }

            if (!long.TryParse(parts[4], out long utcTime))
            {
                Compute.RecordError($"Failed to extract code element record from tvs content because the provided time ({parts[4]}) is not valid. Input tsv: " + tsv);
                return null;
            }

            List<string> inputKeys = new List<string>();
            List<string> outputKeys = new List<string>();
            if (parts.Length >= 7)
            {
                inputKeys = string.IsNullOrEmpty(parts[5]) ? new List<string>() : parts[5].Split(',').ToList();
                outputKeys = string.IsNullOrEmpty(parts[6]) ? new List<string>() : parts[6].Split(',').ToList();
            }

            return new CodeElementRecord
            {
                AssemblyName = parts[0],
                Type = type,
                DisplayText = parts[2],
                //Json = parts[3],
                AssemblyModifiedTime = DateTime.FromFileTimeUtc(utcTime),
                InputKeys = inputKeys,
                OutputKeys = outputKeys
            };
        }

        /***************************************************/

        public static string ToTsv(this CodeElementRecord codeElement)
        {
            return $"{codeElement.AssemblyName}" +
                $"\t{codeElement.Type}" +
                $"\t{codeElement.DisplayText}" +
                //TODO: decrement indices where needed and delete this one!
                $"\tPlaceholder" +
                $"\t{codeElement.AssemblyModifiedTime.ToFileTimeUtc()}" +
                $"\t{codeElement.InputKeys.ToCommaSeparatedList()}" +
                $"\t{codeElement.OutputKeys.ToCommaSeparatedList()}";
        }

        /***************************************************/

        [Description("Returns the best on-disk path for a BHoM assembly, preferring the runtime-specific subdirectory (netX.0\\ or netfx\\) over the flat folder.")]
        [Input("assemblyName", "Assembly name without extension, e.g. 'SQL_Adapter'.")]
        [Output("path", "Full path to the .dll file; the file may or may not exist.")]
        public static string AssemblyFilePath(string assemblyName)
        {
            string bhomFolder = Query.BHoMFolder();

            foreach (string subFolder in SubFoldersForRuntime())
            {
                string runtimePath = System.IO.Path.Combine(bhomFolder, subFolder, assemblyName + ".dll");
                if (File.Exists(runtimePath))
                    return runtimePath;
            }

            return System.IO.Path.Combine(bhomFolder, assemblyName + ".dll");
        }

        /***************************************************/

        [Description("Returns the runtime-specific subdirectories of the BHoM Assemblies folder where assemblies compatible with the current .NET runtime can be found. " +
             "Returns '.../Assemblies/netfx/' on .NET Framework and '.../Assemblies/netX.0/' on CoreCLR (.NET X).")]
        [Output("subFolders", "runtime-specific subdirectories for the BHoM assemblies sorted in the order they should be traversed.")]
        public static List<string> SubFoldersForRuntime()
        {
            if (m_SubFoldersForRuntime != null)
                return m_SubFoldersForRuntime;

            string desc = RuntimeInformation.FrameworkDescription;
            if (desc.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
            {
                m_SubFoldersForRuntime = new List<string> { "netfx" };
            }
            else
            {
                m_SubFoldersForRuntime = new List<string>();
                int major = Environment.Version.Major;
                for (int v = major; v >= 5; v--)
                    m_SubFoldersForRuntime.Add($"net{v}.0");
            }

            return m_SubFoldersForRuntime;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void LoadNewAssembliesForFolder(string folderPath, Dictionary<string, DateTime> lastUpdateTimes, HashSet<string> loadedAssemblies, HashSet<string> visitedAssemblies, Regex assemblyNameFilter)
        {
            if (!Directory.Exists(folderPath))
                return;

            foreach (string file in Directory.GetFiles(folderPath, "*.dll", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileNameWithoutExtension(file);

                if (assemblyNameFilter.IsMatch(name) && !visitedAssemblies.Contains(name))
                {
                    visitedAssemblies.Add(name);
                    string key = name.ToLower();

                    if (!lastUpdateTimes.ContainsKey(key) || lastUpdateTimes[key] < File.GetLastWriteTimeUtc(file))
                    {
                        Assembly assembly = BH.Engine.Base.Compute.LoadAssembly(file);
                        if (assembly != null)
                        {
                            BH.Engine.Base.Compute.RecordNote($"Assembly {name} loaded as it was newer than its last recorded update time.");
                            loadedAssemblies.Add(name);
                        }
                    }
                }
            }
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<string> m_SubFoldersForRuntime = null;

        /***************************************************/
    }
}
