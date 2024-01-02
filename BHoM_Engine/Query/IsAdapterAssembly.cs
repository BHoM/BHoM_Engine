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

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Method               ****/
        /***************************************************/

        [Description("Checks whether a given assembly is a BHoM adapter assembly.")]
        [Input("assembly", "Assembly to be checked whether it is a BHoM adapter assembly.")]
        [Output("isAdapter", "True if the input assembly is a BHoM adapter assembly.")]
        public static bool IsAdapterAssembly(this Assembly assembly)
        {
            return assembly != null && assembly.GetName().Name.IsAdapterAssembly();
        }

        /***************************************************/

        [Description("Checks whether a given assembly name follows the BHoM adapter assembly naming convention.")]
        [Input("assemblyName", "Assembly name to be checked whether it follows the BHoM adapter assembly naming convention.")]
        [Output("isAdapter", "True if the input assembly name follows the BHoM adapter assembly naming convention.")]
        public static bool IsAdapterAssembly(this string assemblyName)
        {
            return assemblyName != null && (assemblyName.EndsWith("_Adapter") || assemblyName.Contains("_Adapter_"));
        }

        /***************************************************/
    }
}





