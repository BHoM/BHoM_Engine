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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string Path(this Type type)
        {
            if(type == null)
            {
                Compute.RecordError("Cannot query the path of a null type.");
                return null;
            }

            return type.Namespace;
        }

        /***************************************************/

        public static string Path(this MethodBase method, bool userReturnTypeForCreate = true, bool useExtentionType = false) 
        {
            if(method == null)
            {
                Compute.RecordError("Cannot query the path of a null method base.");
                return null;
            }

            Type type = method.DeclaringType;

            if (userReturnTypeForCreate && type.Name == "Create" && method is MethodInfo)
            {
                Type returnType = ((MethodInfo)method).ReturnType.UnderlyingType().Type;
                if (returnType.Namespace.StartsWith("BH."))
                    type = returnType;
            }
            else if (useExtentionType && method.IsDefined(typeof(ExtensionAttribute), false))
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length > 0)
                    type = parameters[0].ParameterType;
            }

            return type.ToText(true, true);
        }

        /***************************************************/
    }
}



