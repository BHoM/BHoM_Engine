/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

        [Description("Attempts to load an assembly under the given path.")]
        [Input("assemblyPath", "Path from which the assembly is meant to be loaded.")]
        [Output("assembly", "The assembly under the given path, if it exists and has been loaded to BHoM (at any point in time), otherwise null.")]
        public static Assembly LoadAssembly(string assemblyPath)
        {
            try
            {
                string name = AssemblyName.GetAssemblyName(assemblyPath).FullName;
                if (!Global.AllAssemblies.ContainsKey(name))
                    return Assembly.LoadFrom(assemblyPath);
                else
                    return Global.AllAssemblies[name];
            }
            catch
            {
                RecordWarning("Failed to load assembly " + assemblyPath);
                return null;
            }
        }
    }
}



