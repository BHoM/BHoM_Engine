/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Compute 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void LoadAllAssemblies(string folder = "")
        {
            lock (m_LoadAssembliesLock)
            {
                if (!m_AssemblyAlreadyLoaded)
                {
                    m_AssemblyAlreadyLoaded = true;
                    HashSet<string> loaded = new HashSet<string>(AppDomain.CurrentDomain.GetAssemblies().Select(x => x.FullName.Split(',').First()));

                    if (string.IsNullOrEmpty(folder))
                        folder = Query.BHoMFolder();

                    if (!Directory.Exists(folder))
                    {
                        RecordWarning("The folder provided to load the assemblies from does not exist: " + folder);
                        return;
                    }

                    foreach (string file in Directory.GetFiles(folder))
                    {
                        string[] parts = file.Split(new char[] { '.', '\\' });
                        if (parts.Length >= 2)
                        {
                            string name = parts[parts.Length - 2];
                            if (loaded.Contains(name))
                                continue;
                        }

                        if (file.EndsWith("oM.dll") || file.EndsWith("_Engine.dll") || file.EndsWith("_Adapter.dll") || file.EndsWith("_Test.dll"))
                        {
                            try
                            {
                                Assembly.LoadFrom(file);
                            }
                            catch
                            {
                                RecordWarning("Failed to load assembly " + file);
                            }
                        }
                    }
                }
            }
        }

        /***************************************************/
        /**** Private Static Fields                     ****/
        /***************************************************/

        private static bool m_AssemblyAlreadyLoaded = false;
        private static readonly object m_LoadAssembliesLock = new object();

        /***************************************************/
    }
}

