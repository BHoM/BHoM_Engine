/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Reflection;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using BH.oM.Base;
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Records the given assembly in the Global collection of loaded assemblies, then extracts types and methods from it and adds them to relevant collections in the Global class.")]
        [Input("assembly", "Assembly to be reflected.")]
        [PreviousVersion("5.1", "BH.Engine.Reflection.Compute.ReflectAssembly(System.Reflection.Assembly)")]
        public static void ExtractAssembly(Assembly assembly)
        {
            if (assembly == null || assembly.ReflectionOnly)
                return;

            lock (Global.AssemblyReflectionLock)
            {
                if (Global.AllAssemblies.ContainsKey(assembly.FullName))
                    return;

                Global.AllAssemblies[assembly.FullName] = assembly;
                if (assembly.IsBHoM())
                {
                    Global.BHoMAssemblies[assembly.FullName] = assembly;
                    ExtractTypes(assembly);
                    ExtractMethods(assembly);
                }
            }
        }
    }
}

