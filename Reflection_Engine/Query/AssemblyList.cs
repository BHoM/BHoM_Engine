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
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns all BHoM assemblies loaded in the current domain.")]
        [Output("assemblies", "List of BHoM assemblies loaded in the current domain.")]
        public static List<Assembly> BHoMAssemblyList()
        {
            lock (m_GetAssembliesLock)
            {
                if (m_BHoMAssemblies == null)
                    ExtractAllAssemblies();

                return m_BHoMAssemblies;
            }
        }

        /***************************************************/

        [Description("Returns all assemblies loaded in the current domain.")]
        [Output("assemblies", "List of all assemblies loaded in the current domain.")]
        public static List<Assembly> AllAssemblyList()
        {
            lock (m_GetAssembliesLock)
            {
                if (m_AllAssemblies == null)
                    ExtractAllAssemblies();

                return m_AllAssemblies;
            }
        }

        /***************************************************/

        [Description("Refreshes the lists of loaded assemblies and, in consequence, loaded types and methods.")]
        public static void RefreshAssemblyList()
        {
            ExtractAllAssemblies();
            ExtractAllTypes();
            ExtractAllMethods();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractAllAssemblies()
        {
            m_AllAssemblies = AppDomain.CurrentDomain.GetAssemblies().GroupBy(x => x.FullName).Select(g => g.First()).ToList();
            m_BHoMAssemblies = m_AllAssemblies.Where(x => x.IsBHoM()).ToList();
        }

        /***************************************************/

        private static bool IsBHoM(this Assembly assembly)
        {
            string name = assembly.GetName().Name;
            return name.IsOmAssembly() || name.IsEngineAssembly() || name.IsAdapterAssembly() || name.IsUiAssembly();
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<Assembly> m_BHoMAssemblies = null;
        private static List<Assembly> m_AllAssemblies = null;
        private static readonly object m_GetAssembliesLock = new object();

        /***************************************************/
    }
}


