/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using System.Collections;
using System.Globalization;
using System.Resources;
using BH.oM.Base;
using BH.Engine.Serialiser;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.DataStructure;
using BH.Engine.DataStructure;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<IBHoMObject> Library(string name)
        {
            List<string> keys;
            if (!LibraryPaths().TryGetValue(name, out keys))
                return new List<IBHoMObject>();

            return keys.SelectMany(x => ParseLibrary(x)).ToList();

        }

        /***************************************************/

        public static Tree<string> LibraryTree()
        {
            if (m_dbTree == null || m_dbTree.Count() == 0 || m_dbTree.Children.Count == 0)
            {
                List<string> paths = LibraryStrings().Keys.ToList();
                m_dbTree = DataStructure.Create.Tree(paths, paths.Select(x => x.Split('\\').ToList()).ToList(), "Select a data set").ShortenBranches();
            }
            return m_dbTree;
        }

        /***************************************************/

        public static void RefreshLibraries()
        {
            m_libraryPaths = new Dictionary<string, List<string>>();
            m_libraryStrings = new Dictionary<string, string[]>();
            m_parsedLibrary = new Dictionary<string, List<IBHoMObject>>();
            m_dbTree = new Tree<string>();
            GetPathsAndLoadLibraries();

        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<IBHoMObject> ParseLibrary(string name)
        {
            List<IBHoMObject> objects;
            if (!m_parsedLibrary.TryGetValue(name, out objects))
            {
                //string libraryString = LibraryString(name);

                string[] entries = LibraryString(name);

                if (entries == null)
                    return new List<IBHoMObject>();

                //string[] entries = libraryString.Split('\n');
                objects = new List<IBHoMObject>();

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
                            obj = null;
                        }
                        if (obj != null)
                            objects.Add(obj);
                    }
                }

                m_parsedLibrary[name] = objects;
            }

            return objects;
        }
        
        /***************************************************/
        private static Dictionary<string, string[]> LibraryStrings()
        {
            //Check that libraries has been loaded
            if (m_libraryStrings.Count < 1)
                RefreshLibraries();

            return m_libraryStrings;
        }

        /***************************************************/

        private static Dictionary<string, List<string>> LibraryPaths()
        {
            if (m_libraryPaths.Count < 1)
                RefreshLibraries();
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

        private static void GetPathsAndLoadLibraries(string folderPath = "", string basePath = "")
        {
            string internalPath = Path.Combine(basePath, folderPath);
            string folder = Path.Combine(m_sourceFolder, internalPath);

            if (!Directory.Exists(folder))
                return;

            foreach (string path in Directory.GetFiles(folder))
            {
                string filePathName = Path.Combine(internalPath, Path.GetFileNameWithoutExtension(path));
                AddToPathDictionary(filePathName, filePathName);
                m_libraryStrings[filePathName] = File.ReadAllLines(path);
            }

            foreach (string dictPath in Directory.GetDirectories(folder))
            {
                GetPathsAndLoadLibraries(Path.GetFileName(dictPath), internalPath);
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
                m_libraryPaths[folderName] = new List<string>();
                m_libraryPaths[folderName].Add(dictionaryPath);
            }

            //Add full path name
            if (folderName != path)
            {
                if (m_libraryPaths.ContainsKey(path))
                    m_libraryPaths[path].Add(dictionaryPath);
                else
                {
                    m_libraryPaths[path] = new List<string>();
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

        private static readonly string m_sourceFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"BHoM\DataSets");

        private static Dictionary<string, string[]> m_libraryStrings = new Dictionary<string, string[]>();
        private static Dictionary<string, List<IBHoMObject>> m_parsedLibrary = new Dictionary<string, List<IBHoMObject>>();
        private static Dictionary<string, List<string>> m_libraryPaths = new Dictionary<string, List<string>>();
        private static Tree<string> m_dbTree = new Tree<string>();

        /***************************************************/
    }
}
