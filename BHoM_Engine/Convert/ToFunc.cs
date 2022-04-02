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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Func<object[], object> ToFunc(this MethodBase method)
        {
            if (method is MethodInfo)
                return ((MethodInfo)method).ToFunc();
            else if (method is ConstructorInfo)
                return ((ConstructorInfo)method).ToFunc();
            return null;
        }

        /***************************************************/

        public static Func<object[], object> ToFunc(this MethodInfo method)
        {
            if(method == null)
            {
                Compute.RecordError("Cannot convert method info to func if method is null.");
                return null;
            }

            ParameterExpression lambdaInput = Expression.Parameter(typeof(object[]), "x");
            Expression[] inputs = method.GetParameters().Select((x, i) => Expression.Convert(Expression.ArrayIndex(lambdaInput, Expression.Constant(i)), x.ParameterType.GetTypeIfRef())).ToArray();

            MethodCallExpression methodExpression;
            if (method.IsStatic)
            {
                methodExpression = Expression.Call(method, inputs);
                if (method.ReturnType == typeof(void))
                    return Expression.Lambda<Action<object[]>>(Expression.Convert(methodExpression, typeof(void)), lambdaInput).Compile().ToFunc();
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
                        ).Compile().ToFunc();
                }
                else
                {
                    return Expression.Lambda<Func<object, object[], object>>(
                        Expression.Convert(methodExpression, typeof(object)),
                        new ParameterExpression[] { instanceParameter, lambdaInput }
                        ).Compile().ToFunc();
                }
            }
        }

        /***************************************************/

        public static Func<object[], object> ToFunc(this ConstructorInfo ctor)
        {
            if(ctor == null)
            {
                Compute.RecordError("Cannot convert constructor info to func if the constructor is null.");
                return null;
            }

            ParameterExpression lambdaInput = Expression.Parameter(typeof(object[]), "x");
            Expression[] inputs = ctor.GetParameters().Select((x, i) => Expression.Convert(Expression.ArrayIndex(lambdaInput, Expression.Constant(i)), x.ParameterType)).ToArray();
            NewExpression constructorExpression = Expression.New(ctor as ConstructorInfo, inputs);
            return Expression.Lambda<Func<object[], object>>(Expression.Convert(constructorExpression, typeof(object)), lambdaInput).Compile();
        }

        /***************************************************/

        public static Func<object[], object> ToFunc(this Action<object[]> act)
        {
            return inputs => { act(inputs); return true; };
        }

        /***************************************************/

        public static Func<object[], object> ToFunc(this Func<object, object[], object> func)
        {
            return inputs => { return func(inputs[0], inputs.Skip(1).ToArray()); };
        }

        /***************************************************/

        public static Func<object[], object> ToFunc(this Action<object, object[]> act)
        {
            return inputs => { act(inputs[0], inputs.Skip(1).ToArray()); return true; };
        }

        /***************************************************/

        // Adds support for ByRef types like `System.Double&`
        private static Type GetTypeIfRef(this Type t)
        {
            if (t.IsByRef)
                t = t.GetElementType();

            return t;
        }
    }
}

