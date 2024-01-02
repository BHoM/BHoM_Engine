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

        public static List<string> UsedNamespaces(this MethodBase method, bool onlyBHoM = false, int maxDepth = 10)
        {
            if(method == null)
            {
                Base.Compute.RecordWarning("Cannot query the used namespaces of a null method. An empty list will be returned as the list of used namespaces.");
                return new List<string>();
            }

            return method.UsedTypes(onlyBHoM).Select(x => ClipNamespace(x.Namespace, maxDepth)).Distinct().ToList();
        }

        /***************************************************/

        public static List<string> UsedNamespaces(this Type type, bool onlyBHoM = false, int maxDepth = 10)
        {
            if(type == null)
            {
                Base.Compute.RecordWarning("Cannot query the used namespaces of a null type. An empty list will be returned as the list of used namespaces.");
                return new List<string>();
            }

            return type.UsedTypes(onlyBHoM).Select(x => ClipNamespace(x.Namespace, maxDepth)).Distinct().ToList();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string ClipNamespace(string nameSpace, int maxDepth)
        {
            return nameSpace.Split('.').Take(maxDepth).Aggregate((a, b) => a + '.' + b);
        }

        /***************************************************/
    }
}





