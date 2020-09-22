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
using System.Reflection;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Assembly> BHoMAssemblyList()
        {
            lock (m_GetAssembliesLock)
            {
                if (m_BHoMAssemblies == null || m_BHoMAssemblies.Count == 0)
                    ExtractAllAssemblies();

                return m_BHoMAssemblies.Values.ToList();
            }
        }

        /***************************************************/

        public static List<Assembly> AllAssemblyList()
        {
            lock (m_GetAssembliesLock)
            {
                if (m_AllAssemblies == null || m_AllAssemblies.Count == 0)
                    ExtractAllAssemblies();

                return m_AllAssemblies.Values.ToList();
            }
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractAllAssemblies()
        {
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
            m_BHoMAssemblies = assemblies.Where(x => x.IsBHoM()).ToDictionary(x => x.FullName);
            m_AllAssemblies = assemblies.GroupBy(x => x.FullName).Select(g => g.First()).ToDictionary(x => x.FullName);
        }

        /***************************************************/

        private static bool IsBHoM(this Assembly assembly)
        {
            string name = assembly.GetName().Name;
            return name.EndsWith("oM") || name.EndsWith("_Engine") || name.EndsWith("_Adapter") || name.EndsWith("_UI") || name.EndsWith("_Test");
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, Assembly> m_BHoMAssemblies = null;
        private static Dictionary<string, Assembly> m_AllAssemblies = null;
        private static readonly object m_GetAssembliesLock = new object();

        /***************************************************/
    }
}

