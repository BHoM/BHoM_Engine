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
using System.Reflection;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<MethodInfo> BHoMMethodList()
        {
            // If the dictionary exists already return it
            if (m_BHoMMethodList != null && m_BHoMMethodList.Count > 0)
                return m_BHoMMethodList;

            // Otherwise, create it
            ExtractAllMethods();

            return m_BHoMMethodList;
        }

        /***************************************************/

        public static List<MethodBase> AllMethodList()
        {
            // If the dictionary exists already return it
            if (m_AllMethodList != null && m_AllMethodList.Count > 0)
                return m_AllMethodList;

            // Otherwise, create it
            ExtractAllMethods();

            return m_AllMethodList;
        }

        /***************************************************/

        public static List<MethodBase> ExternalMethodList()
        {
            // Checking for an empty list may be dangerous, we should give different meaning to null and empty lists
            // What if m_ExternalMethodList is empty after calling ExtractAllMethods() ?
            if (m_ExternalMethodList == null || m_ExternalMethodList.Count <= 0)
                ExtractAllMethods();

            return m_ExternalMethodList;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractAllMethods()
        {
            Compute.LoadAllAssemblies();

            m_BHoMMethodList = new List<MethodInfo>();
            m_AllMethodList = new List<MethodBase>();

            BindingFlags bindingBHoM = BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static;

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // Save BHoM objects only
                    string name = asm.GetName().Name;
                    if (name.EndsWith("_Engine"))
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
                    else if (name.EndsWith("oM") || name.EndsWith("_Adapter") || name.EndsWith("_UI") || name.EndsWith("_Test"))
                    {
                        foreach (Type type in asm.GetTypes())
                        {
                            StoreAllMethods(type);
                        }
                    }
                }
                catch (Exception)
                {
                    Compute.RecordWarning("Cannot load types from assembly " + asm.GetName().Name);
                }
            }
        }

        /***************************************************/

        private static void StoreAllMethods(Type type)
        {
            BindingFlags bindingAll = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance;

            if (type.Namespace != null && type.Namespace.StartsWith("BH.") && !type.IsAutoGenerated())
            {
                m_AllMethodList.AddRange(type.GetMethods(bindingAll).Where(x => x.GetMethodBody() != null && !x.IsAutoGenerated()));
                m_AllMethodList.AddRange(type.GetConstructors(bindingAll).Where(x => x.GetMethodBody() != null && !x.IsAutoGenerated()));
            }
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<MethodInfo> m_BHoMMethodList = new List<MethodInfo>();
        private static List<MethodBase> m_AllMethodList = new List<MethodBase>();
        private static List<MethodBase> m_ExternalMethodList = new List<MethodBase>();

        /***************************************************/
    }
}
