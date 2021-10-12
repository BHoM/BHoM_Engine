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

using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("5.0", "BH.Engine.Reflection.Compute.LoadAllAssemblies(System.String)")]
        [Description("Loads all .dll assemblies with names ending with oM, _Engine and _Adapter (with optional suffixes) from a given folder.")]
        [Input("folder", "Folder to load the assemblies from. If left empty, default BHoM assemblies folder will be used.")]
        [Input("suffix", "Suffix to be added to the standard BHoM library endings (oM, _Engine, _Adapter) when parsing the folder.\n"+
               "For example, if this value is equal to '_2018', assemblies ending with oM_2018, _Engine_2018 or _Adapter_2018 will be loaded.")]
        [Output("assemblies", "Assemblies loaded in this method call.")]
        public static List<Assembly> LoadAllAssemblies(string folder = "", string suffix = "")
        {
            List<Assembly> result = new List<Assembly>();
            lock (m_LoadAssembliesLock)
            {
                if (string.IsNullOrEmpty(folder))
                    folder = Query.BHoMFolder();

                if (!Directory.Exists(folder))
                {
                    RecordWarning("The folder provided to load the assemblies from does not exist: " + folder);
                    return result;
                }

                foreach (string file in Directory.GetFiles(folder))
                {
                    if (!file.EndsWith(".dll"))
                        continue;

                    string[] parts = file.Split(new char[] { '.', '\\' });
                    if (parts.Length < 2)
                        continue;

                    string name = parts[parts.Length - 2];
                    if (m_LoadedAssemblies.Contains(name))
                        continue;

                    string[] suffixes = { "oM", "_Engine", "_Adapter" };
                    if (!string.IsNullOrWhiteSpace(suffix))
                        suffixes = suffixes.Select(x => $"x{suffix}").ToArray();

                    if (suffixes.Any(x => name.EndsWith(x)))
                    {
                        try
                        {
                            Assembly loaded = Assembly.LoadFrom(file);
                            result.Add(loaded);
                            m_LoadedAssemblies.Add(name);
                        }
                        catch
                        {
                            RecordWarning("Failed to load assembly " + file);
                        }
                    }
                }
            }

            if (result.Count != 0)
                Query.RefreshAssemblyList();

            return result;
        }

        /***************************************************/
        /**** Private Static Fields                     ****/
        /***************************************************/

        private static List<string> m_LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName().Name).ToList();
        private static readonly object m_LoadAssembliesLock = new object();

        /***************************************************/
    }
}


