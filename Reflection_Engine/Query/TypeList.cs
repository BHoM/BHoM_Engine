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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Type> BHoMInterfaceList()
        {
            lock (m_GetTypesLock)
            {
                // If the dictionary exists already return it
                if (m_InterfaceList != null && m_InterfaceList.Count > 0)
                    return m_InterfaceList;

                // Otherwise, create it
                ExtractAllTypes();

                return m_InterfaceList;
            }
        }

        /***************************************************/

        public static List<Type> BHoMTypeList()
        {
            lock (m_GetTypesLock)
            {
                // If the dictionary exists already return it
                if (m_BHoMTypeList != null && m_BHoMTypeList.Count > 0)
                    return m_BHoMTypeList;

                // Otherwise, create it
                ExtractAllTypes();

                return m_BHoMTypeList;
            }
        }

        /***************************************************/

        public static List<Type> AdapterTypeList()
        {
            lock (m_GetTypesLock)
            {
                // If the dictionary exists already return it
                if (m_AdapterTypeList != null && m_AdapterTypeList.Count > 0)
                    return m_AdapterTypeList;

                // Otherwise, create it
                ExtractAllTypes();

                return m_AdapterTypeList;
            }
        }

        /***************************************************/

        public static List<Type> AllTypeList()
        {
            lock (m_GetTypesLock)
            {
                // If the dictionary exists already return it
                if (m_AllTypeList != null && m_AllTypeList.Count > 0)
                    return m_AllTypeList;

                // Otherwise, create it
                ExtractAllTypes();

                return m_AllTypeList;
            }
        }

        /***************************************************/

        public static List<Type> EngineTypeList()
        {
            lock (m_GetMethodsLock)
            {
                // If the dictionary exists already return it
                if (m_EngineTypeList != null && m_EngineTypeList.Count > 0)
                    return m_EngineTypeList;

                // Otherwise, create it
                ExtractAllMethods();

                return m_EngineTypeList;
            }
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractAllTypes()
        {
            m_BHoMTypeList = new List<Type>();
            m_AdapterTypeList = new List<Type>();
            m_InterfaceList = new List<Type>();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // Save BHoM objects only
                    string name = asm.GetName().Name;
                    if (name == "BHoM" || name.EndsWith("_oM"))
                    {
                        foreach (Type type in asm.GetTypes())
                        {
                            if (type.Namespace != null && type.Namespace.StartsWith("BH.oM") && typeof(IObject).IsAssignableFrom(type))
                            {
                                AddBHoMTypeToDictionary(type.FullName, type);
                                if (!type.IsInterface && !(type.IsAbstract && type.IsSealed)) // Avoid interfaces and static classes
                                    m_BHoMTypeList.Add(type);
                                else
                                    m_InterfaceList.Add(type);
                            }
                            if (!type.IsAutoGenerated())
                                m_AllTypeList.Add(type);
                        }
                    }
                    // Save adapters
                    else if (name.EndsWith("_Adapter"))
                    {
                        foreach (Type type in asm.GetTypes())
                        {
                            if (!type.IsAutoGenerated())
                            {
                                if (!type.IsInterface && type.IsLegal() && type.IsOfType("BHoMAdapter"))
                                    m_AdapterTypeList.Add(type);

                                m_AllTypeList.Add(type);
                            }
                                
                        }
                    }
                    else
                    {
                        foreach (Type type in asm.GetTypes())
                        {
                            if (type.Namespace != null && type.Namespace.StartsWith("BH.") && !type.IsAutoGenerated())
                                m_AllTypeList.Add(type);
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

        private static bool IsOfType(this Type type, string match)
        {
            Type baseType = type.BaseType;
            if (baseType == null || baseType.Name == "Object")
                return false;
            else if (baseType.Name == match)
                return true;
            else
                return baseType.IsOfType(match);
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<Type> m_BHoMTypeList = new List<Type>();
        private static List<Type> m_AdapterTypeList = new List<Type>();
        private static List<Type> m_AllTypeList = new List<Type>();
        private static List<Type> m_InterfaceList = new List<Type>();
        private static readonly object m_GetTypesLock = new object();

        /***************************************************/
    }
}


