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

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Delegate Delegate(this MethodBase method)
        {
            if (method is MethodInfo)
                return ((MethodInfo)method).Delegate();
            else if (method is ConstructorInfo)
                return ((ConstructorInfo)method).Delegate();
            return null;
        }

        /***************************************************/

        public static Delegate Delegate(this MethodInfo method)
        {
            ParameterExpression lambdaInput = Expression.Parameter(typeof(object[]), "x");
            Expression[] inputs = method.GetParameters().Select((x, i) => Expression.Convert(Expression.ArrayIndex(lambdaInput, Expression.Constant(i)), x.ParameterType)).ToArray();

            MethodCallExpression methodExpression;
            if (method.IsStatic)
            {
                methodExpression = Expression.Call(method, inputs);
                if (method.ReturnType == typeof(void))
                    return Expression.Lambda<Action<object[]>>(Expression.Convert(methodExpression, typeof(void)), lambdaInput).Compile();
                else
                    return Expression.Lambda<Func<object[], object>>(Expression.Convert(methodExpression, typeof(object)), lambdaInput).Compile();
            }
            else
            {
                ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
                Expression instanceInput = Expression.Convert(instanceParameter, method.DeclaringType);
                methodExpression = Expression.Call(instanceInput, method, inputs);

                if (method.ReturnType == typeof(void))
                {
                    return Expression.Lambda<Action<object, object[]>>(
                        Expression.Convert(methodExpression, typeof(void)),
                        new ParameterExpression[] { instanceParameter, lambdaInput }
                        ).Compile();
                }
                else
                {
                    return Expression.Lambda<Func<object, object[], object>>(
                        Expression.Convert(methodExpression, typeof(object)),
                        new ParameterExpression[] { instanceParameter, lambdaInput }
                        ).Compile();
                }
            }
        }

        /***************************************************/

        public static Func<object[], object> Delegate(this ConstructorInfo ctor)
        {
            ParameterExpression lambdaInput = Expression.Parameter(typeof(object[]), "x");
            Expression[] inputs = ctor.GetParameters().Select((x, i) => Expression.Convert(Expression.ArrayIndex(lambdaInput, Expression.Constant(i)), x.ParameterType)).ToArray();
            NewExpression constructorExpression = Expression.New(ctor as ConstructorInfo, inputs);
            return Expression.Lambda<Func<object[], object>>(Expression.Convert(constructorExpression, typeof(object)), lambdaInput).Compile();
        }
    }
}
