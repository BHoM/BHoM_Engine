/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the specific type from a type that contains generic parameters")]
        [Input("genericType", "The generic type")]
        [Output("type", "The specific type constructed from the generic one")]
        public static MethodInfo MakeStatic(this MethodInfo method)
        {
            if (method == null)
                return null;

            ParameterInfo[] parameters = method.GetParameters();

            List<Type> parameterTypes = parameters.Select(x => x.ParameterType).ToList();
            parameterTypes.Add(method.ReturnType);
            if (!method.IsStatic)
                parameterTypes.Insert(0, method.DeclaringType);

            object firstArgument = method.IsStatic ? null : method.DeclaringType;

            if (method.IsGenericMethod || method.ContainsGenericParameters)
                return null; //method = method.ConstructGeneric();

            Type delegateType = Expression.GetDelegateType(parameterTypes.ToArray());
            Delegate @delegate = Delegate.CreateDelegate(delegateType, method);

            return @delegate.Method;
        }

        /***************************************************/
    }
}