/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Reflection;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<MethodInfo> ExtensionMethods(this Type type, string methodName)
        {
            List<MethodInfo> methods = new List<MethodInfo>();

            foreach (MethodInfo method in BHoMMethodList().Where(x => x.Name == methodName))
            {
                ParameterInfo[] param = method.GetParameters();

                if (param.Length > 0)
                {
                    if (param[0].ParameterType.IsAssignableFromIncludeGenerics(type))
                        methods.Add(method);
                }
            }

            return methods;
        }

        /***************************************************/
    }
}






