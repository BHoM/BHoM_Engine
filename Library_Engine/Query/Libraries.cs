/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base;
using System.IO;
using System.Linq;
using BH.oM.Data.Collections;
using BH.Engine.Data;
using BH.oM.Data.Library;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the content of the Datasets from the library")]
        [Input("libraryName", "The name of the Dataset(s) to extract")]
        [Output("libraryData", "The data from the Dataset(s)")]
        public static List<IBHoMObject> Library(string libraryName)
        {
            return Datasets(libraryName).SelectMany(x => x.Data).ToList();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dataset ParseLibrary(string name)
        {
            Dataset dataset;

            if (!m_datasets.TryGetValue(name, out dataset))
            {

                m_deserialisationEvents[name] = new List<Tuple<oM.Reflection.Debugging.EventType, string>>();

                string[] entries = LibraryString(name);

                if (entries == null || entries.Length == 0)
                {
                    AddDeserialisationEvent(name, oM.Reflection.Debugging.EventType.Warning, "No dataset with the name " + name + " could be found.");
                    return new Dataset();
                }
                else if (entries.Length == 1)
                {

                    IBHoMObject obj;
                    try
                    {
                        obj = BH.Engine.Serialiser.Convert.FromJson(entries[0]) as IBHoMObject;
                    }
                    catch
                    {
                        AddDeserialisationEvent(name, oM.Reflection.Debugging.EventType.Error, "Failed to deserialise the dataset with name " + name);
                        return new Dataset();
                    }

                    dataset = obj as Dataset;

                    if (dataset == null)
                    {
                        dataset = new Dataset() { Data = new List<IBHoMObject> { obj } };
                        AddDeserialisationEvent(name, oM.Reflection.Debugging.EventType.Warning, "No Source information is available for the dataset named " + name);
                    }
                }
                else
                {

                    List<IBHoMObject> objects = new List<IBHoMObject>();

                    //Parse the library from json to IBHoMObjects
                    foreach (string entry in entries)
                    {
                        if (!string.IsNullOrWhiteSpace(entry))
                        {
                            IBHoMObject obj;
                            try
                            {
                                obj = (IBHoMObject)BH.Engine.Serialiser.Convert.FromJson(entry);
                            }
                            catch
                            {
                                AddDeserialisationEvent(name, oM.Reflection.Debugging.EventType.Warning, "Failed to deserialise at least one item in the dataset named " + name);
                                obj = null;
                            }
                            if (obj != null)
                                objects.Add(obj);
                        }
                    }

                    AddDeserialisationEvent(name, oM.Reflection.Debugging.EventType.Warning, "No Source information is available for the dataset named " + name);
                    dataset = new Dataset { Data = objects };
                }

                m_datasets[name] = dataset;
            }

            foreach (var events in m_deserialisationEvents[name])
            {
                Reflection.Compute.RecordEvent(events.Item2, events.Item1);
            }

            return dataset;
        }

        /***************************************************/

        private static void AddDeserialisationEvent(string name, BH.oM.Reflection.Debugging.EventType type, string message)
        {
            m_deserialisationEvents[name].Add(new Tuple<oM.Reflection.Debugging.EventType, string>(type, message));
        }

        /***************************************************/

        private static Dictionary<string, string[]> LibraryStrings()
        {
            //Check that libraries has been loaded
            if (m_libraryStrings.Count < 1)
                InitialiseLibraries();

            return m_libraryStrings;
        }

        /***************************************************/

        private static Dictionary<string, HashSet<string>> LibraryPaths()
        {
            if (m_libraryPaths.Count < 1)
                InitialiseLibraries();
            return m_libraryPaths;
        }

        /***************************************************/

        private static string[] LibraryString(string name)
        {
            string[] db;
            if (!LibraryStrings().TryGetValue(name, out db))
            {
                db = null;
            }
            return db;
        }

        /***************************************************/

        private static void GetPathsAndLoadLibraries()
        {
            GetPathsAndLoadLibraries(m_sourceFolder, "", "");

            foreach (string path in UserPaths())
            {
                GetPathsAndLoadLibraries(path, "", "");
            }

        }

        /***************************************************/

        private static void InitialiseLibraries()
        {
            m_libraryPaths = new Dictionary<string, HashSet<string>>();
            m_libraryStrings = new Dictionary<string, string[]>();
            m_datasets = new Dictionary<string, Dataset>();
            m_deserialisationEvents = new Dictionary<string, List<Tuple<oM.Reflection.Debugging.EventType, string>>>();
            m_dbTree = new Tree<string>();
            GetPathsAndLoadLibraries();
        }

        /***************************************************/

        private static void GetPathsAndLoadLibraries(string sourceFolder, string folderPath, string basePath)
        {
            string internalPath = Path.Combine(basePath, folderPath);
            string folder = Path.Combine(sourceFolder, internalPath);

            if (!Directory.Exists(folder))
                return;

            foreach (string path in Directory.GetFiles(folder))
            {
                if (Path.HasExtension(path) && Path.GetExtension(path).ToLower() == ".json")
                {
                    string filePathName = Path.Combine(internalPath, Path.GetFileNameWithoutExtension(path));
                    AddToPathDictionary(filePathName, filePathName);
                    m_libraryStrings[filePathName] = File.ReadAllLines(path);
                }
            }

            foreach (string dictPath in Directory.GetDirectories(folder))
            {
                GetPathsAndLoadLibraries(sourceFolder, Path.GetFileName(dictPath), internalPath);
            }
        }

        /***************************************************/

        private static void AddToPathDictionary(string path, string dictionaryPath)
        {
            string folderName = Path.GetFileName(path);

            //Add the path to the folder name
            if (m_libraryPaths.ContainsKey(folderName))
                m_libraryPaths[folderName].Add(dictionaryPath);
            else
            {
                m_libraryPaths[folderName] = new HashSet<string>();
                m_libraryPaths[folderName].Add(dictionaryPath);
            }

            //Add full path name
            if (folderName != path)
            {
                if (m_libraryPaths.ContainsKey(path))
                    m_libraryPaths[path].Add(dictionaryPath);
                else
                {
                    m_libraryPaths[path] = new HashSet<string>();
                    m_libraryPaths[path].Add(dictionaryPath);
                }
            }

            string basePath = Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(basePath))
                AddToPathDictionary(basePath, dictionaryPath);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static readonly string m_sourceFolder = @"C:\ProgramData\BHoM\Datasets";

        private static Dictionary<string, Dataset> m_datasets = new Dictionary<string, Dataset>();
        private static Dictionary<string, List<Tuple<BH.oM.Reflection.Debugging.EventType, string>>> m_deserialisationEvents = new Dictionary<string, List<Tuple<oM.Reflection.Debugging.EventType, string>>>();

        private static Dictionary<string, string[]> m_libraryStrings = new Dictionary<string, string[]>();
        private static Dictionary<string, HashSet<string>> m_libraryPaths = new Dictionary<string, HashSet<string>>();
        private static Tree<string> m_dbTree = new Tree<string>();

        /***************************************************/
    }
}


