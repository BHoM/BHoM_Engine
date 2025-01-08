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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BH.Engine.Base
{
    internal static class Global
    {
        /***************************************************/
        /****     Internal properties - collections     ****/
        /***************************************************/

        internal static ConcurrentDictionary<string, Assembly> BHoMAssemblies { get; set; } = new ConcurrentDictionary<string, Assembly>();

        internal static ConcurrentDictionary<string, Assembly> AllAssemblies { get; set; } = new ConcurrentDictionary<string, Assembly>();

        internal static ConcurrentBag<Type> BHoMTypeList { get; set; } = new ConcurrentBag<Type>();

        internal static ConcurrentBag<Type> AdapterTypeList { get; set; } = new ConcurrentBag<Type>();

        internal static ConcurrentBag<Type> AllTypeList { get; set; } = new ConcurrentBag<Type>();

        internal static ConcurrentBag<Type> InterfaceList { get; set; } = new ConcurrentBag<Type>();

        internal static ConcurrentBag<Type> EngineTypeList { get; set; } = new ConcurrentBag<Type>();

        internal static ConcurrentDictionary<string, List<Type>> BHoMTypeDictionary { get; set; } = new ConcurrentDictionary<string, List<Type>>();

        internal static ConcurrentBag<MethodInfo> BHoMMethodList { get; set; } = new ConcurrentBag<MethodInfo>();

        internal static ConcurrentBag<MethodBase> AllMethodList { get; set; } = new ConcurrentBag<MethodBase>();

        internal static ConcurrentBag<MethodBase> ExternalMethodList { get; set; } = new ConcurrentBag<MethodBase>();


        /***************************************************/
        /****        Internal properties - locks        ****/
        /***************************************************/

        internal static object AssemblyReflectionLock { get; } = new object();

        internal static object DebugLogLock { get; } = new object();

        internal static Regex OmNamespacePattern { get; }

        internal static Regex EngineNamespacePattern { get; }

        internal static Regex AdapterNamespacePattern { get; }


        /***************************************************/
        /****            Static constructor             ****/
        /***************************************************/

        static Global()
        {
            OmNamespacePattern = new Regex(@"^BH\.(\w+\.)?oM\.");
            EngineNamespacePattern = new Regex(@"^BH\.(\w+\.)?Engine\.");
            AdapterNamespacePattern = new Regex(@"^BH\.Adapter");

            // Subscribe to the assembly load event.
            AppDomain.CurrentDomain.AssemblyLoad += ReflectAssemblyOnLoad;

            // Dedicated assembly resolution mechanism to minimise issues related with dependency incompatibility
            AppDomain.CurrentDomain.AssemblyResolve += ResolveBHoMAssembly;

            // Reflect the assemblies that have already been loaded.
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Compute.ExtractAssembly(asm);
            }
        }


        /***************************************************/
        /****              Private methods              ****/
        /***************************************************/

        private static void ReflectAssemblyOnLoad(object sender, AssemblyLoadEventArgs args)
        {
            Compute.ExtractAssembly(args?.LoadedAssembly);
        }

        /***************************************************/

        private static Assembly ResolveBHoMAssembly(object sender, ResolveEventArgs args)
        {
            // Check whether the issue was caused by a BHoM call
            bool bh = false;
            StackTrace stackTrace = new StackTrace();
            foreach (StackFrame frame in stackTrace.GetFrames())
            {
                Type type = frame?.GetMethod()?.DeclaringType;
                if (type != null && type.Namespace.StartsWith("BH.") && BHoMAssemblies.Values.Contains(type.Assembly))
                {
                    bh = true;
                    break;
                }
            }

            // If it was, try loading the assembly from BHoM folder
            if (bh)
            {
                string[] split = args.Name.Split(',');
                if (split.Length > 1)
                {
                    string assemblyPath = Path.Combine(BH.Engine.Base.Query.BHoMFolder(), $"{split[0]}.dll");
                    if (File.Exists(assemblyPath))
                        return Assembly.LoadFrom(assemblyPath);
                }
            }

            return null;
        }

        /***************************************************/
    }
}




