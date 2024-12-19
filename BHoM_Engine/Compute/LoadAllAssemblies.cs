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

using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Loads all .dll assemblies with names ending with oM, _Engine and _Adapter (with optional suffixes) from a given folder.")]
        [Input("folder", "Folder to load the assemblies from. If left empty, default BHoM assemblies folder will be used.")]
        [Input("regexFilter", "Regular expression filter to be applied to the assembly names (with .dll already cut off)." +
                              "Default value is 'oM$|_Engine$|_Adapter$' (names ending with 'oM', '_Engine' or '_Adapter')." +
                              "If the input is left null or blank, filter '.*' will be applied (accepts all names).")]
        [Input("parseSubfolders", "If true, subfolders of the input folder will be parsed, otherwise only top folder to be considered.")]
        [Input("forceParseFolder", "If false, the method will execute only once per lifetime of the process per each combination of folder and suffix values (every attempt after the first will be skipped).\n" +
                                   "If true, the given folder will be parsed for assemblies with given suffix on every call of this method.")]
        [Output("assemblies", "Assemblies that meet folder and suffix requirements and are loaded to BHoM.")]
        public static List<Assembly> LoadAllAssemblies(string folder = "", string regexFilter = @"oM$|_Engine$|_Adapter$", bool parseSubfolders = false, bool forceParseFolder = false)
        {
            List<Assembly> result = new List<Assembly>();
            if (string.IsNullOrEmpty(folder))
                folder = Query.BHoMFolder();

            if (!Directory.Exists(folder))
            {
                RecordWarning("The folder provided to load the assemblies from does not exist: " + folder);
                return result;
            }

            lock (m_LoadAllAssembliesLock)
            {
                string key = folder + "%" + regexFilter + "%" + parseSubfolders;
                if (!forceParseFolder && m_AlreadyLoaded.ContainsKey(key))
                    return m_AlreadyLoaded[key].ToList();

                Regex regex;
                if (!string.IsNullOrWhiteSpace(regexFilter))
                    regex = new Regex(regexFilter);
                else
                    regex = new Regex(".*");

                SearchOption searchOption = parseSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                foreach (string file in Directory.GetFiles(folder, "*.dll", searchOption))
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    if (regex.IsMatch(name))
                    {
                        Assembly loaded = LoadAssembly(file);
                        if (loaded != null)
                            result.Add(loaded);
                    }
                }

                m_AlreadyLoaded[key] = result.ToList();
                return result;
            }
        }


        /***************************************************/
        /****              Private fields               ****/
        /***************************************************/

        private static Dictionary<string, List<Assembly>> m_AlreadyLoaded = new Dictionary<string, List<Assembly>>();

        private static readonly object m_LoadAllAssembliesLock = new object();

        /***************************************************/
    }
}






