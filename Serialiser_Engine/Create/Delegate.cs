/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using System.Linq.Expressions;
using System.Reflection;

namespace BH.Engine.Serialiser
{
    public static partial class Create
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static Delegate Delegate(this MethodInfo method, Type target)
        {
            ParameterInfo[] parameters = method.GetParameters();

            List<Type> parameterTypes = parameters.Select(x => x.ParameterType).ToList();
            parameterTypes.Add(method.ReturnType);
            if (!method.IsStatic)
                parameterTypes.Insert(0, target);

            object firstArgument = method.IsStatic ? null : target ?? method.DeclaringType;

            if (method.IsGenericMethod || method.ContainsGenericParameters)
            {
                //Type[] types = method.GetGenericArguments().Select(x => GetConstructedType(x)).ToArray();
                //method = method.MakeGenericMethod(types);
                //return method;
                return null;
            }

            Type delegateType = Expression.GetDelegateType(parameterTypes.ToArray());
            Delegate @delegate = System.Delegate.CreateDelegate(delegateType, method);


            return @delegate;
        }

        /*******************************************/
    }
}
