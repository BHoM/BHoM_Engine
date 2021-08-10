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
using System.IO;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether the assembly the item is housed in is part of a prototype assembly.")]
        public static bool IsPrototype(this object item)
        {
            if (m_CoreAssemblyPaths == null)
                ExtractCoreAssemblyPaths();

            return !m_CoreAssemblyPaths.Contains(System.IO.Path.GetFileName(item.IAssemblyPath()));
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractCoreAssemblyPaths()
        {
            string refFile = @"C:\ProgramData\BHoM\Settings\IncludedDlls.txt";

            if (File.Exists(refFile))
                m_CoreAssemblyPaths = new HashSet<string>(File.ReadAllLines(refFile).Select(x => System.IO.Path.GetFileName(x)));
            else
                m_CoreAssemblyPaths = new HashSet<string>();
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static HashSet<string> m_CoreAssemblyPaths = null;

        /***************************************************/
    }
}

