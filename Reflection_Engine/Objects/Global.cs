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
using System.Reflection;

namespace BH.Engine.Reflection
{
    internal static class Global
    {
        /***************************************************/
        /****     Internal properties - collections     ****/
        /***************************************************/

        internal static Dictionary<string, Assembly> BHoMAssemblies { get; set; } = new Dictionary<string, Assembly>();

        internal static Dictionary<string, Assembly> AllAssemblies { get; set; } = new Dictionary<string, Assembly>();

        internal static List<Type> BHoMTypeList { get; set; } = new List<Type>();

        internal static List<Type> AdapterTypeList { get; set; } = new List<Type>();

        internal static List<Type> AllTypeList { get; set; } = new List<Type>();

        internal static List<Type> InterfaceList { get; set; } = new List<Type>();

        internal static List<Type> EngineTypeList { get; set; } = new List<Type>();

        internal static Dictionary<string, List<Type>> BHoMTypeDictionary { get; set; } = new Dictionary<string, List<Type>>();

        internal static List<MethodInfo> BHoMMethodList { get; set; } = new List<MethodInfo>();

        internal static List<MethodBase> AllMethodList { get; set; } = new List<MethodBase>();

        internal static List<MethodBase> ExternalMethodList { get; set; } = new List<MethodBase>();


        /***************************************************/
        /****            Static constructor             ****/
        /***************************************************/

        static Global()
        {
            // Subscribe to the assembly load event.
            AppDomain.CurrentDomain.AssemblyLoad += ReflectAssemblyOnLoad;

            // Reflect the assemblies that have already been loaded.
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Compute.ReflectAssembly(asm);
            }
        }


        /***************************************************/
        /****              Private methods              ****/
        /***************************************************/

        private static void ReflectAssemblyOnLoad(object sender, AssemblyLoadEventArgs args)
        {
            Compute.ReflectAssembly(args?.LoadedAssembly);
        }

        /***************************************************/
    }
}
