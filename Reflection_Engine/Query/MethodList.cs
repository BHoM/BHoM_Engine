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
using System.Linq;
using System.Text.RegularExpressions;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<MethodInfo> BHoMMethodList()
        {
            lock (m_GetMethodsLock)
            {
                ExtractMethodsFromNewAssemblies();
                return m_BHoMMethodList;
            }
        }

        /***************************************************/

        public static List<MethodBase> AllMethodList()
        {
            lock (m_GetMethodsLock)
            {
                ExtractMethodsFromNewAssemblies();
                return m_AllMethodList;
            }
        }

        /***************************************************/

        public static List<MethodBase> ExternalMethodList()
        {
            lock (m_GetMethodsLock)
            {
                ExtractMethodsFromNewAssemblies();
                return m_ExternalMethodList;
            }
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractMethodsFromNewAssemblies()
        {
            List<Assembly> assembliesToLoad = BHoMAssemblyList().Where(x => m_AssembliesWithLoadedMethods.All(y => x.GetName().Name != y)).ToList();

            BindingFlags bindingBHoM = BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static;
            foreach (Assembly asm in assembliesToLoad)
            {
                try
                {
                    // Save BHoM objects only
                    if (asm.IsEngineAssembly())
                    {
                        foreach (Type type in asm.GetTypes())
                        {
                            // Get only the BHoM methods
                            if (!type.IsInterface && type.IsAbstract)
                            {
                                MethodInfo[] typeMethods = type.GetMethods(bindingBHoM);
                                m_BHoMMethodList.AddRange(typeMethods.Where(x => x.IsLegal()));
                            }

                            if (type.Name == "External")
                            {
                                MethodInfo getExternalMethods = type.GetMethod("Methods");
                                if (getExternalMethods != null)
                                    m_ExternalMethodList.AddRange((List<MethodInfo>)getExternalMethods.Invoke(null, null));
                                MethodInfo getExternalCtor = type.GetMethod("Constructors");
                                if (getExternalCtor != null)
                                    m_ExternalMethodList.AddRange((List<ConstructorInfo>)getExternalCtor.Invoke(null, null));
                            }
                            // Get everything
                            StoreAllMethods(type);
                        }
                    }
                    else if (asm.IsOmAssembly() || asm.IsAdapterAssembly() || asm.IsUiAssembly())
                    {
                        foreach (Type type in asm.GetTypes())
                        {
                            StoreAllMethods(type);
                        }
                    }
                }
                catch (Exception e)
                {

                    string message = "Cannot load types from assembly " + asm.GetName().Name + ". Exception message: " + e.Message;

                    if (!string.IsNullOrEmpty(e.InnerException?.Message))
                    {
                        message += "\nInnerException: " + e.InnerException.Message;
                    }

                    Compute.RecordWarning(message);
                }

                m_AssembliesWithLoadedMethods.Add(asm.GetName().Name);
            }
        }

        /***************************************************/

        private static void StoreAllMethods(Type type)
        {
            BindingFlags bindingAll = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance;

            if (type.Namespace != null && type.Namespace.StartsWith("BH.") && !type.IsAutoGenerated())
            {
                m_AllMethodList.AddRange(type.GetConstructors(bindingAll).Where(x => x.GetMethodBody() != null && !x.IsAutoGenerated()));

                MethodInfo[] methods = type.GetMethods(bindingAll);
                foreach (var method in methods)
                {
                    try
                    {
                        if (method.GetMethodBody() != null && !method.IsAutoGenerated())
                            m_AllMethodList.Add(method);
                    }
                    catch(Exception e)
                    {
                        string message = "Cannot load method" + method.Name + " from type  " + type.Name + ". Exception message: " + e.Message;

                        if (!string.IsNullOrEmpty(e.InnerException?.Message))
                        {
                            message += "\nInnerException: " + e.InnerException.Message;
                        }

                        Compute.RecordWarning(message);
                    }
                }
            }
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<string> m_AssembliesWithLoadedMethods = new List<string>();
        private static List<MethodInfo> m_BHoMMethodList = new List<MethodInfo>();
        private static List<MethodBase> m_AllMethodList = new List<MethodBase>();
        private static List<MethodBase> m_ExternalMethodList = new List<MethodBase>();
        private static readonly object m_GetMethodsLock = new object();

        /***************************************************/
    }
}

