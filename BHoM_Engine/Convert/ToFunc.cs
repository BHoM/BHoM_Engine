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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts the provided MethodBase into a compiled Func.")]
        public static Func<object[], object> ToFunc(this MethodBase method)
        {
            if (method is MethodInfo)
                return ((MethodInfo)method).ToFunc();
            else if (method is ConstructorInfo)
                return ((ConstructorInfo)method).ToFunc();
            return null;
        }

        /***************************************************/

        [Description("Converts the provided MethodInfo into a compiled Func by creation and compilation of the expression tree corresponding to the method. Methods returning void will return a boolean value, always true.")]
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

        [Description("Converts the provided ConstructorInfo into a compiled Func by creation and compilation of the expression tree corresponding to the method.")]
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

        [Description("Converts the Action into a Func that calls the Action and returns true.")]
        public static Func<object[], object> ToFunc(this Action<object[]> act)
        {
            return inputs => { act(inputs); return true; };
        }

        /***************************************************/

        [Description("Converts the provided Func into a new Func where the inputs have been simplified into a single array.")]
        public static Func<object[], object> ToFunc(this Func<object, object[], object> func)
        {
            return inputs => { return func(inputs[0], inputs.Skip(1).ToArray()); };
        }

        /***************************************************/

        [Description("Converts the provided Action into a  Func where the inputs have been simplified into a single array. Function will call the Action and return true.")]
        public static Func<object[], object> ToFunc(this Action<object, object[]> act)
        {
            return inputs => { act(inputs[0], inputs.Skip(1).ToArray()); return true; };
        }

        /***************************************************/

        [Description("Compiles only the getter of the PropertyInfo into a callable `Func<object, object>`. This is a delegate of the getter method, which can be invoked like a property selector by providing an instance of the property owner type. No Type match checking will be done by the function; an exception will be thrown if the Func is used with a type that is not compatible with the PropertyInfo.")]
        public static Func<object, object> ToFunc(this PropertyInfo prop)
        {
            if (prop == null)
                return null;

            var method = typeof(Convert)
                      .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                      .Single(m => m.Name == nameof(CompileCastProperty) && m.IsGenericMethodDefinition && m.GetParameters().Count() == 1);

            MethodInfo generic = method.MakeGenericMethod(new Type[] { prop.DeclaringType, prop.PropertyType });
            return (Func<object, object>)generic.Invoke(null, new object[] { prop });
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Creates a delegate of type Func<T,P> that matches that of the provided PropertyInfo. Returns a Func<object,object> that casts the object into a T.")]
        private static Func<object, object> CompileCastProperty<T, P>(PropertyInfo prop)
        {
            Func<T, P> func = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), prop.GetGetMethod());
            return x => func((T)x);
        }

        /***************************************************/

        // Adds support for ByRef types like `System.Double&`
        private static Type GetTypeIfRef(this Type t)
        {
            if (t.IsByRef)
                t = t.GetElementType();

            return t;
        }

        /***************************************************/
    }
}




