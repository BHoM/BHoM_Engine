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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Base.Objects
{
    public class AssemblyResolver : IAssemblyResolver
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public AssemblyResolver(Dictionary<string, List<string>> assemblyNamePerType = null, Dictionary<string, Dictionary<string, List<string>>> assemblyNamesPerExtensionMethod = null)
        {
            if (assemblyNamePerType != null)
                m_AssemblyNamePerType = assemblyNamePerType;

            if (assemblyNamesPerExtensionMethod != null)
                m_AssemblyNamesPerExtensionMethod = assemblyNamesPerExtensionMethod;
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Make sure assemblies that contain a type matching the input name are loaded. Return true if any assembly was loaded.")]
        public bool MakeSureAssemblyIsLoadedForType(string type)
        {
            if (string.IsNullOrEmpty(type) || !type.StartsWith("BH."))
                return false;

            string[] parts = type.Split(',');
            List<string> assemblyNames = new List<string>();

            if (parts.Length > 1)
            {
                assemblyNames.Add(parts[1].Trim());
            }
            else if (parts.Length == 1)
            {
                if (m_AssemblyNamePerType.ContainsKey(type))
                    assemblyNames.AddRange(m_AssemblyNamePerType[type]);
            }

            bool anyLoaded = false;
            foreach (string assemblyName in assemblyNames.Where(x => !string.IsNullOrEmpty(x)))
            {
                if (!BH.Engine.Base.Query.IsAssemblyLoaded(assemblyName))
                {
                    string assemblyPath = Initialisation.AssemblyFilePath(assemblyName);
                    if (!File.Exists(assemblyPath))
                    {
                        BH.Engine.Base.Compute.RecordError($"Assembly file not found when trying to load assemblies for type {type}: {assemblyPath}");
                        continue;
                    }

                    try
                    {
                        Assembly assembly = BH.Engine.Base.Compute.LoadAssembly(assemblyPath);
                        if (assembly == null)
                            BH.Engine.Base.Compute.RecordError($"Failed to load assembly: {assemblyName}");
                        else
                            anyLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        BH.Engine.Base.Compute.RecordError(ex, $"Exception while loading assembly for type {type}: {assemblyPath}.");
                        return false;
                    }
                }
            }

            return anyLoaded;
        }

        /***************************************************/

        [Description("Make sure assemblies containing extension methods matching the input name and target type are loaded. Returns true if any assembly was loaded.")]
        public bool MakeSureAssemblyIsLoadedForExtensionMethod(string methodName, Type targetType)
        {
            if (string.IsNullOrEmpty(methodName) || targetType == null)
                return false;

            if (!m_AssemblyNamesPerExtensionMethod.ContainsKey(methodName))
                return false;

            Dictionary<string, List<string>> typeToAssemblies = m_AssemblyNamesPerExtensionMethod[methodName];
            HashSet<string> assembliesToLoad = new HashSet<string>();

            string exactTypeName = TypeKey(targetType);
            if (typeToAssemblies.ContainsKey(exactTypeName))
                assembliesToLoad.UnionWith(typeToAssemblies[exactTypeName]);

            Type baseType = targetType.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                string baseTypeName = TypeKey(baseType);
                if (typeToAssemblies.ContainsKey(baseTypeName))
                    assembliesToLoad.UnionWith(typeToAssemblies[baseTypeName]);
                baseType = baseType.BaseType;
            }

            foreach (Type interfaceType in targetType.GetInterfaces())
            {
                string interfaceName = TypeKey(interfaceType);
                if (typeToAssemblies.ContainsKey(interfaceName))
                    assembliesToLoad.UnionWith(typeToAssemblies[interfaceName]);
            }

            if (targetType.IsGenericType)
            {
                Type genericDef = targetType.GetGenericTypeDefinition();
                string genericTypeName = TypeKey(genericDef);
                if (typeToAssemblies.ContainsKey(genericTypeName))
                    assembliesToLoad.UnionWith(typeToAssemblies[genericTypeName]);
            }

            bool anyLoaded = false;
            foreach (string assemblyName in assembliesToLoad)
            {
                if (!BH.Engine.Base.Query.IsAssemblyLoaded(assemblyName))
                {
                    string assemblyPath = Initialisation.AssemblyFilePath(assemblyName);
                    if (!File.Exists(assemblyPath))
                    {
                        BH.Engine.Base.Compute.RecordNote($"Assembly not found for extension method {methodName}: {assemblyPath}");
                        continue;
                    }

                    try
                    {
                        Assembly assembly = BH.Engine.Base.Compute.LoadAssembly(assemblyPath);
                        if (assembly == null)
                            BH.Engine.Base.Compute.RecordWarning($"Failed to load assembly: {assemblyName}");
                        else
                            anyLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        BH.Engine.Base.Compute.RecordError(ex, $"Exception loading assembly for extension method {methodName}: {assemblyPath}");
                    }
                }
            }

            return anyLoaded;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string TypeKey(Type type)
        {
            string key = type.FullName ?? type.Name;
            int cut = key.IndexOfAny(new char[] { ',', '[' });
            if (cut > 0)
                key = key.Substring(0, cut);
            return key;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        Dictionary<string, List<string>> m_AssemblyNamePerType = new Dictionary<string, List<string>>();

        Dictionary<string, Dictionary<string, List<string>>> m_AssemblyNamesPerExtensionMethod = new Dictionary<string, Dictionary<string, List<string>>>();

        /***************************************************/
    }
}
