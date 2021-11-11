﻿/*
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
using System.ComponentModel;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Method               ****/
        /***************************************************/

        [Description("Checks whether a given assembly is a BHoM UI assembly.")]
        [Input("assembly", "Assembly to be checked whether it is a BHoM UI assembly.")]
        [Output("isUi", "True if the input assembly is a BHoM UI assembly.")]
        public static bool IsUiAssembly(this Assembly assembly)
        {
            return assembly != null && assembly.GetName().Name.IsUiAssembly();
        }

        /***************************************************/

        [Description("Checks whether a given assembly name follows the BHoM UI assembly naming convention.")]
        [Input("assemblyName", "Assembly name to be checked whether it follows the BHoM UI assembly naming convention.")]
        [Output("isUi", "True if the input assembly name follows the BHoM UI assembly naming convention.")]
        public static bool IsUiAssembly(this string assemblyName)
        {
            return assemblyName != null && (assemblyName.EndsWith("_UI") || assemblyName.Contains("_UI_"));
        }

        /***************************************************/
    }
}


