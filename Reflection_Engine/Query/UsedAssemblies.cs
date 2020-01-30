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
using Mono.Cecil;
using Mono.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Assembly> UsedAssemblies(this Assembly assembly, bool onlyBHoM = false)
        {
            try
            {
                if (m_AllAssemblies == null || m_AllAssemblies.Count == 0)
                    ExtractAllAssemblies();

                IEnumerable<AssemblyName> assemblyNames = assembly.GetReferencedAssemblies();
                foreach (AssemblyName name in assemblyNames)
                    Compute.RecordWarning("Could not find the assembly with the name " + name);

                return assemblyNames.Where(x => m_BHoMAssemblies.ContainsKey(x.FullName)).Select(x => m_BHoMAssemblies[x.FullName]).ToList();
            }
            catch (Exception e)
            {
                Compute.RecordWarning("failed to get the assemblies used by " + assembly.FullName + ".\nError: " + e.ToString());
                return new List<Assembly>();
            }
        }
            

        /***************************************************/
    }
}

