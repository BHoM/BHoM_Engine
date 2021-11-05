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

        //internal static List<string> LoadedAssemblies { get; set; } = null;

        internal static List<Assembly> BHoMAssemblies { get; set; } = null;

        internal static List<Assembly> AllAssemblies { get; set; } = null;

        internal static List<Type> BHoMTypeList { get; set; } = null;

        internal static List<Type> AdapterTypeList { get; set; } = null;

        internal static List<Type> AllTypeList { get; set; } = null;

        internal static List<Type> InterfaceList { get; set; } = null;

        internal static List<Type> EngineTypeList { get; set; } = null;

        internal static Dictionary<string, List<Type>> BHoMTypeDictionary { get; set; } = null;

        internal static List<MethodInfo> BHoMMethodList { get; set; } = null;

        internal static List<MethodBase> AllMethodList { get; set; } = null;

        internal static List<MethodBase> ExternalMethodList { get; set; } = null;

        /***************************************************/
    }
}
